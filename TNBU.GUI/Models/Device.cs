using System.Net;
using System.Net.NetworkInformation;
using TNBU.Core.Models;

namespace TNBU.GUI.Models {
	public class Device {
		public PhysicalAddress Mac { get; set; } = null!;
		public string? Model { get; set; }
		public string? ModelDisplay { get; set; }
		public IPAddress? IP { get; set; }
		public string? HostName { get; set; }
		public string? Firmware { get; set; }
		public bool IsConnected { get; set; }
		public bool IsDefault { get; set; }
		public bool IsAdopted { get; set; }
		public bool IsAdoptable => IsConnected && IsDefault && !IsAdopted;
		public string Status {
			get {
				if(!IsConnected) {
					return "Offline";
				}
				if(!IsAdopted) {
					if(IsDefault) {
						return "Waiting for adoption";
					}
					return "Managed by another console";
				}
				if(IsAdopting) {
					return "Adopting...";
				}
				if(IsConfiguring) {
					return "Configuring...";
				}
				if(ResetRequested) {
					return "Resetting...";
				}
				return "Ready";
			}
		}

		public bool IsAdopting { get; set; }
		public bool IsConfiguring { get; set; }
		public SystemConfig? SystemConfig { get; set; }
		public string CfgVersion { get; set; } = "?";
		public ManagementConfig? ManagementConfig { get; set; }

		public bool ResetRequested { get; set; }

		public List<PhysicalRadio> PhysicalRadios { get; set; } = new();
	}
}
