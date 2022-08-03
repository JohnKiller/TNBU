using TNBU.Core.Models;

namespace TNBU.GUI.Services.ConfigurationBuilder {
	public class ApCfgBuilder : ICfgBuilder {
		private readonly string Phy1;
		private readonly string Dev1;
		private readonly string Phy2;
		private readonly string Dev2;

		public ApCfgBuilder(string phy1, string dev1, string phy2, string dev2) {
			Phy1 = phy1;
			Dev1 = dev1;
			Phy2 = phy2;
			Dev2 = dev2;
		}

		public SystemConfig Build() {
			return new SystemConfig {
				Aaa = new() {
					Entries = {
						new() {
							SSID = "MyFirstWifi",
							PSK = "qwertyuiop",
							DevName = Dev1
						},
						new() {
							SSID = "MyFirstWifi",
							PSK = "qwertyuiop",
							DevName = Dev2
						}
					}
				},
				Radio = new() {
					Entries = {
						new() {
							PhyName = Phy1,
							DevNames = { Dev1 }
						},
						new() {
							PhyName = Phy2,
							DevNames = { Dev2 }
						}
					}
				},
				Wireless = new() {
					Entries = {
						new() {
							DevName = Dev1,
							PhyName = Phy1,
							SSID = "MyFirstWifi"
						},
						new() {
							DevName = Dev2,
							PhyName = Phy2,
							SSID = "MyFirstWifi"
						}
					}
				},
				Bridge = new() {
					Entries = {
						new() {
							DevName = "br0",
							Ports = { "eth0", Dev1, Dev2 }
						}
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
						new() {
							DevName = Dev1,
							Up = false,
						},
						new() {
							DevName = Dev2,
							Up = false,
						}
					}
				},
			};
		}
	}
}
