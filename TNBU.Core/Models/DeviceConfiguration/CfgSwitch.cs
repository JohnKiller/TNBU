namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgSwitch {
		public long StpPriority { get; set; } = 32768;
		public List<CfgSwitchEntry> Entries { get; } = new();
		public override string ToString() {
			if(Entries.Count == 0) {
				return $@"
switch.status=disabled
";
			}
			return $@"
switch.dhcp_snoop.status=enabled
switch.dot1x.status=disabled
switch.jumboframes=disabled
switch.managementvlan=1
switch.mtu=9216
switch.power_source.status=disabled
switch.routing_enabled=false
switch.stp.priority={StpPriority}
switch.stp.status=enabled
switch.stp.version=rstp
{string.Join('\n', Entries.Select((x, i) => x.GetConfig(i + 1)))}
";
		}
	}
}
