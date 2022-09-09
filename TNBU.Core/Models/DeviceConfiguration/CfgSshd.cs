namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgSshd {
		public string? Interface { get; set; }	
		public override string ToString() {
			if(Interface == null) {
				return $@"
sshd.status=disabled
sshd.1.ifname=lo
sshd.1.status=enabled
";
			}
			return $@"
sshd.status=enabled
sshd.1.ifname={Interface}
sshd.1.status=enabled
";
		}
	}
}
