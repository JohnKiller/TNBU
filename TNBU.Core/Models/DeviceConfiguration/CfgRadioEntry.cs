namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgRadioEntry {
		public string PhyName { get; set; } = "wifi0";
		public List<string> DevNames { get; } = new() { "ath0" };

		public string GetConfig(int num) {
			return $@"
radio.{num}.ack.auto=disabled
radio.{num}.acktimeout=64
radio.{num}.ampdu.status=enabled
radio.{num}.antenna.gain=3
radio.{num}.antenna=-1
radio.{num}.backup_channel=0
radio.{num}.bcmc_l2_filter.status=enabled
radio.{num}.bgscan.status=disabled
radio.{num}.channel=0
radio.{num}.clksel=1
radio.{num}.countrycode=380
radio.{num}.cwm.enable=0
radio.{num}.cwm.mode=0
radio.{num}.forbiasauto=0
radio.{num}.hard_noisefloor.status=disabled
radio.{num}.ieee_mode=11nght20
radio.{num}.mode=master
radio.{num}.phyname={PhyName}
radio.{num}.rate.auto=enabled
radio.{num}.rate.mcs=auto
radio.{num}.rfscan=disabled
radio.{num}.status=enabled
radio.{num}.txpower_mode=auto
radio.{num}.txpower=20
radio.{num}.devname={DevNames.First()}
{string.Join('\n', DevNames.Select((x, i) => @$"
radio.{num}.virtual.{i}.devname={x}
radio.{num}.virtual.{i}.status=enabled").Skip(1))}
";
		}
	}
}
