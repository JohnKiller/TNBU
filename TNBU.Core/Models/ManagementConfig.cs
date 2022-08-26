namespace TNBU.Core.Models {
	public class ManagementConfig {
		public string CfgVersion { get; set; } = "?";
		public bool LedEnabled { get; set; } = true;
		public string? MgmtUrl { get; set; }
		public string? StunUrl { get; set; }
		public string? InformUrl { get; set; }
		public string? AuthKey { get; set; }

		public override string ToString() {
			var ret = $@"
capability=notif,fastapply-bg,notif-assoc-stat
cfgversion={CfgVersion}
led_enabled={(LedEnabled ? "true" : "false")}
report_crash=true
selfrun_guest_mode=pass
use_aes_gcm=true
";
			if(MgmtUrl != null) {
				ret += $@"
mgmt_url={MgmtUrl}
";
			}
			if(StunUrl != null) {
				ret += $@"
stun_url={StunUrl}
";
			}
			if(AuthKey != null) {
				ret += $@"
authkey={AuthKey}
";
			}
			if(InformUrl != null) {
				ret += $@"
inform_url={InformUrl}
";
			}
			return ret.Replace(Environment.NewLine, "\n");
		}
	}
}
