namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgRadio {
		public List<CfgRadioEntry> Entries { get; } = new();
		public override string ToString() {
			return $@"
radio.countrycode=380
radio.outdoor=disabled
radio.status=enabled
{string.Join('\n', Entries.Select((x, i) => x.GetConfig(i + 1)))}
";
		}
	}
}
