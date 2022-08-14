namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgRps {
		public override string ToString() {
			return $@"
rps.port.1.mode=auto
rps.port.1.name=Port 1
rps.power_management_mode=dynamic
rps.status=enabled
";
		}
	}
}
