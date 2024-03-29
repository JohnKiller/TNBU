using System.Net;
using System.Net.NetworkInformation;
using TNBU.Core.Models;
using TNBU.Core.Models.Inform;
using TNBU.GUI.Services.FirmwareUpdate;

namespace TNBU.GUI.Models {
	public class Device {
		public string Key { get; }
		public PhysicalAddress Mac { get; }

		public Device(string key, PhysicalAddress mac) {
			Key = key;
			Mac = mac;
		}

		public string? Model { get; set; }
		public string? ModelDisplay { get; set; }
		public IPAddress? IP { get; set; }
		public string? HostName { get; set; }
		public string? Firmware { get; set; }
		public FirmwareInfo? FirmwareUpdate { get; set; }
		public DateTime LastPing { get; private set; } = DateTime.MinValue;
		public DateTime LastInform { get; private set; } = DateTime.MinValue;
		public bool IsOnline => (DateTime.UtcNow - LastPing).TotalSeconds < MaxOfflineTime;
		public bool IsInformValid => (DateTime.UtcNow - LastInform).TotalSeconds < MaxOfflineTime;
		private int MaxOfflineTime {
			get {
				if(IsUpdating) {
					return 300;
				} else if(IsWorking) {
					return 120;
				} else if(IsAdopted) {
					return 30;
				} else {
					return 60;
				}
			}
		}
		public bool Isolated { get; set; }
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
					return "Updating firmware";
				}
				if(!IsAdopted) {
					if(IsDefault) {
						return "Waiting for adoption";
					}
					return "Managed by another console";
				}
				if(IsAdopting) {
					return "Adopting";
				}
				if(IsConfiguring) {
					return "Configuring";
				}
				if(IsResetting) {
					return "Resetting";
				}
				if(Isolated) {
					return "Isolated";
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
				if(IsWorking) {
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

		public ExtendedInformBody? Inform { get; set; }

		public void OnlinePing(bool isInform) {
			LastPing = DateTime.UtcNow;
			if(isInform) {
				LastInform = DateTime.UtcNow;
			}
		}
	}
}
