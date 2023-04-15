namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgSwitchVLAN {
		public int ID { get; set; }
		public Dictionary<int, bool> Ports { get; } = new();
		public string GetConfig(int num) {
			var ret = $@"
switch.vlan.{num}.id={ID}
switch.vlan.{num}.status=enabled
switch.vlan.{num}.mode={(ID == 1 ? "untagged" : "tagged")}
{string.Join('\n', Ports.Select(x => $"switch.vlan.{num}.port.{x.Key}.mode=" + (x.Value ? "untagged" : "exclude")))}
";
			return ret;
		}
	}
}
