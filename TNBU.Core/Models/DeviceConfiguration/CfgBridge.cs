namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgBridge {
		public List<CfgBridgeEntry> Entries { get; } = new();
		public override string ToString() {
			if(Entries.Count == 0) {
				return @"
bridge.status=disabled
";
			}
			return $@"
bridge.status=enabled
{string.Join('\n', Entries.Select((x, i) => x.GetConfig(i + 1)))}
";
		}
	}
}
