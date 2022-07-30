using System.Net;
using System.Net.NetworkInformation;

namespace TNBU.GUI.Models {
	public class Device {
		public PhysicalAddress MAC { get; set; } = null!;
		public string Model { get; set; } = null!;
		public IPAddress? IP { get; set; }
		public bool IsConnected { get; set; }
		public bool IsDefault { get; set; }
		public bool IsAssociated { get; set; }
		public string Status {
			get {
				if(!IsConnected) {
					return "Offline";
				}
				if(!IsAssociated) {
					if(IsDefault) {
						return "Waiting for adoption";
					}
					return "Managed by another console";
				}
				return "Ready";
			}
		}
	}
}
