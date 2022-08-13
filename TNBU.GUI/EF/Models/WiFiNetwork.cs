using System.ComponentModel.DataAnnotations;

namespace TNBU.GUI.EF.Models {
	public class WiFiNetwork {
		public long ID { get; set; }
		[Required]
		public string SSID { get; set; }
		[Required, MinLength(8)]
		public string Password { get; set; }
	}
}
