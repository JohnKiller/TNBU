namespace TNBU.Core.Models.Inform {
	public static class InformResponse {
		public const string ADOPT_CFG = "0123456789abcdef";
		public static dynamic SetAdopt() {
			return new {
				_type = "setparam",
				mgmt_cfg = new ManagementConfig {
					CfgVersion = ADOPT_CFG
				}.ToString(),
				server_time_in_utc = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
			};
		}

		public static dynamic SetConfig(ManagementConfig mgmt_cfg, SystemConfig system_cfg) {
			return new {
				_type = "setparam",
				cfgversion = mgmt_cfg.CfgVersion,
				system_cfg = system_cfg.ToString(),
				mgmt_cfg = mgmt_cfg.ToString(),
				server_time_in_utc = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
			};
		}

		public static dynamic Immediate() {
			return new {
				_type = "noop",
				immediate = 1,
				server_time_in_utc = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
			};
		}

		public static dynamic Noop(int delay) {
			return new {
				_type = "noop",
				interval = delay,
				server_time_in_utc = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
			};
		}

		public static dynamic Reset() {
			return new {
				_type = "setdefault",
				server_time_in_utc = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
			};
		}

		public static dynamic Upgrade(string version, string md5sum, string url) {
			return new {
				_type = "upgrade",
				version,
				md5sum,
				url,
				server_time_in_utc = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
			};
		}

		public static dynamic SetLocate() {
			return new {
				_type = "cmd",
				cmd = "set-locate",
				server_time_in_utc = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
			};
		}

		public static dynamic UnsetLocate() {
			return new {
				_type = "cmd",
				cmd = "unset-locate",
				server_time_in_utc = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
			};
		}

		public static dynamic Reboot() {
			return new {
				_type = "reboot",
				server_time_in_utc = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
			};
		}

		public static dynamic HardReboot() {
			return new {
				_type = "reboot",
				reboot_type = "hard",
				server_time_in_utc = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
			};
		}

		public static dynamic PortPowerCycle(int port) {
			return new {
				_type = "cmd",
				cmd = "power-cycle",
				port,
				server_time_in_utc = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
			};
		}
	}
}
