using System.Net.NetworkInformation;
using TNBU.Core.Models;
using TNBU.MitM.Models;

namespace TNBU.MitM.Services;

public class DeviceManagerService {
	private readonly ILogger<DeviceManagerService> logger;
	public Dictionary<PhysicalAddress, DeviceRelay> Devices { get; } = new();

	public DeviceManagerService(ILogger<DeviceManagerService> _loggger) {
		logger = _loggger;
	}

	public void GotDiscovery(DiscoveryPacket dp) {
		var mac = dp.Mac;
		if(!Devices.ContainsKey(mac)) {
			Devices[mac] = new DeviceRelay(dp);
			logger.LogInformation("New device detected: {model} {mac}", dp.Model, dp.Mac);
		}
		Devices[mac].ReplayDiscovery(dp);
	}
}
