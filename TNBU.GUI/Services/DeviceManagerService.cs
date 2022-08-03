using Renci.SshNet;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.Json;
using TNBU.Core.Models;
using TNBU.Core.Models.Inform;
using TNBU.Core.Utils;
using TNBU.GUI.Models;
using TNBU.GUI.Services.ConfigurationBuilder;

namespace TNBU.GUI.Services {
	public class DeviceManagerService {
		public Dictionary<PhysicalAddress, Device> Devices { get; } = new();
		public event EventHandler? OnDeviceChange;

		private readonly ILogger<DiscoveryService> logger;
		private readonly ConfigurationBuilderService configurationBuilder;

		public DeviceManagerService(ILogger<DiscoveryService> _logger, ConfigurationBuilderService _configurationBuilder) {
			logger = _logger;
			configurationBuilder = _configurationBuilder;
		}

		public void GotDiscovery(DiscoveryPacket dp) {
			var (mac, ip) = dp.GetPayloadAsMacIp(DiscoveryPacket.PAYLOAD_MACIP);
			if(!Devices.ContainsKey(mac)) {
				Devices.Add(mac, new() {
					Mac = mac,
					IsAdopted = false,
				});
			}
			var device = Devices[mac];
			device.IsConnected = true;
			device.IsDefault = dp.IsDefault;
			if(device.IsDefault) {
				device.IsAdopted = false;
			}
			device.IP = ip;
			device.Model = dp.Model;
			device.HostName = dp.HostName;
			device.Firmware = dp.FirmwareVersion;
			OnDeviceChange?.Invoke(device, EventArgs.Empty);
		}

		public async Task<byte[]?> GotInform(InformPacket req, IPAddress ip) {
			try {
				req.Decrypt();
			} catch(Exception ex) {
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
			device.IsAdopted = true;
			device.IP = ip;

			var request = JsonSerializer.Deserialize<BaseInformBody>(req.Body);
			if(request == null) {
				logger.LogError("Failed deserializing inform payload body to json");
				return null;
			}

			device.HostName = request.hostname;
			device.Model = request.model;
			device.ModelDisplay = request.model_display;
			device.Firmware = request.version;

			configurationBuilder.UpdateDeviceConfiguration(device);

			var inform_resp = new InformPacket {
				IsAES = true,
				IsGCM = true,
				IsSnappy = false,
				IsZLIB = false,
				IV = InformPacket.GenerateIV(),
				MACAddress = mac,
				Version = 0,
				PayloadVersion = 1
			};

			dynamic body;
			if(request.inform_as_notif) {
				logger.LogWarning("Received notif from {mac}: {message}", mac, request.notif_reason);
				body = InformResponse.Immediate();
			} else if(request.cfgversion == "?") {
				logger.LogWarning("Received adopt inform from {mac}", mac);
				inform_resp.IsGCM = false;
				body = InformResponse.SetAdopt();
				device.IsAdopting = true;
			} else if(request.cfgversion == InformResponse.ADOPT_CFG) {
				logger.LogWarning("Received adopt confirmation from {mac}", mac);
				body = InformResponse.SetConfig(device.ManagementConfig, device.SystemConfig);
				device.IsAdopting = false;
				device.IsConfiguring = true;
			} else if(request.cfgversion != device.CfgVersion) {
				logger.LogError("Wrong cfgversion {mac}", mac);
				body = InformResponse.SetConfig(device.ManagementConfig, device.SystemConfig);
				device.IsConfiguring = true;
			} else if(device.ResetRequested) {
				logger.LogError("Reset {mac}", mac);
				body = InformResponse.Reset();
				Devices.Remove(mac);
			} else {
				var interval = Math.Max(5, request.inform_min_interval);
				logger.LogInformation("Noop {mac} interval {interval}", mac, interval);
				body = InformResponse.Noop(interval);
				device.IsAdopting = false;
				device.IsConfiguring = false;
			}

			inform_resp.Body = JsonSerializer.Serialize(body);

			OnDeviceChange?.Invoke(device, EventArgs.Empty);
			return inform_resp.Encode();
		}

		public async Task Adopt(Device device) {
			if(device.IP == null) {
				throw new Exception("Requested to adopt a device without an IP");
			}
			if(!configurationBuilder.IsDeviceSupported(device)) {
				throw new Exception("This device is not currently supported!");
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
