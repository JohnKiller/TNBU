namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgNetconfEntry {
		public string DevName { get; set; } = "br0";
		public bool Up { get; set; } = true;
		public bool Promisc { get; set; } = true;

		public string GetConfig(int num) {
			var ret = $@"
netconf.{num}.autoip.status=disabled
netconf.{num}.devname={DevName}
netconf.{num}.ip=0.0.0.0
netconf.{num}.status=enabled
netconf.{num}.up={(Up ? "enabled" : "disabled")}
";
			if(Promisc) {
				ret += $@"
netconf.{num}.promisc=enabled
";
			}
			return ret;
		}
	}
}
