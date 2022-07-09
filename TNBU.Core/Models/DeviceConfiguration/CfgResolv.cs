namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgResolv {
		public override string ToString() {
			return $@"
resolv.status=enabled
resolv.nameserver.1.status=disabled
resolv.nameserver.2.status=disabled
";
		}
	}
}
