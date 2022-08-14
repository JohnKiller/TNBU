namespace TNBU.GUI.Services.FirmwareUpdate {
	public class FirmwareInfo {
		public string MD5 { get; }
		public Version Version { get; }
		public string URL { get; }
		public string DeviceVersion { get; }

		public FirmwareInfo(SrvFirmwareJson.Firmware fw) {
			MD5 = fw.md5;
			Version = new Version(fw.version_major, fw.version_minor, fw.version_patch);
			URL = fw._links.data.href;
			DeviceVersion = $"{fw.version_major}.{fw.version_minor}.{fw.version_patch}.{fw.version_build}";
		}
	}
}
