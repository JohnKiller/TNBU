namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgSwitchEntry {
		public bool Enabled { get; set; } = true;
		public bool PoeEnabled { get; set; } = true;
		public bool StpEnabled { get; set; } = true;
		public int? LAG { get; set; }
		public string GetConfig(int num) {
			var OpMode = "switch";
			if(LAG.HasValue) {
				OpMode = "aggregate";
			}
			var ret = $@"
switch.port.{num}.keepalive=disabled
switch.port.{num}.lldpmed.opmode=enabled
switch.port.{num}.lldpmed.topology_notify=disabled
switch.port.{num}.name=Port {num}
switch.port.{num}.opmode={OpMode}
switch.port.{num}.poe={(PoeEnabled ? "auto" : "shutdown")}
switch.port.{num}.port-security=disabled
switch.port.{num}.stp.port_mode={(StpEnabled ? "enabled" : "disabled")}
";
			if(!Enabled) {
				ret += $@"
switch.port.{num}.status=disabled
";
			}
			if(LAG.HasValue) {
				ret += $@"
switch.port.{num}.lag={LAG.Value}
";
			}
			return ret;
		}
	}
}
