namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgDhcpc {
		public string DevName { get; set; } = "eth0";
		public override string ToString() {
			return $@"
dhcpc.status=enabled
dhcpc.1.devname={DevName}
dhcpc.1.status=enabled
";
		}
	}
}
