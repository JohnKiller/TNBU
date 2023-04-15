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
	public StoredCFG Config { get; }
	public string FakeInformUrl => $"http://{FakeIP}:8081/inform";

	private readonly DeviceSSHService sshService;
	private readonly ILogger logger;
	private readonly string cfgPath;
	private readonly string logPath;
	private readonly string mgmtPath;
	private readonly string systemCfgPath;

	private static readonly HttpClient client = new(new SocketsHttpHandler {
		ActivityHeadersPropagator = null
	});
	static DeviceRelay() {
		client.DefaultRequestHeaders.Accept.Clear();
		client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
	}

	public DeviceRelay(PhysicalAddress _mac, IPAddress _ip, DeviceSSHService _sshService, ILogger _logger, string _deviceCfgPath) {
		Mac = _mac;
		FakeMac = NetworkUtils.GetFakeMacAddress(Mac);
		IP = _ip;
		(FakeIP, Netmask) = NetworkUtils.GetMyIPOnThisSubnet(IP);
		sshService = _sshService;
		sshService.Owner = this;
		logger = _logger;

		cfgPath = Path.Combine(_deviceCfgPath, "device.json");
		logPath = Path.Combine(_deviceCfgPath, "logs");
		mgmtPath = Path.Combine(_deviceCfgPath, "mgmt.ini");
		systemCfgPath = Path.Combine(_deviceCfgPath, "system.ini");
		Directory.CreateDirectory(logPath);
		if(File.Exists(cfgPath)) {
			Config = JsonConvert.DeserializeObject<StoredCFG>(File.ReadAllText(cfgPath))!;
		} else {
			Config = new();
		}
	}

	public async Task<byte[]> HandleInform(InformPacket req) {
		string logfile;
		var counter = 0;
		do {
			logfile = Path.Combine(logPath, DateTime.Now.ToString("yyyyMMdd-HHmmss-") + ++counter + ".txt");
		} while(File.Exists(logfile));

		req.Decrypt(Config.AuthKey);

		var logdata = $"---- REQUEST ----\n";
		logdata += $"AuthKey {Config.AuthKey}\n";
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

		req.Body = req.Body.Replace(Config.InformURL, FakeInformUrl);
		req.Body = req.Body.Replace(IP.ToString(), FakeIP.ToString());
		req.Body = req.Body.Replace(Serial, FakeSerial);
		if(fingerprint != null) {
			req.Body = req.Body.Replace(fingerprint.ToString(), sshService.Hash);
		}
		req.Body = req.Body.Replace(HexConversions.BytesToColon(Mac.GetAddressBytes()), HexConversions.BytesToColon(FakeMac.GetAddressBytes()));

		var byteArrayContent = new ByteArrayContent(req.Encode(Config.AuthKey));
		byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-binary");
		var response = await client.PostAsync(Config.InformURL, byteArrayContent);
		response.EnsureSuccessStatusCode();

		var respdata = response.Content.ReadAsStream();
		var rawstream = new MemoryStream();
		respdata.CopyTo(rawstream);
		rawstream.Position = 0;

		var inform_resp = InformPacket.Decode(rawstream);
		inform_resp.Decrypt(Config.AuthKey);

		logdata += $"---- RESPONSE ----\n";
		logdata += $"AuthKey {Config.AuthKey}\n";
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

		var respBody = JsonConvert.DeserializeObject<JObject>(inform_resp.Body)!;
		var hasChanged = false;

		var system_cfg = respBody["system_cfg"];
		if(system_cfg != null) {
			Config.CurrentConfig = (string?)system_cfg;
			hasChanged = true;
		}

		var mgmt_cfg = respBody["mgmt_cfg"];
		if(mgmt_cfg != null) {
			Config.CurrentMgmtConfig = (string?)mgmt_cfg;
			hasChanged = true;
		}

		/* //MESH v3 adoption process:
		 * controller finds isolated APs using other AP scan result
		 * to adopt, it asks the closest AP to connect to it
		 * by issuing set-meshv3-payload with the mac of the device
		 * after that, the new AP gets bridged to br0 and is available on the network
		 * then it's the standard adoption process like if it was connected by wire
		var type = (string?)respBody["_type"];
		switch(type) {
			case "noop":
				break;
			case "setparam":
				Console.WriteLine("setparam");
				break;
			case "cmd":
				var cmd = (string?)respBody["cmd"];
				switch(cmd) {
					case "set-meshv3-payload":
						Console.WriteLine($"set-meshv3-payload {respBody["mac"]}");
						break;
					case "unset-meshv3-payload":
						Console.WriteLine("unset-meshv3-payload");
						break;
					default:
						throw new NotImplementedException();
				}
				break;
			default:
				throw new NotImplementedException();
		}*/

		if(hasChanged) {
			SaveConfig();
		}

		inform_resp.Body = inform_resp.Body.Replace(Config.InformURL, FakeInformUrl);

		inform_resp.MACAddress = Mac;
		return inform_resp.Encode(Config.AuthKey);
	}

	public void HandleDiscovery(DiscoveryPacket dp) {
		dp.SetPayloadAsMac(DiscoveryPacket.PAYLOAD_MAC, FakeMac);
		dp.SetPayloadAsMacIp(DiscoveryPacket.PAYLOAD_MACIP, FakeMac, FakeIP);
		dp.SetPayloadAsMac(DiscoveryPacket.PAYLOAD_SERIAL, FakeMac);
		dp.SetPayloadAsUShort(DiscoveryPacket.PAYLOAD_SSHPORT, sshService.Port);
		dp.Broadcast();
	}

	public bool HandleSSHAuth(string user, string password) {
		try {
			using var client = new Renci.SshNet.SshClient(IP.ToString(), user, password);
			client.Connect();
			return true;
		}catch(Exception ex) {
			logger.LogError("Failed test auth: {reason}", ex.Message);
			return false;
		}
	}

	public (string response, int exitCode) HandleSSHExec(string cmd, string user, string password) {
		if(cmd.StartsWith("/usr/bin/syswrapper.sh set-adopt")) {
			var args = cmd.Split(' ');
			var newInformUrl = args[2];
			var newAuthKey = args[3];
			if(Config.AuthKey != newAuthKey || Config.InformURL != newInformUrl) {
				Config.AuthKey = newAuthKey;
				Config.InformURL = newInformUrl;
				SaveConfig();
			}
			var newcmd = $"/usr/bin/syswrapper.sh set-adopt {FakeInformUrl} {Config.AuthKey}";
			using var client = new Renci.SshNet.SshClient(IP.ToString(), user, password);
			client.Connect();
			var ret = client.RunCommand(newcmd);
			var result = ret.Result;
			logger.LogInformation("Response from real device: \"{result}\" exit code is {exit}", result, ret.ExitStatus);
			return (result, ret.ExitStatus);
		}
		throw new NotImplementedException(cmd);
	}

	private void SaveConfig() {
		File.WriteAllText(cfgPath, JsonConvert.SerializeObject(Config));
		if(Config.CurrentMgmtConfig != null) {
			WriteIni(Config.CurrentMgmtConfig, mgmtPath);
		}
		if(Config.CurrentConfig != null) {
			WriteIni(Config.CurrentConfig, systemCfgPath);
		}
	}

	private static void WriteIni(string data, string path) {
		File.WriteAllLines(path, data.Split('\n').OrderBy(x => x));
	}

	public class StoredCFG {
		public string AuthKey { get; set; } = InformPacket.DEFAULT_KEY;
		public string? InformURL { get; set; }
		public string? CurrentConfig { get; set; }
		public string? CurrentMgmtConfig { get; set; }
	}
}
