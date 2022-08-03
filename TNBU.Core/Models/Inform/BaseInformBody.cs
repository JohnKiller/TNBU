#pragma warning disable IDE1006
namespace TNBU.Core.Models.Inform {
	public class BaseInformBody {
		public string? cfgversion { get; set; }
		public bool _default { get; set; }

		public bool discovery_response { get; set; }
		public int inform_min_interval { get; set; }
		public string? inform_url { get; set; }
		public bool inform_as_notif { get; set; }
		public string? notif_reason { get; set; }
		public object? notif_payload { get; set; }

		public string? hostname { get; set; }
		public string? mac { get; set; }
		public string? ip { get; set; }
		public string? netmask { get; set; }

		public bool isolated { get; set; }
		public bool locating { get; set; }

		public string? anon_id { get; set; }
		public string? hash_id { get; set; }
		public string? architecture { get; set; }
		public int board_rev { get; set; }
		public int bootid { get; set; }
		public string? bootrom_version { get; set; }
		public bool dualboot { get; set; }
		public string? kernel_version { get; set; }
		public bool ever_crash { get; set; }
		public int guest_kicks { get; set; }
		public string? guest_token { get; set; }
		public string? last_error { get; set; }
		public object[]? last_error_conns { get; set; }

		public int manufacturer_id { get; set; }
		public string? model { get; set; }
		public string? model_display { get; set; }
		public string? required_version { get; set; }
		public bool selfrun_beacon { get; set; }
		public string? serial { get; set; }
		public int state { get; set; }
		public int sys_error_caps { get; set; }
		public int time { get; set; }
		public int uptime { get; set; }
		public string? version { get; set; }
	}
}
#pragma warning restore IDE1006
