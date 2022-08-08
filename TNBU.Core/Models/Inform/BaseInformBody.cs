#pragma warning disable IDE1006
namespace TNBU.Core.Models.Inform {
	public class BaseInformBody {
		public string? cfgversion { get; set; }
		public bool @default { get; set; }

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

		public Radio_Table[] radio_table { get; set; }
	}

	public class Radio_Table {
		public string? name { get; set; }
		public string? radio { get; set; }
		public bool is_11ac { get; set; }
		public bool has_dfs { get; set; }
		public bool has_fccdfs { get; set; }

		public int builtin_ant_gain { get; set; }
		public bool builtin_antenna { get; set; }

		public Athstats? athstats { get; set; }
		public int ieee_modes { get; set; }
		public int max_txpower { get; set; }
		public int min_txpower { get; set; }
		public int nss { get; set; }
		public int radio_caps { get; set; }
		public int radio_caps2 { get; set; }

		public Scan_Table[]? scan_table { get; set; }
	}

	public class Athstats {
		public string? name { get; set; }
		public int ast_ath_reset { get; set; }
		public int ast_be_xmit { get; set; }
		public int ast_cst { get; set; }
		public int ast_deadqueue_reset { get; set; }
		public int ast_fullqueue_stop { get; set; }
		public int ast_txto { get; set; }
		public int ce_send_fail_cnt { get; set; }
		public int cu_self_rx { get; set; }
		public int cu_self_tx { get; set; }
		public int cu_total { get; set; }
		public int invalid_mac_addr_cnt { get; set; }
		public int n_rx_aggr { get; set; }
		public int n_rx_pkts { get; set; }
		public int n_tx_bawadv { get; set; }
		public int n_tx_bawretries { get; set; }
		public int n_tx_pkts { get; set; }
		public int n_tx_queue { get; set; }
		public int n_tx_retries { get; set; }
		public int n_tx_xretries { get; set; }
		public int n_txaggr_compgood { get; set; }
		public int n_txaggr_compretries { get; set; }
		public int n_txaggr_compxretry { get; set; }
		public int n_txaggr_prepends { get; set; }
		public int noise_floor { get; set; }
		public int satisfaction { get; set; }
		public int satisfaction_now { get; set; }
		public int satisfaction_real { get; set; }
		public int timeout_waiting_for_vap_cnt { get; set; }
	}

	public class Scan_Table {
		public int age { get; set; }
		public string? band { get; set; }
		public string? bssid { get; set; }
		public int bw { get; set; }
		public int center_freq { get; set; }
		public int channel { get; set; }
		public string? essid { get; set; }
		public int freq { get; set; }
		public bool is_adhoc { get; set; }
		public bool is_ubnt { get; set; }
		public int noise { get; set; }
		public int rssi { get; set; }
		public int rssi_age { get; set; }
		public string? security { get; set; }
		public int signal { get; set; }
		public string? anon_id { get; set; }
		public string? fw_version { get; set; }
		public bool is_default { get; set; }
		public bool is_isolated { get; set; }
		public bool is_locating { get; set; }
		public bool is_meshv3 { get; set; }
		public bool is_unifi { get; set; }
		public bool is_vport { get; set; }
		public bool is_vwire { get; set; }
		public string? model { get; set; }
		public string? model_display { get; set; }
		public string? serialno { get; set; }
	}
}
#pragma warning restore IDE1006
