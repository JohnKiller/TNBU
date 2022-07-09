using TNBU.Core.Models.DeviceConfiguration;

namespace TNBU.Core.Models {
	public partial class SystemConfig {
		public CfgUnifi Unifi { get; set; } = new();
		public CfgSystem System { get; set; } = new();
		public CfgUsers Users { get; set; } = new();
		public CfgAaa Aaa { get; set; } = new();
		public CfgRadio Radio { get; set; } = new();
		public CfgWireless Wireless { get; set; } = new();
		public CfgMesh Mesh { get; set; } = new();
		public CfgBandSteering BandSteering { get; set; } = new();
		public CfgStaMgr StaMgr { get; set; } = new();
		public CfgConnectivity Connectivity { get; set; } = new();
		public CfgVlan Vlan { get; set; } = new();
		public CfgBridge Bridge { get; set; } = new();
		public CfgMark Mark { get; set; } = new();
		public CfgQos Qos { get; set; } = new();
		public CfgNetconf Netconf { get; set; } = new();
		public CfgMacAcl MacAcl { get; set; } = new();
		public CfgDhcpc Dhcpc { get; set; } = new();
		public CfgRoute Route { get; set; } = new();
		public CfgResolv Resolv { get; set; } = new();
		public CfgEbTables EbTables { get; set; } = new();
		public CfgIpTables IpTables { get; set; } = new();
		public CfgRedirector Redirector { get; set; } = new();
		public CfgIpSet IpSet { get; set; } = new();
		public CfgDnsMasq DnsMasq { get; set; } = new();
		public CfgSysLog SysLog { get; set; } = new();
		public CfgSshd Sshd { get; set; } = new();
		public CfgNtpClient NtpClient { get; set; } = new();
		public CfgSwitch Switch { get; set; } = new();
		public override string ToString() {
			return $@"
# unifi
{Unifi}
# system
{System}
# users
{Users}
# wlans (aaa)
{Aaa}
# wlans (radio)
{Radio}
# wlans (wireless)
{Wireless}
# mesh
{Mesh}
# bandsteering
{BandSteering}
# stamgr
{StaMgr}
# connectivity
{Connectivity}
# vlan
{Vlan}
# bridge
{Bridge}
# mark
{Mark}
# qos
{Qos}
# netconf
{Netconf}
# mac acl
{MacAcl}
# dhcpc
{Dhcpc}
# route
{Route}
# resolv
{Resolv}
# ebtables
{EbTables}
# iptables
{IpTables}
# redirector
{Redirector}
# ipset
{IpSet}
# dnsmasq
{DnsMasq}
# syslog
{SysLog}
# sshd
{Sshd}
# ntpclient
{NtpClient}
# switch
{Switch}
# DPI Fingerprint
# misc
";
		}
	}
}
