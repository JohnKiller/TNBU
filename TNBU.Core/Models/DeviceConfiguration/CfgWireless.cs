namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgWireless {
		public List<CfgWirelessEntry> Entries { get; } = new();
		public override string ToString() {
			return $@"
wireless.status=enabled
{string.Join('\n', Entries.Select((x, i) => x.GetConfig(i + 1)))}
";
		}
	}
}
