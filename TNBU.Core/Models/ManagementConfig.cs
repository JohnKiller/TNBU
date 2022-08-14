namespace TNBU.Core.Models {
	public class ManagementConfig {
		public string CfgVersion { get; set; } = "?";
		public bool LedEnabled { get; set; } = true;

		public override string ToString() {
			/*
				mgmt_url=
				stun_url=
			*/
			return $@"
capability=notif,fastapply-bg,notif-assoc-stat
cfgversion={CfgVersion}
led_enabled={(LedEnabled ? "true" : "false")}
report_crash=true
selfrun_guest_mode=pass
use_aes_gcm=true
".Replace(Environment.NewLine, "\n");
		}
	}
}
