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
		public string? uplink { get; set; }

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

		public Lldp_Table[]? lldp_table { get; set; }
		public Vap_Table[]? vap_table { get; set; }

		public Radio_Table[]? radio_table { get; set; }
		public Port_Table[]? port_table { get; set; }

	}

	public class Vap_Table {
		public Anomalies_Bar_Chart? anomalies_bar_chart { get; set; }
		public Anomalies_Bar_Chart? anomalies_bar_chart_now { get; set; }
		public int avg_client_signal { get; set; }
		public string? bssid { get; set; }
		public int bw { get; set; }
		public int ccq { get; set; }
		public int channel { get; set; }
		public int dns_avg_latency { get; set; }
		public string? essid { get; set; }
		public int extchannel { get; set; }
		public int icmp_avg_rtt { get; set; }
		public string? id { get; set; }
		public int mac_filter_rejections { get; set; }
		public string? name { get; set; }
		public int num_satisfaction_sta { get; set; }
		public int num_sta { get; set; }
		public string? radio { get; set; }
		public string? radio_name { get; set; }
		public Reasons_Bar_Chart? reasons_bar_chart { get; set; }
		public Reasons_Bar_Chart? reasons_bar_chart_now { get; set; }
		public int rx_bytes { get; set; }
		public int rx_crypts { get; set; }
		public int rx_dropped { get; set; }
		public int rx_errors { get; set; }
		public int rx_frags { get; set; }
		public int rx_nwids { get; set; }
		public int rx_packets { get; set; }
		public Tcp_Stats? rx_tcp_stats { get; set; }
		public int satisfaction { get; set; }
		public int satisfaction_now { get; set; }
		public int satisfaction_real { get; set; }
		public Sta_Table[]? sta_table { get; set; }
		public string? state { get; set; }
		public int tx_bytes { get; set; }
		public int tx_combined_retries { get; set; }
		public int tx_data_mpdu_bytes { get; set; }
		public int tx_dropped { get; set; }
		public int tx_errors { get; set; }
		public int tx_packets { get; set; }
		public int tx_power { get; set; }
		public int tx_retries { get; set; }
		public int tx_rts_retries { get; set; }
		public int tx_success { get; set; }
		public Tcp_Stats? tx_tcp_stats { get; set; }
		public int tx_total { get; set; }
		public bool up { get; set; }
		public string? usage { get; set; }
		public int wifi_tx_attempts { get; set; }
		public int wifi_tx_dropped { get; set; }
		public Wifi_Latency? wifi_tx_latency_mov { get; set; }
	}

	public class Anomalies_Bar_Chart {
		public int high_disconnect_count { get; set; }
		public int high_dns_latency { get; set; }
		public int high_icmp_rtt { get; set; }
		public int high_tcp_latency { get; set; }
		public int high_tcp_packet_loss { get; set; }
		public int high_wifi_latency { get; set; }
		public int high_wifi_retries { get; set; }
		public int low_phy_rate { get; set; }
		public int no_dhcp_response { get; set; }
		public int poor_stream_eff { get; set; }
		public int sleepy_client { get; set; }
		public int sta_arp_timeout { get; set; }
		public int sta_dns_timeout { get; set; }
		public int sta_ip_timeout { get; set; }
		public int weak_signal { get; set; }
	}

	public class Reasons_Bar_Chart {
		public int no_dhcp_response { get; set; }
		public int phy_rate { get; set; }
		public int signal { get; set; }
		public int sleepy_client { get; set; }
		public int sta_arp_timeout { get; set; }
		public int sta_disconnects { get; set; }
		public int sta_dns_latency { get; set; }
		public int sta_dns_timeout { get; set; }
		public int sta_icmp_rtt { get; set; }
		public int sta_ip_timeout { get; set; }
		public int stream_eff { get; set; }
		public int tcp_latency { get; set; }
		public int tcp_packet_loss { get; set; }
		public int wifi_latency { get; set; }
		public int wifi_retries { get; set; }
	}

	public class Sta_Table {
		public int anomalies { get; set; }
		public int anomalies_now { get; set; }
		public string? anon_client_id { get; set; }
		public long auth_time { get; set; }
		public bool authorized { get; set; }
		public int ccq { get; set; }
		public int dhcpend_time { get; set; }
		public int dhcpstart_time { get; set; }
		public int dns_avg_latency { get; set; }
		public string? hostname { get; set; }
		public int idletime { get; set; }
		public string? ip { get; set; }
		public bool is_11a { get; set; }
		public bool is_11ac { get; set; }
		public bool is_11n { get; set; }
		public string? mac { get; set; }
		public int noise { get; set; }
		public int nss { get; set; }
		public int rssi { get; set; }
		public int rx_bytes { get; set; }
		public int rx_bytes_mov { get; set; }
		public int rx_mcast { get; set; }
		public int rx_packets { get; set; }
		public int rx_rate { get; set; }
		public int rx_rate_mov { get; set; }
		public int rx_retries { get; set; }
		public Tcp_Stats? rx_tcp_stats { get; set; }
		public int satisfaction { get; set; }
		public int satisfaction_now { get; set; }
		public int satisfaction_real { get; set; }
		public int satisfaction_reason { get; set; }
		public int satisfaction_reason_now { get; set; }
		public int[]? satisfaction_subscores_now { get; set; }
		public string? serialno { get; set; }
		public int signal { get; set; }
		public int skip_satisfaction { get; set; }
		public int state { get; set; }
		public bool state_ht { get; set; }
		public bool state_pwrmgt { get; set; }
		public int tx_bytes { get; set; }
		public int tx_bytes_mov { get; set; }
		public int tx_combined_retries { get; set; }
		public int tx_data_mpdu_bytes { get; set; }
		public int tx_mcs { get; set; }
		public int tx_packets { get; set; }
		public int tx_power { get; set; }
		public int tx_rate { get; set; }
		public int tx_rate_mov { get; set; }
		public int tx_retries { get; set; }
		public int tx_retries_mov { get; set; }
		public int tx_rts_retries { get; set; }
		public Tcp_Stats? tx_tcp_stats { get; set; }
		public int tx_total { get; set; }
		public int tx_total_mov { get; set; }
		public int uptime { get; set; }
		public int vlan_id { get; set; }
		public int wifi_tx_attempts { get; set; }
		public int wifi_tx_attempts_mov { get; set; }
		public int wifi_tx_dropped { get; set; }
		public int wifi_tx_dropped_mov { get; set; }
		public Wifi_Latency? wifi_tx_latency_mov { get; set; }
		public int wifi_tx_success { get; set; }
	}

	public class Tcp_Stats {
		public int goodbytes { get; set; }
		public int lat_avg { get; set; }
		public int lat_max { get; set; }
		public int lat_min { get; set; }
		public int lat_samples { get; set; }
		public int lat_sum { get; set; }
		public int retries { get; set; }
		public int stalls { get; set; }
	}

	public class Wifi_Latency {
		public int avg { get; set; }
		public int max { get; set; }
		public int min { get; set; }
		public int total { get; set; }
		public int total_count { get; set; }
	}

	public class Lldp_Table {
		public string? chassis_descr { get; set; }
		public string? chassis_id { get; set; }
		public bool is_wired { get; set; }
		public int local_port_idx { get; set; }
		public string? local_port_name { get; set; }
		public string? port_descr { get; set; }
		public string? port_id { get; set; }
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

	public class Port_Table {
		public int anomalies { get; set; }
		public bool autoneg { get; set; }
		public string? dot1x_mode { get; set; }
		public string? dot1x_status { get; set; }
		public bool enable { get; set; }
		public bool flowctrl_rx { get; set; }
		public bool flowctrl_tx { get; set; }
		public bool full_duplex { get; set; }
		public bool is_uplink { get; set; }
		public bool jumbo { get; set; }
		public Mac_Table[]? mac_table { get; set; }
		public string? media { get; set; }
		public int poe_caps { get; set; }
		public string? poe_class { get; set; }
		public string? poe_current { get; set; }
		public bool poe_enable { get; set; }
		public bool poe_good { get; set; }
		public string? poe_mode { get; set; }
		public string? poe_power { get; set; }
		public string? poe_voltage { get; set; }
		public int port_idx { get; set; }
		public bool port_poe { get; set; }
		public int rx_broadcast { get; set; }
		public int rx_bytes { get; set; }
		public int rx_dropped { get; set; }
		public int rx_errors { get; set; }
		public int rx_multicast { get; set; }
		public int rx_packets { get; set; }
		public int satisfaction { get; set; }
		public int satisfaction_reason { get; set; }
		public int speed { get; set; }
		public int speed_caps { get; set; }
		public int stp_pathcost { get; set; }
		public string? stp_state { get; set; }
		public int tx_broadcast { get; set; }
		public int tx_bytes { get; set; }
		public int tx_dropped { get; set; }
		public int tx_errors { get; set; }
		public int tx_multicast { get; set; }
		public int tx_packets { get; set; }
		public bool up { get; set; }
		public string? sfp_compliance { get; set; }
		public string? sfp_current { get; set; }
		public bool sfp_found { get; set; }
		public string? sfp_part { get; set; }
		public string? sfp_rev { get; set; }
		public bool sfp_rxfault { get; set; }
		public string? sfp_rxpower { get; set; }
		public string? sfp_serial { get; set; }
		public string? sfp_temperature { get; set; }
		public bool sfp_txfault { get; set; }
		public string? sfp_txpower { get; set; }
		public string? sfp_vendor { get; set; }
		public string? sfp_voltage { get; set; }
	}

	public class Mac_Table {
		public int age { get; set; }
		public string? hostname { get; set; }
		public string? ip { get; set; }
		public string? mac { get; set; }
		public bool @static { get; set; }
		public int uptime { get; set; }
		public int vlan { get; set; }
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
