namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgResolv {
		public string? HostName { get; set; }
		public override string ToString() {
			var ret = $@"
resolv.status=enabled
resolv.nameserver.1.status=disabled
resolv.nameserver.2.status=disabled
";
			if(HostName != null) {
				ret += $@"
resolv.host.1.name={HostName}
";
			}
			return ret;
		}
	}
}
