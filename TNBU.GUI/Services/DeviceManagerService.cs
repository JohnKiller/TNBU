using Renci.SshNet;
using System.Net;
using System.Net.NetworkInformation;
using TNBU.Core.Models;
using TNBU.Core.Utils;
using TNBU.GUI.Models;

namespace TNBU.GUI.Services {
	public class DeviceManagerService {
		public Dictionary<PhysicalAddress, Device> Devices { get; } = new();
		public event EventHandler? OnDeviceChange;

		private readonly ILogger<DiscoveryService> logger;

		public DeviceManagerService(ILogger<DiscoveryService> _logger) {
			logger = _logger;
		}

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

		public async Task<byte[]?> GotInform(InformPacket req, IPAddress ip) {
			try {
				req.Decrypt();
			}catch(Exception ex) {
				logger.LogError("Error decoding inform packet: {message}", ex.Message);
				return null;
			}
			var mac = req.MACAddress;
			if(!Devices.ContainsKey(mac)) {
				Devices.Add(mac, new() {
					Mac = mac,
				});
			}
			var device = Devices[mac];
			device.IsConnected = true;
			device.IsAssociated = true;
			device.IP = ip;
			OnDeviceChange?.Invoke(device, EventArgs.Empty);
			return null;
		}

		public async Task Adopt(Device device) {
			if(device.IP == null) {
				throw new Exception("Requested to adopt a device without an IP");
			}
			logger.LogInformation("Starting adoption task");
			var (ourIp, _) = NetworkUtils.GetMyIPOnThisSubnet(device.IP);
			var newcmd = $"/usr/bin/syswrapper.sh set-adopt http://{ourIp}:8081/inform {InformPacket.DEFAULT_KEY}";
			await Task.Run(() => {
				try {
					using var client = new SshClient(device.IP.ToString(), "ubnt", "ubnt");
					client.Connect();
					var ret = client.RunCommand(newcmd);
					var result = ret.Result;
					logger.LogInformation("Response from device: \"{result}\" exit code is {exit}", result, ret.ExitStatus);
					if(ret.ExitStatus != 0) {
						throw new Exception("Wrong response from device: " + result);
					}
				} catch(Exception ex) {
					logger.LogError("Device ssh adoption error: {message}", ex.Message);
					throw;
				}
			});
		}
	}
}
