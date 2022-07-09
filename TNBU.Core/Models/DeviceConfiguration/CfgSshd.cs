namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgSshd {
		public override string ToString() {
			return $@"
sshd.status=disabled
sshd.1.ifname=lo
sshd.1.status=enabled
";
		}
	}
}
