using System.Net.NetworkInformation;
using TNBU.Core.Models;
using TNBU.GUI.Models;

namespace TNBU.GUI.Services {
	public class DeviceManagerService {
		public Dictionary<PhysicalAddress, Device> Devices { get; } = new();
		public event EventHandler? OnDeviceChange;

		public void GotDiscovery(DiscoveryPacket dp) {
			var (mac, ip) = dp.GetPayloadAsMacIp(DiscoveryPacket.PAYLOAD_MACIP);
			if(!Devices.ContainsKey(mac)) {
				Devices.Add(mac, new() {
					Mac = mac,
					IsAssociated = false,
				});
			}
			var device = Devices[mac];
			device.IsConnected = true;
			device.IsDefault = dp.IsDefault;
			device.IP = ip;
			device.Model = dp.Model;
			device.HostName = dp.HostName;
			OnDeviceChange?.Invoke(device, EventArgs.Empty);
		}

		public async Task Adopt(Device device) {
			await Task.Delay(2000);
		}
	}
}
