namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgWirelessEntry {
		public string DevName { get; set; } = "ath0";
		public string PhyName { get; set; } = "wifi0";
		public string? ID { get; set; } = "000000000000000000000000";
		public string SSID { get; set; } = "MyWifi";
		public bool BgaFilter { get; set; } = true;
		public bool HideSSID { get; set; } = false;
		public string Mode { get; set; } = "master";
		public string Usage { get; set; } = "user";
		public bool VPort { get; set; } = false;
		public bool VWire { get; set; } = false;
		public bool WDS { get; set; } = false;

		public string GetConfig(int num) {
			var ret = $@"
wireless.{num}.addmtikie=disabled
wireless.{num}.authmode=1
wireless.{num}.autowds=disabled
wireless.{num}.bga_filter={(BgaFilter ? "enabled" : "disabled")}
wireless.{num}.devname={DevName}
wireless.{num}.element_adopt=disabled
wireless.{num}.is_guest=false
wireless.{num}.l2_isolation=disabled
wireless.{num}.mac_acl.policy=deny
wireless.{num}.mac_acl.status=enabled
wireless.{num}.mcast.enhance=0
wireless.{num}.mcastrate=auto
wireless.{num}.mode={Mode}
wireless.{num}.no2ghz_oui=disabled
wireless.{num}.parent={PhyName}
wireless.{num}.pureg=1
wireless.{num}.puren=0
wireless.{num}.schedule_enabled=disabled
wireless.{num}.security=none
wireless.{num}.ssid={SSID}
wireless.{num}.status=enabled
wireless.{num}.uapsd=disabled
wireless.{num}.usage={Usage}
wireless.{num}.vport={(VPort ? "enabled" : "disabled")}
wireless.{num}.vwire={(VWire ? "enabled" : "disabled")}
wireless.{num}.wds={(WDS ? "enabled" : "disabled")}
wireless.{num}.wmm=enabled
";
			if(HideSSID) {
				ret += $@"
wireless.{num}.hide_ssid=true
";
			} else {
				ret += $@"
wireless.{num}.hide_ssid=false
wireless.{num}.dtim_period=3
";
			}
			if(ID != null) {
				ret += $@"
wireless.{num}.id={ID}
";
			}
			return ret;
		}
	}
}
