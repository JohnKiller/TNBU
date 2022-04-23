using System.Net.NetworkInformation;
using TNBU.Core.Models;
using TNBU.MitM.Models;

namespace TNBU.MitM.Services;

public class DeviceManagerService {
	private readonly ILogger<DeviceManagerService> logger;
	private readonly SSHServerFactoryService sshFactory;
	public Dictionary<PhysicalAddress, DeviceRelay> Devices { get; } = new();

	public DeviceManagerService(ILogger<DeviceManagerService> _loggger, SSHServerFactoryService _sshFactory) {
		logger = _loggger;
		sshFactory = _sshFactory;
	}

	public void GotDiscovery(DiscoveryPacket dp) {
		var mac = dp.Mac;
		if(!Devices.ContainsKey(mac)) {
			logger.LogInformation("New device detected: {model} {mac}", dp.Model, dp.Mac);
			Devices.Add(mac, new DeviceRelay(dp, sshFactory.CreateSSHServer(), logger));
		}
		Devices[mac].HandleDiscovery(dp);
	}

	public Task<byte[]> GotInform(InformPacket req) {
		var mac = req.MACAddress;
		if(!Devices.ContainsKey(mac)) {
			throw new Exception("Got inform request for unknown device");
		}
		return Devices[mac].HandleInform(req);
	}
}
