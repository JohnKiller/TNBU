using Newtonsoft.Json;
using System.Security.Cryptography;
using TNBU.Core.Utils;

namespace TNBU.MitM.Services;

public class SSHServerFactoryService {
	private readonly ILogger<SSHServerFactoryService> logger;
	private ushort port = 3000;

	public SSHServerFactoryService(ILogger<SSHServerFactoryService> _logger) {
		logger = _logger;
	}

	public DeviceSSHService CreateSSHServer(string cfgpath) {
		SSHCfg cfg;
		if(File.Exists(cfgpath)) {
			cfg = JsonConvert.DeserializeObject<SSHCfg>(File.ReadAllText(cfgpath))!;
		} else {
			cfg = SSHCfg.GenerateNew();
			File.WriteAllText(cfgpath, JsonConvert.SerializeObject(cfg));
		}
		return new DeviceSSHService(logger, port++, cfg.Key, cfg.Hash);
	}

	class SSHCfg {
		public string Key { get; set; } = null!;
		public string Hash { get; set; } = null!;

		public static SSHCfg GenerateNew() {
			var ret = new SSHCfg();
			using var keygen = new SshKeyGenerator.SshKeyGenerator(2048);
			ret.Key = keygen.ToB64Blob(true);
			using var sha = SHA1.Create();
			var b64key = keygen.ToRfcPublicKey().Split(' ')[1];
			ret.Hash = HexConversions.BytesToColon(sha.ComputeHash(Convert.FromBase64String(b64key)));
			return ret;
		}
	}
}
