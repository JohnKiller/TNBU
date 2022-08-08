using System.Net.Http.Headers;
using System.Net.NetworkInformation;

namespace TNBU.Simulator {
	internal class Program {
		static async Task Main(string[] args) {
			Console.WriteLine("Hello, World!");
			var client = new HttpClient(new SocketsHttpHandler {
				ActivityHeadersPropagator = null
			});
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

			var macs = new List<PhysicalAddress>();
			var mac = PhysicalAddress.Parse("1E:2E:3E:4E:5E:00");
			for(var j = 0; j < 200; j++) {
				var bytes = mac.GetAddressBytes();
				bytes[5] = (byte)j;
				macs.Add(new PhysicalAddress(bytes));
			}
			var i = 0;
			var devices = new List<DeviceSimulator> {
				/*new DeviceSimulator(macs[i++], "BZ2", "UniFi AP"),
				new DeviceSimulator(macs[i++], "BZ2LR", "UniFi AP-LR"),
				new DeviceSimulator(macs[i++], "S216150", "UniFi Switch 16 AT-150W"),
				new DeviceSimulator(macs[i++], "S224250", "UniFi Switch 24 AT-250W"),
				new DeviceSimulator(macs[i++], "S224500", "UniFi Switch 24 AT-500W"),
				new DeviceSimulator(macs[i++], "S248500", "UniFi Switch 48 AT-500W"),
				new DeviceSimulator(macs[i++], "S248750", "UniFi Switch 48 AT-750W"),
				new DeviceSimulator(macs[i++], "S28150", "UniFi Switch 8 AT-150W"),
				new DeviceSimulator(macs[i++], "U2HSR", "UniFi AP-Outdoor+"),
				new DeviceSimulator(macs[i++], "U2IW", "UniFi AP-In Wall"),
				new DeviceSimulator(macs[i++], "U2Lv2", "UniFi AP-LR v2"),
				new DeviceSimulator(macs[i++], "U2O", "UniFi AP-Outdoor"),
				new DeviceSimulator(macs[i++], "U2Sv2", "UniFi AP v2"),
				new DeviceSimulator(macs[i++], "U5O", "UniFi AP-Outdoor 5G"),
				new DeviceSimulator(macs[i++], "U6ENT", "U6-Enterprise"),
				new DeviceSimulator(macs[i++], "U6ENTIW", "U6-Enterprise-IW"),
				new DeviceSimulator(macs[i++], "U6EXT", "U6-Extender"),
				new DeviceSimulator(macs[i++], "U6IW", "U6-IW"),
				new DeviceSimulator(macs[i++], "U6M", "U6-Mesh"),
				new DeviceSimulator(macs[i++], "U7E", "UniFi AP-AC"),
				new DeviceSimulator(macs[i++], "U7EDU", "UniFi AP-AC-EDU"),
				new DeviceSimulator(macs[i++], "U7Ev2", "UniFi AP-AC v2"),
				new DeviceSimulator(macs[i++], "U7HD", "UniFi AP-HD"),
				new DeviceSimulator(macs[i++], "U7IW", "UniFi AP-AC-In Wall"),
				new DeviceSimulator(macs[i++], "U7IWP", "UniFi AP-AC-In Wall Pro"),*/
				new DeviceSimulator(macs[i++], "U7LR", "UniFi AP-AC-LR"),
				/*new DeviceSimulator(macs[i++], "U7LT", "UniFi AP-AC-Lite"),
				new DeviceSimulator(macs[i++], "U7MP", "UniFi AP-AC-Mesh-Pro"),
				new DeviceSimulator(macs[i++], "U7MSH", "UniFi AP-AC-Mesh"),
				new DeviceSimulator(macs[i++], "U7NHD", "UniFi AP-nanoHD"),
				new DeviceSimulator(macs[i++], "U7O", "UniFi AP-AC Outdoor"),
				new DeviceSimulator(macs[i++], "U7P", "UniFi AP-Pro"),
				new DeviceSimulator(macs[i++], "U7PG2", "UniFi AP-AC-Pro"),
				new DeviceSimulator(macs[i++], "U7SHD", "UniFi AP-SHD"),
				new DeviceSimulator(macs[i++], "UAE6", "U6-Extender-EA"),
				new DeviceSimulator(macs[i++], "UAIW6", "U6-IW-EA"),
				new DeviceSimulator(macs[i++], "UAL6", "U6-Lite"),
				new DeviceSimulator(macs[i++], "UALR6", "U6-LR-EA"),
				new DeviceSimulator(macs[i++], "UALR6v2", "U6-LR"),
				new DeviceSimulator(macs[i++], "UALR6v3", "U6-LR"),
				new DeviceSimulator(macs[i++], "UAM6", "U6-Mesh-EA"),
				new DeviceSimulator(macs[i++], "UAP6", "U6-LR"),
				new DeviceSimulator(macs[i++], "UAP6MP", "U6-Pro"),
				new DeviceSimulator(macs[i++], "UBB", "UniFi Building Bridge"),
				new DeviceSimulator(macs[i++], "UBBXG", "UniFi Building-to-Building XG"),
				new DeviceSimulator(macs[i++], "UCK", "UniFi Cloud Key"),
				new DeviceSimulator(macs[i++], "UCKG2", "UniFi Cloud Key Gen2"),
				new DeviceSimulator(macs[i++], "UCKP", "UniFi Cloud Key Plus"),
				new DeviceSimulator(macs[i++], "UCMSH", "UniFi AP-MeshXG"),
				new DeviceSimulator(macs[i++], "UCXG", "UniFi AP-XG"),
				new DeviceSimulator(macs[i++], "UDC48X6", "UniFi Data Center 100G-48X6"),
				new DeviceSimulator(macs[i++], "UDM", "UniFi Dream Machine"),
				new DeviceSimulator(macs[i++], "UDMB", "UDM Beacon"),
				new DeviceSimulator(macs[i++], "UDMPRO", "UniFi Dream Machine PRO"),
				new DeviceSimulator(macs[i++], "UDMPROSE", "UniFi Dream Machine PRO SE"),
				new DeviceSimulator(macs[i++], "UDMSE", "UniFi Dream Machine SE"),
				new DeviceSimulator(macs[i++], "UDR", "UniFi Dream Router"),
				new DeviceSimulator(macs[i++], "UDW", "UniFi Dream Wall"),
				new DeviceSimulator(macs[i++], "UFLHD", "UniFi Flex HD"),
				new DeviceSimulator(macs[i++], "UGW3", "UniFi Security Gateway"),
				new DeviceSimulator(macs[i++], "UGW4", "UniFi Security Gateway-Pro"),
				new DeviceSimulator(macs[i++], "UGWXG", "UniFi Security Gateway XG-8"),
				new DeviceSimulator(macs[i++], "UHDIW", "UniFi AP-HD-In Wall"),
				new DeviceSimulator(macs[i++], "ULTE", "UniFi AP-LTE"),
				new DeviceSimulator(macs[i++], "ULTEPEU", "UniFi LTE Pro EU"),
				new DeviceSimulator(macs[i++], "ULTEPUS", "UniFi LTE Pro US"),
				new DeviceSimulator(macs[i++], "UP1", "UniFi Smart Power Plug"),
				new DeviceSimulator(macs[i++], "UP6", "UniFi Smart Power 6-Port Power Strip"),
				new DeviceSimulator(macs[i++], "US16P150", "UniFi Switch 16 POE-150W"),
				new DeviceSimulator(macs[i++], "US24", "UniFi Switch 24"),
				new DeviceSimulator(macs[i++], "US24P250", "UniFi Switch 24 POE-250W"),
				new DeviceSimulator(macs[i++], "US24P500", "UniFi Switch 24 POE-500W"),
				new DeviceSimulator(macs[i++], "US24PL2", "UniFi Switch 24 L2 POE"),
				new DeviceSimulator(macs[i++], "US24PRO", "UniFi Switch Pro 24 PoE"),
				new DeviceSimulator(macs[i++], "US24PRO2", "UniFi Switch Pro 24"),
				new DeviceSimulator(macs[i++], "US48", "UniFi Switch 48"),
				new DeviceSimulator(macs[i++], "US48P500", "UniFi Switch 48 POE-500W"),
				new DeviceSimulator(macs[i++], "US48P750", "UniFi Switch 48 POE-750W"),
				new DeviceSimulator(macs[i++], "US48PL2", "UniFi Switch 48 L2 POE"),
				new DeviceSimulator(macs[i++], "US48PRO", "UniFi Switch Pro 48 PoE"),
				new DeviceSimulator(macs[i++], "US48PRO2", "UniFi Switch Pro 48"),
				new DeviceSimulator(macs[i++], "US624P", "UniFi Switch Enterprise 24 PoE"),
				new DeviceSimulator(macs[i++], "US648P", "UniFi Switch Enterprise 48 PoE"),
				new DeviceSimulator(macs[i++], "US68P", "UniFi Switch Enterprise 8 PoE"),
				new DeviceSimulator(macs[i++], "US6XG150", "UniFi Switch 6 XG PoE"),
				new DeviceSimulator(macs[i++], "US8", "UniFi Switch 8"),
				new DeviceSimulator(macs[i++], "US8P150", "UniFi Switch 8 POE-150W"),
				new DeviceSimulator(macs[i++], "US8P60", "UniFi Switch 8 POE-60W"),
				new DeviceSimulator(macs[i++], "USAGGPRO", "Unifi Switch Pro Aggregation"),
				new DeviceSimulator(macs[i++], "USC8", "UniFi Switch 8"),
				new DeviceSimulator(macs[i++], "USC8P450", "UniFi Switch Industrial"),
				new DeviceSimulator(macs[i++], "USF5P", "UniFi Switch Flex"),
				new DeviceSimulator(macs[i++], "USFXG", "UniFi Switch Flex XG"),
				new DeviceSimulator(macs[i++], "USL16LP", "UniFi Switch Lite 16 POE"),
				new DeviceSimulator(macs[i++], "USL16P", "UniFi Switch 16 POE"),
				new DeviceSimulator(macs[i++], "USL24", "UniFi Switch 24"),
				new DeviceSimulator(macs[i++], "USL24P", "UniFi Switch 24 POE"),
				new DeviceSimulator(macs[i++], "USL48", "UniFi Switch 48"),
				new DeviceSimulator(macs[i++], "USL48P", "UniFi Switch 48 POE"),
				new DeviceSimulator(macs[i++], "USL8A", "UniFi Switch Aggregation"),
				new DeviceSimulator(macs[i++], "USL8LP", "UniFi Switch Lite 8 POE"),
				new DeviceSimulator(macs[i++], "USL8MP", "UniFi Switch Mission Critical"),
				new DeviceSimulator(macs[i++], "USMINI", "Unifi Switch Mini"),
				new DeviceSimulator(macs[i++], "USPPDUP", "UniFi Smart Power Power Distribution Unit Pro"),
				new DeviceSimulator(macs[i++], "USPRPS", "UniFi Smart Power Redundant Power System"),
				new DeviceSimulator(macs[i++], "USPRPSP", "UniFi SmartPower - Redundant Power System Pro"),
				new DeviceSimulator(macs[i++], "USXG", "UniFi Switch XG"),
				new DeviceSimulator(macs[i++], "USXG24", "UniFi Switch Enterprise XG 24"),
				new DeviceSimulator(macs[i++], "UXBSDM", "UniFi AP-BlackBaseStationXG"),
				new DeviceSimulator(macs[i++], "UXGPRO", "UniFi NeXt-Gen Gateway PRO"),
				new DeviceSimulator(macs[i++], "UXSDM", "UniFi AP-BaseStationXG"),
				new DeviceSimulator(macs[i++], "p2N", "PicoStation M2"),*/
			};

			while(true) {
				foreach(var d in devices) {
					await d.Run(client);
				}
				await Task.Delay(1000);
			}
		}
	}
}
