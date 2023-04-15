using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using TNBU.Core.Models;
using TNBU.Core.Models.DeviceConfiguration;
using TNBU.GUI.EF;
using TNBU.GUI.Models;

namespace TNBU.GUI.Services.ConfigurationBuilder {
	public class ConfigurationBuilderService {
		private readonly IDbContextFactory<DB> DBS;

		public ConfigurationBuilderService(IDbContextFactory<DB> _DBS) {
			DBS = _DBS;
		}

		public void UpdateDeviceConfiguration(Device d) {
			d.SystemConfig = Build(d);
			using(var md5 = MD5.Create()) {
				var cfgBytes = Encoding.UTF8.GetBytes(d.SystemConfig.ToString());
				d.CfgVersion = Convert.ToHexString(md5.ComputeHash(cfgBytes));
			}
			d.ManagementConfig = new ManagementConfig {
				CfgVersion = d.CfgVersion
			};
		}

		private SystemConfig Build(Device d) {
			var ret = new SystemConfig {
				Netconf = new() {
					Entries = {
						new() {
							DevName = "eth0",
							Up = true,
						},
					}
				},
				Sshd = new() {
					Interface = "eth0"
				}
			};
			if(d.PhysicalSwitchPorts.Count > 2) {
				ret.Switch.VLANs.Add(new() {
					ID = 1
				});
				ret.Switch.VLANs.Add(new() {
					ID = 10
				});
				ret.Switch.VLANs.Add(new() {
					ID = 11
				});
				if(d.Mac.ToString() == "70A741C418A8") {
					ret.Switch.VLANs[0].Ports.Add(3, false);
					ret.Switch.VLANs[1].Ports.Add(3, true);
					ret.Switch.VLANs[2].Ports.Add(3, false);

					ret.Switch.VLANs[0].Ports.Add(4, false);
					ret.Switch.VLANs[1].Ports.Add(4, false);
					ret.Switch.VLANs[2].Ports.Add(4, true);
				}
				ret.Switch.StpPriority = d.Mac.ToString() switch {
					"70A741C418A8" => 4096 * 2,
					"D021F9B8E0E4" => 4096 * 3,
					_ => 32768
				};
				foreach(var port in d.PhysicalSwitchPorts) {
					var p = new CfgSwitchEntry() {
						/*LAG = port.ID switch {
							49 => 1,
							51 => 1,
							50 => 2,
							52 => 2,
							15 => 3,
							16 => 3,
							_ => null
						},*/
						//Enabled = port.ID != 15 && port.ID != 16 && port.ID != 49 && port.ID != 50,
						//Enabled = port.ID < 10,
						//StpEnabled = port.ID > 2,
					};
					if(
						(d.Mac.ToString() == "70A741C418A8") &&
						(port.ID == 1 || port.ID == 2)
					) {
						p.LAG = 1;
					}
					if(
						(d.Mac.ToString() == "70A741C418A8") &&
						(port.ID == 3)
					) {
						p.PVID = 10;
					}
					if(
						(d.Mac.ToString() == "70A741C418A8") &&
						(port.ID == 4)
					) {
						p.PVID = 11;
					}
					ret.Switch.Entries.Add(p);
				}
			}
			using var db = DBS.CreateDbContext();
			if(d.PhysicalRadios.Count > 0) {
				ret.Mesh.Enabled = true;
				var br0 = new CfgBridgeEntry() {
					DevName = "br0",
					Ports = { "eth0" }
				};
				ret.Bridge.Entries.Add(br0);
				ret.Dhcpc.DevName = br0.DevName;
				ret.Netconf.Entries.Insert(0, new() {
					DevName = br0.DevName,
					Up = true,
					Promisc = false,
				});
				ret.Sshd.Interface = br0.DevName;
				var vDevs = new Dictionary<string, int>();
				var isAth = !d.PhysicalRadios[0].Name.StartsWith("ra");
				var athCounter = 0;
				foreach(var phy in d.PhysicalRadios) {
					var phyPrefix = phy.Name[..^1];
					var phyCounter = 0;
					var radio = new CfgRadioEntry {
						PhyName = phy.Name,
						Mode = phy.Is11AC ? "managed" : "master", //TODO: "combined" inform? first physical?
						IEEEMode = phy.Is11AC ? "11naht40" : "11nght20"
					};

					if(phy.Is11AC) {
						var vportdev = isAth ? $"ath{athCounter++}" : $"apclii{phyCounter++}";
						var vportssid = "vport-" + d.Mac.ToString();
						ret.Connectivity = new() {
							Enabled = true,
							Bridge = br0.DevName,
							WDS = vportdev,
						};
						ret.Aaa.Entries.Add(new() {
							SSID = vportssid,
							DevName = vportdev,
							BrDevName = br0.DevName,
							HideSSID = true,
							Enabled = false,
							IAPP = null,
							ID = null,
						});
						radio.DevNames.Add(vportdev);
						ret.Wireless.Entries.Add(new() {
							DevName = vportdev,
							PhyName = phy.Name,
							SSID = vportssid,
							ID = null,
							BgaFilter = false,
							HideSSID = true,
							Mode = "managed",
							Usage = "uplink",
							VPort = true,
							VWire = false,
							WDS = true,
						});
						br0.Ports.Add(vportdev);
						ret.Netconf.Entries.Add(new() {
							DevName = vportdev,
							Up = false,
						});
					}

					foreach(var w in db.WiFiNetworks.OrderBy(x => x.ID)) {
						var vdev = isAth ? $"ath{athCounter++}" : $"{phyPrefix}{phyCounter++}";
						ret.Aaa.Entries.Add(new() {
							SSID = w.SSID,
							PSK = w.Password,
							DevName = vdev,
							BrDevName = br0.DevName,
							ID = "000000000000000000000000", //????
							IAPP = "00000000000000000000000000000000", //????
						});
						radio.DevNames.Add(vdev);
						ret.Wireless.Entries.Add(new() {
							DevName = vdev,
							PhyName = phy.Name,
							SSID = w.SSID,
							ID = "000000000000000000000000", //????
						});
						br0.Ports.Add(vdev);
						ret.Netconf.Entries.Add(new() {
							DevName = vdev,
							Up = false,
						});
					}

					var vwiredev = isAth ? $"vwire{athCounter++}" : $"{phyPrefix}{phyCounter++}";
					ret.Aaa.Entries.Add(new() {
						SSID = ret.Mesh.SSID,
						PSK = ret.Mesh.PSK,
						DevName = vwiredev,
						BrDevName = br0.DevName,
						HideSSID = true,
						Enabled = true,
						IAPP = null,
						ID = null,
					});
					radio.DevNames.Add(vwiredev);
					ret.Wireless.Entries.Add(new() {
						DevName = vwiredev,
						PhyName = phy.Name,
						SSID = ret.Mesh.SSID,
						ID = null,
						BgaFilter = false,
						HideSSID = true,
						Mode = "master",
						Usage = "downlink",
						VPort = false,
						VWire = true,
						WDS = true,
					});
					br0.Ports.Add(vwiredev);
					ret.Netconf.Entries.Add(new() {
						DevName = vwiredev,
						Up = false,
					});

					ret.Radio.Entries.Add(radio);
				}
			}
			return ret;
		}
	}
}
