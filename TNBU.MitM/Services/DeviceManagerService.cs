using System.Net;
using System.Net.NetworkInformation;
using TNBU.Core.Models;
using TNBU.MitM.Models;

namespace TNBU.MitM.Services;

public class DeviceManagerService {
	private const string DEVICES_PATH = "discovered_devices";
	private readonly ILogger<DeviceManagerService> logger;
	private readonly SSHServerFactoryService sshFactory;
	public Dictionary<PhysicalAddress, DeviceRelay> Devices { get; } = new();

	public DeviceManagerService(ILogger<DeviceManagerService> _loggger, SSHServerFactoryService _sshFactory) {
		logger = _loggger;
		sshFactory = _sshFactory;
		Directory.CreateDirectory(DEVICES_PATH);
	}

	public void GotDiscovery(DiscoveryPacket dp) {
		var mac = dp.Mac;
		if(!Devices.ContainsKey(mac)) {
			LoadDevice(mac, dp.IP);
		}
		Devices[mac].HandleDiscovery(dp);
	}

	public Task<byte[]> GotInform(InformPacket req, IPAddress ip) {
		var mac = req.MACAddress;
		if(!Devices.ContainsKey(mac)) {
			LoadDevice(mac, ip);
		}
		return Devices[mac].HandleInform(req);
	}

	private void LoadDevice(PhysicalAddress mac, IPAddress ip) {
		logger.LogInformation("New device detected: {ip} {mac}", ip, mac);
		var deviceCfgPath = Path.Combine(DEVICES_PATH, mac.ToString());
		var sshcfg = Path.Combine(deviceCfgPath, "ssh.json");
		Directory.CreateDirectory(deviceCfgPath);
		Devices.Add(mac, new DeviceRelay(mac, ip, sshFactory.CreateSSHServer(sshcfg), logger, deviceCfgPath));
	}
}
