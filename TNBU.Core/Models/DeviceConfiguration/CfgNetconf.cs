namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgNetconf {
		public List<CfgNetconfEntry> Entries { get; } = new();
		public override string ToString() {
			return $@"
netconf.status=enabled
{string.Join('\n', Entries.Select((x, i) => x.GetConfig(i + 1)))}
";
		}
	}
}
