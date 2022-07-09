namespace TNBU.Core.Models {
	public class ManagementConfig {
		public string CfgVersion { get; set; } = "?";
		public bool LedEnabled { get; set; } = true;

		public override string ToString() {
			return $@"
capability=notif,notif-assoc-stat
cfgversion={CfgVersion}
led_enabled={(LedEnabled ? "true" : "false")}
mgmt_url=
report_crash=true
selfrun_guest_mode=pass
stun_url=
use_aes_gcm=true
";
		}
	}
}
