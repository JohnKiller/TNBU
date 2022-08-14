namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgLcm {
		public override string ToString() {
			return $@"
lcm.brightness=80
lcm.idle_timeout=300
lcm.night_mode_begins=22:00
lcm.night_mode_ends=08:00
lcm.settings_restricted_access=false
lcm.status=true
lcm.sync=true
lcm.touch_event=true
";
		}
	}
}
