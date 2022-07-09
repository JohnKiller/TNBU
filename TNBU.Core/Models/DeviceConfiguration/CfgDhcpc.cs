namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgDhcpc {
		public override string ToString() {
			return $@"
dhcpc.status=enabled
dhcpc.1.devname=br0
dhcpc.1.status=enabled
";
		}
	}
}
