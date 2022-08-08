using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNBU.Simulator {
	public class BodyClass {
		public string anon_id { get; set; }
		public string architecture { get; set; }
		public int board_rev { get; set; }
		public int bootid { get; set; }
		public string bootrom_version { get; set; }
		public string cfgversion { get; set; }
		public bool @default { get; set; }
		public bool discovery_response { get; set; }
		public bool dualboot { get; set; }
		public bool ever_crash { get; set; }
		public int guest_kicks { get; set; }
		public string guest_token { get; set; }
		public string hash_id { get; set; }
		public string hostname { get; set; }
		public int inform_min_interval { get; set; }
		public string inform_url { get; set; }
		public string ip { get; set; }
		public bool isolated { get; set; }
		public string last_error { get; set; }
		public string[] last_error_conns { get; set; }
		public bool locating { get; set; }
		public string mac { get; set; }
		public int manufacturer_id { get; set; }
		public string model { get; set; }
		public string model_display { get; set; }
		public string netmask { get; set; }
		public string required_version { get; set; }
		public bool selfrun_beacon { get; set; }
		public string serial { get; set; }
		public int state { get; set; }
		public int sys_error_caps { get; set; }
		public long time { get; set; }
		public int uptime { get; set; }
		public string version { get; set; }
		public Radio_Table[] radio_table { get; set; }
	}

	public class Radio_Table {
		public int builtin_ant_gain { get; set; }
		public bool builtin_antenna { get; set; }
		public int ieee_modes { get; set; }
		public int max_txpower { get; set; }
		public int min_txpower { get; set; }
		public string name { get; set; }
		public int nss { get; set; }
		public string radio { get; set; }
		public int radio_caps { get; set; }
		public int radio_caps2 { get; set; }
		public string[] scan_table { get; set; }
	}
}
