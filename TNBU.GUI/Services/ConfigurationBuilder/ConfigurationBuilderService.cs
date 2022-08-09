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
			var br0 = new CfgBridgeEntry() {
				DevName = "br0",
				Ports = { "eth0" }
			};
			var ret = new SystemConfig {
				Bridge = new() {
					Entries = {
						br0
					}
				},
				Netconf = new() {
					Entries = {
						new() {
							DevName = "br0",
							Up = true,
						},
						new() {
							DevName = "eth0",
							Up = true,
						},
					}
				}
			};
			using var db = DBS.CreateDbContext();
			var vDevs = new Dictionary<string, int>();
			var isAth = !d.PhysicalRadios[0].Name.StartsWith("ra");
			var athCounter = 0;
			foreach(var phy in d.PhysicalRadios) {
				var phyPrefix = phy.Name[..^1];
				var phyCounter = 0;
				var radio = new CfgRadioEntry {
					PhyName = phy.Name,
				};
				foreach(var w in db.WiFiNetworks.OrderBy(x => x.ID)) {
					var vdev = isAth ? $"ath{athCounter++}" : $"{phyPrefix}{phyCounter++}";
					ret.Aaa.Entries.Add(new() {
						SSID = w.SSID,
						PSK = w.Password,
						DevName = vdev,
						BrDevName = br0.DevName,
					});
					radio.DevNames.Add(vdev);
					ret.Wireless.Entries.Add(new() {
						DevName = vdev,
						PhyName = phy.Name,
						SSID = w.SSID,
						//ID = "", ????
					});
					br0.Ports.Add(vdev);
					ret.Netconf.Entries.Add(new() {
						DevName = vdev,
						Up = false,
					});
				}
				ret.Radio.Entries.Add(radio);
			}
			return ret;
		}
	}
}
