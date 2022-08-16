namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgConnectivity {
		public bool Enabled { get; set; }	= false;
		public string Bridge { get; set; } = "br0";
		public string ETH { get; set; } = "eth0";
		public string WDS { get; set; } = "ath1";
		public string UplinkIP { get; set; } = "10.0.0.4";

		public override string ToString() {
			if(!Enabled) {
				return $@"
connectivity.status=disabled
";
			}
			return $@"
connectivity.status=enabled
connectivity.uplink_bridge={Bridge}
connectivity.uplink_eth={ETH}
connectivity.uplink_wds={WDS}
connectivity.uplink={UplinkIP}
";
		}
	}
}
