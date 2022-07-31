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
					IP = ip,
					IsAssociated = false,
					Model = dp.Model,
				});
			}
			var device = Devices[mac];
			device.IsConnected = true;
			var isDefault = true;
			try {
				isDefault = dp.GetPayloadAsBoolean(DiscoveryPacket.PAYLOAD_IS_DEFAULT);
			} catch { }
			device.IsDefault = isDefault;
			OnDeviceChange?.Invoke(device, EventArgs.Empty);
		}
	}
}
