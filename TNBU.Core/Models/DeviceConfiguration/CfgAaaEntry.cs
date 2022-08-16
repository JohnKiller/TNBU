namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgAaaEntry {
		public bool Enabled { get; set; } = true;
		public string BrDevName { get; set; } = "br0";
		public string DevName { get; set; } = "ath0";
		public string SSID { get; set; } = "MyWifi";
		public bool HideSSID { get; set; } = false;
		public string PSK { get; set; } = "0123456789";
		public string? ID { get; set; }
		public string? IAPP { get; set; }

		public string GetConfig(int num) {
			var ret = $@"
aaa.{num}.11k.status=disabled
aaa.{num}.br.devname={BrDevName}
aaa.{num}.country_beacon=disabled
aaa.{num}.devname={DevName}
aaa.{num}.driver=madwifi
aaa.{num}.ft.status=disabled
aaa.{num}.hide_ssid={(HideSSID ? "true" : "false")}
aaa.{num}.pmf.mode=0
aaa.{num}.pmf.status=disabled
aaa.{num}.radius.macacl.status=disabled
aaa.{num}.ssid={SSID}
aaa.{num}.status={(Enabled ? "enabled" : "disabled")}
";
			if(Enabled) {
				ret += $@"
aaa.{num}.bss_transition=enabled
aaa.{num}.eapol_version=2
aaa.{num}.is_guest=false
aaa.{num}.p2p_cross_connect=disabled
aaa.{num}.p2p=disabled
aaa.{num}.pmf.cipher=AES-128-CMAC
aaa.{num}.proxy_arp=disabled
aaa.{num}.tdls_prohibit=disabled
aaa.{num}.verbose=2
aaa.{num}.wpa.1.pairwise=CCMP
aaa.{num}.wpa.group_rekey=3600
aaa.{num}.wpa.key.1.mgmt=WPA-PSK
aaa.{num}.wpa.psk={PSK}
aaa.{num}.wpa=2
";
			}
			if(IAPP != null) {
				ret += $@"
aaa.{num}.iapp_key={IAPP}
";
			}
			if(ID != null) {
				ret += $@"
aaa.{num}.id={ID}
";
			}
			return ret;
		}
	}
}
