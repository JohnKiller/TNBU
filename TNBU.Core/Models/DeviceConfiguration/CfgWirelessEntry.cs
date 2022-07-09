namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgWirelessEntry {
		public string DevName { get; set; } = "ath0";
		public string PhyName { get; set; } = "wifi0";
		public string ID { get; set; } = "000000000000000000000000";
		public string SSID { get; set; } = "MyWifi";

		public string GetConfig(int num) {
			return $@"
wireless.{num}.addmtikie=disabled
wireless.{num}.authmode=1
wireless.{num}.autowds=disabled
wireless.{num}.bga_filter=enabled
wireless.{num}.devname={DevName}
wireless.{num}.dtim_period=3
wireless.{num}.element_adopt=disabled
wireless.{num}.hide_ssid=false
wireless.{num}.id={ID}
wireless.{num}.is_guest=false
wireless.{num}.l2_isolation=disabled
wireless.{num}.mac_acl.policy=deny
wireless.{num}.mac_acl.status=enabled
wireless.{num}.mcast.enhance=0
wireless.{num}.mcastrate=auto
wireless.{num}.mode=master
wireless.{num}.no2ghz_oui=disabled
wireless.{num}.parent={PhyName}
wireless.{num}.pureg=1
wireless.{num}.puren=0
wireless.{num}.schedule_enabled=disabled
wireless.{num}.security=none
wireless.{num}.ssid={SSID}
wireless.{num}.status=enabled
wireless.{num}.uapsd=disabled
wireless.{num}.usage=user
wireless.{num}.vport=disabled
wireless.{num}.vwire=disabled
wireless.{num}.wds=disabled
wireless.{num}.wmm=enabled
";
		}
	}
}
