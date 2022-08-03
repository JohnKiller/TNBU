using System.Security.Cryptography;
using System.Text;
using TNBU.Core.Models;
using TNBU.GUI.Models;

namespace TNBU.GUI.Services.ConfigurationBuilder {
	public class ConfigurationBuilderService {
		private readonly Dictionary<string, ICfgBuilder> Builders = new();

		public ConfigurationBuilderService() {
			Builders["U7NHD"] = new ApCfgBuilder("ra0", "ra0", "rai0", "rai0");
			Builders["U7LR"] = new ApCfgBuilder("wifi0", "ath0", "wifi1", "ath1");
		}

		public void UpdateDeviceConfiguration(Device d) {
			if(!IsDeviceSupported(d)) {
				throw new Exception("Model is not supported");
			}
			d.SystemConfig = Builders[d.Model!].Build();
			using(var md5 = MD5.Create()) {
				var cfgBytes = Encoding.UTF8.GetBytes(d.SystemConfig.ToString());
				d.CfgVersion = Convert.ToHexString(md5.ComputeHash(cfgBytes));
			}
			d.ManagementConfig = new ManagementConfig {
				CfgVersion = d.CfgVersion
			};
		}

		public bool IsDeviceSupported(Device device) {
			if(device.Model == null) {
				return false;
			}
			return Builders.ContainsKey(device.Model);
		}
	}
}
