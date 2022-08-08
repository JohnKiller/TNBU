using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using TNBU.Core.Models;
using TNBU.Core.Utils;

namespace TNBU.Simulator {
	public class DeviceSimulator {
		readonly InformPacket pkt;
		string authkey = InformPacket.DEFAULT_KEY;
		readonly BodyClass body;
		string KeyFile => $"key_{body.model}.txt";

		public DeviceSimulator(PhysicalAddress mac, string model, string display) {
			pkt = new InformPacket {
				MACAddress = mac,
				IsAES = true,
				IsGCM = true,
				IsZLIB = true,
				PayloadVersion = 1,
				Version = 0
			};
			body = new BodyClass {
				anon_id = "d88f24aa-2292-48f5-8a47-8fbd058a4ee9",
				architecture = "armv7l",
				board_rev = 23,
				bootid = 0,
				bootrom_version = "usw-USHR3_v1.0.12.62-g8616ed47",
				cfgversion = "?",
				@default = true,
				discovery_response = false,
				dualboot = true,
				ever_crash = false,
				guest_kicks = 0,
				guest_token = "405CDF2C152F154F9B3ADB5643AC0836",
				hash_id = "5a478fbd058a4ee9",
				hostname = model,
				inform_min_interval = 1,
				inform_url = "http://10.0.2.1:8080/inform",
				ip = "10.0.2.1",
				isolated = true,
				last_error = "",
				last_error_conns = Array.Empty<string>(),
				locating = false,
				mac = mac.GetAddressBytes().BytesToColon(),
				manufacturer_id = 2,
				model = model,
				model_display = display,
				netmask = "255.0.0.0",
				required_version = "5.76.5",
				selfrun_beacon = true,
				serial = mac.ToString(),
				state = 4,
				sys_error_caps = 0,
				time = 0,
				uptime = 0,
				version = "5.76.5.13433",
				radio_table = new Radio_Table[] {
					new Radio_Table {
						builtin_ant_gain=4,
						builtin_antenna=true,
						ieee_modes=10,
						max_txpower=23,
						min_txpower=6,
						name="wifi0",
						nss=4,
						radio="ng",
						radio_caps=67256324,
						radio_caps2=1,
						scan_table=Array.Empty<string>()
					}
				}
			};
			if(File.Exists(KeyFile)) {
				authkey = File.ReadAllText(KeyFile);
			}
		}

		public async Task Run(HttpClient client) {
			body.time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			body.uptime++;

			pkt.IV = InformPacket.GenerateIV();
			pkt.Body = System.Text.Json.JsonSerializer.Serialize(body);
			var data = pkt.Encode(authkey);
			var byteArrayContent = new ByteArrayContent(data);
			byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-binary");
			var response = await client.PostAsync(body.inform_url, byteArrayContent);

			if(response.IsSuccessStatusCode) {
				var respdata = response.Content.ReadAsStream();
				var rawstream = new MemoryStream();
				respdata.CopyTo(rawstream);
				rawstream.Position = 0;
				var inform_resp = InformPacket.Decode(rawstream);
				inform_resp.Decrypt(authkey);
				var respbody = System.Text.Json.JsonSerializer.Deserialize<BodyResp>(inform_resp.Body)!;
				switch(respbody._type) {
					case "noop":
						//Console.WriteLine("Got noop");
						break;
					case "setparam":
						if(respbody.cfgversion != null) {
							body.cfgversion = respbody.cfgversion;
						}
						if(respbody.mgmt_cfg != null) {
							var mgmt_cfg = respbody.mgmt_cfg.Split('\n');
							foreach(var cfgline in mgmt_cfg) {
								var kv = cfgline.Split('=', 2);
								switch(kv[0]) {
									case "cfgversion":
										body.cfgversion = kv[1];
										break;
									case "authkey":
										authkey = kv[1];
										File.WriteAllText(KeyFile, authkey);
										break;
								}
							}
						}
						if(respbody.system_cfg != null) {
							File.WriteAllText($"system_cfg_{body.model}.txt", respbody.system_cfg);
						}
						break;
					default:
						throw new Exception();
				}
			}
		}
	}
}
