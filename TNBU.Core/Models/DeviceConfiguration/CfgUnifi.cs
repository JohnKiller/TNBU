namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgUnifi {
		public override string ToString() {
			return $@"
unifi.anonymous_controller_id={Guid.Empty}
unifi.anonymous_site_id={Guid.Empty}
unifi.cfgcap_info=0x7
unifi.idp=enabled
unifi.key=000000000000000000000000000000000
unifi.mcip=239.254.127.63
unifi.reporterid={Guid.Empty}
unifi.siteid=0000000000000000000000000
unifi.version=7.1.66
";
		}
	}
}
