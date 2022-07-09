namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgAaaEntry {
		public string BrDevName { get; set; } = "br0";
		public string DevName { get; set; } = "ath0";
		public string SSID { get; set; } = "MyWifi";
		public string PSK { get; set; } = "0123456789";
		public string ID { get; set; } = "000000000000000000000000";
		public string IAPP { get; set; } = "00000000000000000000000000000000";

		public string GetConfig(int num) {
			return $@"
aaa.{num}.11k.status=disabled
aaa.{num}.br.devname={BrDevName}
aaa.{num}.bss_transition=enabled
aaa.{num}.country_beacon=disabled
aaa.{num}.devname={DevName}
aaa.{num}.driver=madwifi
aaa.{num}.eapol_version=2
aaa.{num}.ft.status=disabled
aaa.{num}.hide_ssid=false
aaa.{num}.iapp_key={IAPP}
aaa.{num}.id={ID}
aaa.{num}.is_guest=false
aaa.{num}.p2p_cross_connect=disabled
aaa.{num}.p2p=disabled
aaa.{num}.pmf.cipher=AES-128-CMAC
aaa.{num}.pmf.mode=0
aaa.{num}.pmf.status=disabled
aaa.{num}.proxy_arp=disabled
aaa.{num}.radius.macacl.status=disabled
aaa.{num}.ssid={SSID}
aaa.{num}.status=enabled
aaa.{num}.tdls_prohibit=disabled
aaa.{num}.verbose=2
aaa.{num}.wpa.1.pairwise=CCMP
aaa.{num}.wpa.group_rekey=3600
aaa.{num}.wpa.key.1.mgmt=WPA-PSK
aaa.{num}.wpa.psk={PSK}
aaa.{num}.wpa=2
";
		}
	}
}
