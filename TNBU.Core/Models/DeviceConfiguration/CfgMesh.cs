namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgMesh {
		public bool Enabled { get; set; } = false;
		public string SSID { get; set; } = "vwire-0123456789abcdef";
		public string PSK { get; set; } = "abababababababababababababababab";
		public override string ToString() {
			if(!Enabled) {
				return $@"
mesh.status=disabled
";
			}
			return $@"
mesh.essid={SSID}
mesh.psk={PSK}
mesh.status=enabled
mesh.version=3
";
		}
	}
}
