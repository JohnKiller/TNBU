using System.Net.NetworkInformation;
using TNBU.GUI.Models;

namespace TNBU.GUI.Services {
	public class DeviceManagerService {
		public Dictionary<PhysicalAddress, Device> Devices { get; } = new();
	}
}
