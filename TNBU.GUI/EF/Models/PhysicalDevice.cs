using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;

namespace TNBU.GUI.EF.Models {
	public class PhysicalDevice {
		[Key]
		public PhysicalAddress MAC { get; set; } = null!;
		public string Model { get; set; } = null!;
	}
}
