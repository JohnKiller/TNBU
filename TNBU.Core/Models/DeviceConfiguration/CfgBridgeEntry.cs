namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgBridgeEntry {
		public string DevName { get; set; } = "br0";
		public List<string> Ports { get; } = new() { "eth0" };

		public string GetConfig(int num) {
			return $@"
bridge.{num}.devname={DevName}
bridge.{num}.fd=1
bridge.{num}.stp.status=disabled
{string.Join('\n', Ports.Select((x, i) => $"bridge.{num}.port.{i + 1}.devname={x}"))}";
		}
	}
}
