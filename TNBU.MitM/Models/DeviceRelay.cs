using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using TNBU.Core.Models;
using TNBU.Core.Utils;
using TNBU.MitM.Services;
using TNBU.MitM.Utils;

namespace TNBU.MitM.Models;

public class DeviceRelay {
	public PhysicalAddress Mac { get; }
	public PhysicalAddress FakeMac { get; }
	public string Serial => Mac.ToString().ToLowerInvariant();
	public string FakeSerial => FakeMac.ToString().ToLowerInvariant();
	public IPAddress IP { get; }
	public IPAddress FakeIP { get; }
	public IPAddress Netmask { get; }
	public IPAddress BroadcastIP { get; }
	public IPEndPoint BroadcastEP { get; }
	public string? InformURL { get; set; }
	public string AuthKey { get; set; } = InformPacket.DEFAULT_KEY;
	public string FakeInformUrl => $"http://{FakeIP}:8081/inform";

	private readonly Socket udpClient = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
	private readonly DeviceSSHService sshService;
	private readonly ILogger logger;

	private static readonly HttpClient client = new(new SocketsHttpHandler {
		ActivityHeadersPropagator = null
	});
	static DeviceRelay() {
		client.DefaultRequestHeaders.Accept.Clear();
		client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
	}

	public DeviceRelay(DiscoveryPacket dp, DeviceSSHService _sshService, ILogger _logger) {
		Mac = dp.Mac;
		FakeMac = NetworkUtils.GetFakeMacAddress(Mac);
		IP = dp.IP;
		(FakeIP, Netmask) = NetworkUtils.GetMyIPOnThisSubnet(IP);
		BroadcastIP = FakeIP.GetBroadcastAddress(Netmask);
		BroadcastEP = new(BroadcastIP, 10001);
		sshService = _sshService;
		sshService.Owner = this;
		logger = _logger;
	}

	public async Task<byte[]> HandleInform(InformPacket req) {
		req.Decrypt(AuthKey);

		req.MACAddress = FakeMac;

		var decodedBody = JsonConvert.DeserializeObject<JObject>(req.Body)!;
		var fingerprint = decodedBody["fingerprint"];

		req.Body = req.Body.Replace(FakeInformUrl, InformURL);
		req.Body = req.Body.Replace(IP.ToString(), FakeIP.ToString());
		req.Body = req.Body.Replace(Serial, FakeSerial);
		if(fingerprint != null) {
			req.Body = req.Body.Replace(fingerprint.ToString(), sshService.Hash);
		}
		req.Body = req.Body.Replace(HexConversions.BytesToColon(Mac.GetAddressBytes()), HexConversions.BytesToColon(FakeMac.GetAddressBytes()));

		var byteArrayContent = new ByteArrayContent(req.Encode(AuthKey));
		byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-binary");
		var response = await client.PostAsync(InformURL, byteArrayContent);
		response.EnsureSuccessStatusCode();

		var respdata = response.Content.ReadAsStream();
		var rawstream = new MemoryStream();
		respdata.CopyTo(rawstream);
		rawstream.Position = 0;

		var inform_resp = InformPacket.Decode(rawstream);
		inform_resp.Decrypt(AuthKey);
		inform_resp.MACAddress = Mac;
		return inform_resp.Encode(AuthKey);
	}

	public void HandleDiscovery(DiscoveryPacket dp) {
		dp.SetPayloadAsMac(DiscoveryPacket.PAYLOAD_MAC, FakeMac);
		dp.SetPayloadAsMacIp(DiscoveryPacket.PAYLOAD_MACIP, FakeMac, FakeIP);
		dp.SetPayloadAsMac(DiscoveryPacket.PAYLOAD_SERIAL, FakeMac);
		dp.SetPayloadAsUShort(DiscoveryPacket.PAYLOAD_SSHPORT, sshService.Port);
		udpClient.SendTo(dp.Encode(), BroadcastEP);
	}

	public (string response, int exitCode) HandleSSHExec(string cmd, string user, string password) {
		if(cmd.StartsWith("/usr/bin/syswrapper.sh set-adopt")) {
			var args = cmd.Split(' ');
			InformURL = args[2];
			AuthKey = args[3];
			var newcmd = $"/usr/bin/syswrapper.sh set-adopt {FakeInformUrl} {AuthKey}";
			using var client = new Renci.SshNet.SshClient(IP.ToString(), user, password);
			client.Connect();
			var ret = client.RunCommand(newcmd);
			var result = ret.Execute();
			logger.LogInformation("Response from real device: \"{result}\" exit code is {exit}", result, ret.ExitStatus);
			return (result, ret.ExitStatus);
		}
		throw new NotImplementedException(cmd);
	}
}
