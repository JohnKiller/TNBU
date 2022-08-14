namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgSwitchEntry {
		public bool PoeEnabled { get; set; } = true;
		public string GetConfig(int num) {
			return $@"
switch.port.{num}.keepalive=disabled
switch.port.{num}.lldpmed.opmode=enabled
switch.port.{num}.lldpmed.topology_notify=disabled
switch.port.{num}.name=Port {num}
switch.port.{num}.opmode=switch
switch.port.{num}.poe={(PoeEnabled?"auto":"shutdown")}
switch.port.{num}.port-security=disabled
switch.port.{num}.stp.port_mode=enabled
";
		}
	}
}
