using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using TNBU.Core.Models;
using TNBU.Core.Utils;
using TNBU.MitM.Services;

namespace TNBU.MitM.Models;

public class DeviceRelay {
	public PhysicalAddress Mac { get; }
	public PhysicalAddress FakeMac { get; }
	public string Serial => Mac.ToString().ToLowerInvariant();
	public string FakeSerial => FakeMac.ToString().ToLowerInvariant();
	public IPAddress IP { get; }
	public IPAddress FakeIP { get; }
	public IPAddress Netmask { get; }
	public string? InformURL { get; set; }
	public string AuthKey { get; set; } = InformPacket.DEFAULT_KEY;
	public string FakeInformUrl => $"http://{FakeIP}:8081/inform";

	private readonly DeviceSSHService sshService;
	private readonly ILogger logger;
	private readonly string cfgPath;
	private readonly string logPath;

	private static readonly HttpClient client = new(new SocketsHttpHandler {
		ActivityHeadersPropagator = null
	});
	static DeviceRelay() {
		client.DefaultRequestHeaders.Accept.Clear();
		client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
	}

	public DeviceRelay(PhysicalAddress _mac, IPAddress _ip, DeviceSSHService _sshService, ILogger _logger, string _cfgPath, string _logPath) {
		Mac = _mac;
		FakeMac = NetworkUtils.GetFakeMacAddress(Mac);
		IP = _ip;
		(FakeIP, Netmask) = NetworkUtils.GetMyIPOnThisSubnet(IP);
		sshService = _sshService;
		sshService.Owner = this;
		logger = _logger;
		cfgPath = _cfgPath;
		logPath = _logPath;
		if(File.Exists(_cfgPath)) {
			var cfg = JsonConvert.DeserializeObject<DeviceCFG>(File.ReadAllText(cfgPath))!;
			AuthKey = cfg.AuthKey;
			InformURL = cfg.InformURL;
		}
	}

	public async Task<byte[]> HandleInform(InformPacket req) {
		string logfile;
		var counter = 0;
		do {
			logfile = Path.Combine(logPath, DateTime.Now.ToString("yyyyMMdd-HHmmss-") + ++counter + ".txt");
		} while(File.Exists(logfile));

		req.Decrypt(AuthKey);

		var logdata = $"---- REQUEST ----\n";
		logdata += $"AuthKey {AuthKey}\n";
		logdata += $"IsAES {(req.IsAES ? "Y" : "N")}\n";
		logdata += $"IsZLIB {(req.IsZLIB ? "Y" : "N")}\n";
		logdata += $"IsGCM {(req.IsGCM ? "Y" : "N")}\n";
		logdata += $"IsSnappy {(req.IsSnappy ? "Y" : "N")}\n";
		logdata += $"MACAddress {req.MACAddress}\n";
		logdata += $"Version {req.Version}\n";
		logdata += $"PayloadVersion {req.PayloadVersion}\n";
		logdata += $"IV {req.IV.BytesToColon()}\n";
		logdata += $"\n{req.Body}\n\n";

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

		logdata += $"---- RESPONSE ----\n";
		logdata += $"AuthKey {AuthKey}\n";
		logdata += $"IsAES {(inform_resp.IsAES ? "Y" : "N")}\n";
		logdata += $"IsZLIB {(inform_resp.IsZLIB ? "Y" : "N")}\n";
		logdata += $"IsGCM {(inform_resp.IsGCM ? "Y" : "N")}\n";
		logdata += $"IsSnappy {(inform_resp.IsSnappy ? "Y" : "N")}\n";
		logdata += $"MACAddress {inform_resp.MACAddress}\n";
		logdata += $"Version {inform_resp.Version}\n";
		logdata += $"PayloadVersion {inform_resp.PayloadVersion}\n";
		logdata += $"IV {inform_resp.IV.BytesToColon()}\n";
		logdata += $"\n{inform_resp.Body}\n\n";

		File.WriteAllText(logfile, logdata);

		inform_resp.MACAddress = Mac;
		return inform_resp.Encode(AuthKey);
	}

	public void HandleDiscovery(DiscoveryPacket dp) {
		dp.SetPayloadAsMac(DiscoveryPacket.PAYLOAD_MAC, FakeMac);
		dp.SetPayloadAsMacIp(DiscoveryPacket.PAYLOAD_MACIP, FakeMac, FakeIP);
		dp.SetPayloadAsMac(DiscoveryPacket.PAYLOAD_SERIAL, FakeMac);
		dp.SetPayloadAsUShort(DiscoveryPacket.PAYLOAD_SSHPORT, sshService.Port);
		dp.Broadcast();
	}

	public (string response, int exitCode) HandleSSHExec(string cmd, string user, string password) {
		if(cmd.StartsWith("/usr/bin/syswrapper.sh set-adopt")) {
			var args = cmd.Split(' ');
			var newInformUrl = args[2];
			var newAuthKey = args[3];
			if(AuthKey != newAuthKey || InformURL != newInformUrl) {
				AuthKey = newAuthKey;
				InformURL = newInformUrl;
				var cfg = new DeviceCFG {
					AuthKey = newAuthKey,
					InformURL = newInformUrl
				};
				File.WriteAllText(cfgPath, JsonConvert.SerializeObject(cfg));
			}
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

	class DeviceCFG {
		public string AuthKey { get; set; } = null!;
		public string InformURL { get; set; } = null!;
	}
}
