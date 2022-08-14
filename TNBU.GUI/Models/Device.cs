using System.Net;
using System.Net.NetworkInformation;
using TNBU.Core.Models;
using TNBU.GUI.Services.FirmwareUpdate;

namespace TNBU.GUI.Models {
	public class Device {
		public PhysicalAddress Mac { get; set; } = null!;
		public string? Model { get; set; }
		public string? ModelDisplay { get; set; }
		public IPAddress? IP { get; set; }
		public string? HostName { get; set; }
		public string? Firmware { get; set; }
		public FirmwareInfo? FirmwareUpdate { get; set; }
		public DateTime LastPing { get; private set; } = DateTime.MinValue;
		public bool IsOnline {
			get {
				int maxSec;
				if(IsUpdating) {
					maxSec = 300;
				} else if(IsWorking) {
					maxSec = 120;
				} else if(IsAdopted) {
					maxSec = 30;
				} else{
					maxSec = 60;
				}
				return (DateTime.UtcNow - LastPing).TotalSeconds < maxSec;
			}
		}
		public bool IsDefault { get; set; }
		public bool IsAdopted { get; set; }
		public bool IsAdoptable => IsOnline && IsDefault && !IsAdopted;
		public bool IsWorking => IsAdopting || IsConfiguring || IsResetting;
		public string Status {
			get {
				if(!IsOnline) {
					return "Offline";
				}
				if(IsUpdating) {
					return "Updating firmware...";
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
				if(IsResetting) {
					return "Resetting...";
				}
				if(FirmwareUpdate != null) {
					return "Update available";
				}
				return "Ready";
			}
		}
		public string StatusColor {
			get {
				if(!IsOnline) {
					return "gray";
				}
				if(!IsAdopted) {
					if(IsDefault) {
						return "blue";
					}
					return "red";
				}
				if(IsResetting) {
					return "red";
				}
				if(IsAdopting || IsConfiguring) {
					return "yellow";
				}
				if(FirmwareUpdate != null || IsUpdating) {
					return "orange";
				}
				return "green";
			}
		}

		public bool IsAdopting { get; set; }
		public bool IsConfiguring { get; set; }
		public bool IsUpdating { get; set; }
		public SystemConfig? SystemConfig { get; set; }
		public string CfgVersion { get; set; } = "?";
		public ManagementConfig? ManagementConfig { get; set; }

		public bool IsResetting { get; set; }

		public List<PhysicalRadio> PhysicalRadios { get; set; } = new();
		public List<PhysicalSwitchPort> PhysicalSwitchPorts { get; set; } = new();

		public void OnlinePing() {
			LastPing = DateTime.UtcNow;
		}
	}
}
