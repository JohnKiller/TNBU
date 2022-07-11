namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgAaa {
		public List<CfgAaaEntry> Entries { get; } = new();
		public override string ToString() {
			return $@"
aaa.status=enabled
{string.Join('\n', Entries.Select((x, i) => x.GetConfig(i + 1)))}
";
		}
	}
}
