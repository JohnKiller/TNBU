namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgMark {
		public override string ToString() {
			return $@"
mark.1.bits=2
mark.1.location=skb
mark.1.name=guest
mark.1.shift=30
mark.2.bits=5
mark.2.location=ubnt
mark.2.name=vapid
mark.2.shift=11
mark.3.bits=11
mark.3.location=ubnt
mark.3.name=associd
mark.3.shift=0
";
		}
	}
}
