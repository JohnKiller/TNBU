namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgNtpClient {
		public override string ToString() {
			return $@"
ntpclient.status=enabled
ntpclient.1.server=0.ubnt.pool.ntp.org
ntpclient.1.status=enabled
ntpclient.2.server=1.ubnt.pool.ntp.org
ntpclient.2.status=enabled
ntpclient.3.server=2.ubnt.pool.ntp.org
ntpclient.3.status=enabled
ntpclient.4.server=3.ubnt.pool.ntp.org
ntpclient.4.status=enabled
";
		}
	}
}
