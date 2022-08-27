using System.Text.Json;
using TNBU.GUI.Models;

namespace TNBU.GUI.Services.FirmwareUpdate {
	public class FirmwareManager {
		private const string URL = "https://fw-update.ubnt.com/api/firmware-latest?filter=eq~~product~~unifi-firmware&filter=eq~~channel~~release";
		private readonly HttpClient httpClient = new();
		private readonly Dictionary<string, FirmwareInfo> fwCache = new();

		public async Task UpdateFirmwareCache() {
			var body = await httpClient.GetStringAsync(URL);
			var decoded = JsonSerializer.Deserialize<SrvFirmwareJson>(body);
			if(decoded?._embedded?.firmware == null) {
				return;
			}
			foreach(var fw in decoded._embedded.firmware) {
				fwCache[fw.platform] = new FirmwareInfo(fw);
			}
		}

		public void DeviceNeedsUpdate(Device d) {
			if(d.Model == null || d.Firmware == null || !fwCache.ContainsKey(d.Model)) {
				d.FirmwareUpdate = null;
				return;
			}
			var deviceVersion = new Version(d.Firmware);
			if(fwCache[d.Model].Version <= deviceVersion) {
				d.FirmwareUpdate = null;
				return;
			}
			d.FirmwareUpdate = fwCache[d.Model];
		}
	}
}
