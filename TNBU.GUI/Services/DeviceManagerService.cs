using Renci.SshNet;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text.Json;
using TNBU.Core.Models;
using TNBU.Core.Models.Inform;
using TNBU.Core.Utils;
using TNBU.GUI.Models;
using TNBU.GUI.Services.ConfigurationBuilder;
using TNBU.GUI.Services.FirmwareUpdate;

namespace TNBU.GUI.Services {
	public class DeviceManagerService {
		private const string MASTER_KEY_PATH = "masterkey.txt";
		private readonly byte[] MASTER_KEY;

		public IReadOnlyCollection<Device> Devices => devices.Values;
		private readonly Dictionary<PhysicalAddress, Device> devices = new();
		public event EventHandler? OnDeviceChange;

		private readonly ILogger<DiscoveryService> logger;
		private readonly ConfigurationBuilderService configurationBuilder;
		private readonly FirmwareManager fwManager = new();

		private readonly Thread bgThread;

		private async void BackgroundThread() {
			while(true) {
				await Task.Delay(TimeSpan.FromSeconds(15));
				foreach(var d in Devices.Where(x => !x.IsOnline && !x.IsAdopted && x.IsDefault).ToList()) {
					lock(devices) {
						devices.Remove(d.Mac);
					}
					OnDeviceChange?.Invoke(d, EventArgs.Empty);
				}
				foreach(var d in Devices.Where(x => !x.IsOnline && (x.Inform != null || x.IP != null))) {
					d.Inform = null;
					d.IP = null;
					OnDeviceChange?.Invoke(d, EventArgs.Empty);
				}
			}
		}

		private string GetKey(PhysicalAddress mac) {
			using var hmac = new HMACMD5(MASTER_KEY);
			var bhmac = hmac.ComputeHash(mac.GetAddressBytes());
			return Convert.ToHexString(bhmac).ToLower();
		}

		public DeviceManagerService(ILogger<DiscoveryService> _logger, ConfigurationBuilderService _configurationBuilder) {
			logger = _logger;
			configurationBuilder = _configurationBuilder;

			if(File.Exists(MASTER_KEY_PATH)) {
				MASTER_KEY = Convert.FromHexString(File.ReadAllText(MASTER_KEY_PATH));
			} else {
				MASTER_KEY = RandomNumberGenerator.GetBytes(32);
				File.WriteAllText(MASTER_KEY_PATH, Convert.ToHexString(MASTER_KEY));
				logger.LogWarning("Generated new master key!");
			}

			bgThread = new(BackgroundThread) {
				IsBackground = true
			};
			bgThread.Start();
		}

		public void GotDiscovery(DiscoveryPacket dp) {
			var (mac, ip) = dp.GetPayloadAsMacIp(DiscoveryPacket.PAYLOAD_MACIP);
			lock(devices) {
				if(!devices.ContainsKey(mac)) {
					devices.Add(mac, new(GetKey(mac), mac) {
						IsAdopted = false,
					});
				}
			}
			var device = devices[mac];
			device.OnlinePing(false);
			if(!dp.IsProbe) {
				device.IsDefault = dp.IsDefault;
				if(device.IsDefault) {
					device.IsAdopted = false;
				}
			}
			device.IP = ip;
			device.Model = dp.Model;
			if(device.ModelDisplay == null) {
				device.ModelDisplay = dp.Model;
			}
			device.HostName = dp.HostName;
			var fw = dp.FirmwareVersion;
			if(fw != null) {
				device.Firmware = fw;
				fwManager.DeviceNeedsUpdate(device);
			}
			OnDeviceChange?.Invoke(device, EventArgs.Empty);
		}

		public async Task<byte[]?> GotInform(InformPacket req, IPAddress ip) {
			var mac = req.MACAddress;
			var deviceKey = InformPacket.DEFAULT_KEY;
			lock(devices) {
				try {
					if(devices.ContainsKey(mac)) {
						deviceKey = devices[mac].Key;
					} else {
						deviceKey = GetKey(mac);
					}
					try {
						req.Decrypt(deviceKey);
					} catch(Exception) {
						logger.LogWarning("Error decoding inform packet from known device: {mac}", mac);
						deviceKey = InformPacket.DEFAULT_KEY;
						req.Decrypt(); //retry with default key
					}
				} catch(Exception ex) {
					logger.LogError("Error decoding inform packet: {message}", ex.Message);
					return null;
				}
				if(!devices.ContainsKey(mac)) {
					devices.Add(mac, new(GetKey(mac), mac) {

					});
				}
			}
			var device = devices[mac];
			device.OnlinePing(true);
			device.IsAdopted = true; //TODO: check if serial already adopted (manual ssh set-inform)
			device.IP = ip;

			var request = JsonSerializer.Deserialize<BaseInformBody>(req.Body);
			if(request == null) {
				logger.LogError("Failed deserializing inform payload body to json");
				return null;
			}
			device.IsDefault = request.@default;
			device.Isolated = request.isolated;
			device.HostName = request.hostname;
			device.Model = request.model;
			device.ModelDisplay = request.model_display;
			device.Firmware = request.version;
			fwManager.DeviceNeedsUpdate(device);

			ExtendedInformBody? extendedInformBody = null;
			try {
				extendedInformBody = JsonSerializer.Deserialize<ExtendedInformBody>(req.Body);
			} catch(Exception) {
				logger.LogError("Failed deserializing inform payload body to json");
				if(device.FirmwareUpdate == null) {
					throw;
				}
			}
			if(extendedInformBody != null) {
				if(!request.inform_as_notif) {
					device.Inform = extendedInformBody;
				}
				device.PhysicalRadios.Clear();
				if(extendedInformBody.radio_table != null) {
					foreach(var r in extendedInformBody.radio_table) {
						device.PhysicalRadios.Add(new(r.name, r.is_11ac));
						if(r.scan_table != null) {
							foreach(var client in r.scan_table) {
								if(client.is_vport) {
									var clientmac = PhysicalAddress.Parse(client.serialno);
									lock(devices) {
										if(!devices.ContainsKey(clientmac)) {
											devices.Add(clientmac, new(GetKey(clientmac), clientmac) {

											});
										}
									}
									var clientdev = devices[clientmac];
									if(!clientdev.IsInformValid) {
										clientdev.IsDefault = client.is_default;
										clientdev.Model = client.model;
										clientdev.ModelDisplay = client.model_display;
										clientdev.Firmware = client.fw_version;
										clientdev.Isolated = client.is_isolated;
										clientdev.OnlinePing(false);
									}
								}
							}
						}
					}
				}
				device.PhysicalSwitchPorts.Clear();
				if(extendedInformBody.port_table != null) {
					foreach(var p in extendedInformBody.port_table) {
						device.PhysicalSwitchPorts.Add(new(p.port_idx));
					}
				}
			}

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
				logger.LogWarning("Received notif from {mac}: {message} {payload}", mac, request.notif_reason, request.notif_payload);
				body = InformResponse.Immediate();
			} else if(request.cfgversion == "?" || deviceKey != device.Key) {
				logger.LogWarning("Received adopt inform from {mac}", mac);
				inform_resp.IsGCM = false;
				body = InformResponse.SetAdopt(device.Key);
				device.IsAdopting = true;
			} else if(device.FirmwareUpdate != null) {
				if(device.IsUpdating) {
					logger.LogWarning("Got inform while updating from {mac}", mac);
					body = InformResponse.Noop(30);
				} else {
					device.IsUpdating = true;
					logger.LogWarning("Sending firmware update to {mac}", mac);
					var fw = device.FirmwareUpdate;
					body = InformResponse.Upgrade(fw.DeviceVersion, fw.MD5, fw.URL);
				}
			} else if(request.cfgversion == InformResponse.ADOPT_CFG) {
				logger.LogWarning("Received adopt confirmation from {mac}", mac);
				body = InformResponse.SetConfig(device.ManagementConfig, device.SystemConfig);
				device.IsAdopting = false;
				device.IsUpdating = false;
				device.IsConfiguring = true;
			} else if(request.cfgversion != device.CfgVersion) {
				logger.LogError("Wrong cfgversion {mac}", mac);
				body = InformResponse.SetConfig(device.ManagementConfig, device.SystemConfig);
				device.IsConfiguring = true;
			} else if(device.IsResetting) {
				logger.LogError("Reset {mac}", mac);
				body = InformResponse.Reset();
				lock(devices) {
					devices.Remove(mac);
				}
			} else {
				var interval = Math.Max(5, request.inform_min_interval);
				logger.LogInformation("Noop {mac} interval {interval}", mac, interval);
				body = InformResponse.Noop(interval);
				device.IsAdopting = false;
				device.IsConfiguring = false;
				device.IsUpdating = false;
			}

			inform_resp.Body = JsonSerializer.Serialize(body);

			OnDeviceChange?.Invoke(device, EventArgs.Empty);
			return inform_resp.Encode(deviceKey);
		}

		public async Task Adopt(Device device) {
			//TODO: if the device is known, we should check ssh key
			if(device.IP == null) {
				throw new Exception("Requested to adopt a device without an IP");
			}
			logger.LogInformation("Starting adoption task");
			var (ourIp, _) = NetworkUtils.GetMyIPOnThisSubnet(device.IP);
			var newcmd = $"/usr/bin/syswrapper.sh set-adopt http://{ourIp}:8081/inform {device.Key}";
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

		public async Task CheckFirmwareUpdate() {
			await fwManager.UpdateFirmwareCache();
		}
	}
}
