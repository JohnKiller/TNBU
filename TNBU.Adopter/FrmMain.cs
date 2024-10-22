using Renci.SshNet;
using System.Net.Sockets;
using TNBU.Core.Models;

namespace TNBU.Adopter {
	public partial class FrmMain : Form {
		private readonly Thread backgroundThread;

		public FrmMain() {
			InitializeComponent();
			backgroundThread = new(new ThreadStart(Run)) {
				IsBackground = true
			};
			backgroundThread.Start();
		}

		private async void Run() {
			using var client = new UdpClient(10001) {
				EnableBroadcast = true
			};
			while(true) {
				try {
					var pkt = await client.ReceiveAsync();
					if(pkt.Buffer.Length < 5) {
						continue;
					}
					DiscoveryPacket decoded;
					try {
						decoded = DiscoveryPacket.Decode(pkt.Buffer);
					} catch(Exception) {
						continue;
					}
					Invoke(() => GotDiscovery(decoded));
				} catch(OperationCanceledException) {
					break;
				} catch(Exception) {
					continue;
				}
			}
		}

		private void GotDiscovery(DiscoveryPacket decoded) {
			ListViewItem? lwi = null;
			foreach(ListViewItem itm in LwDevices.Items) {
				if(decoded.Mac.Equals(itm.Tag)) {
					lwi = itm;
					break;
				}
			}
			if(lwi == null) {
				lwi = LwDevices.Items.Add(decoded.Mac.ToString());
				lwi.Tag = decoded.Mac;
				lwi.SubItems.Add(decoded.IP.ToString());
				lwi.SubItems.Add(decoded.Model.ToString());
			} else {
				lwi.SubItems[1].Text = decoded.IP.ToString();
				lwi.SubItems[2].Text = decoded.Model.ToString();
			}
			lwi.BackColor = decoded.IsDefault ? Color.White : Color.Red;
		}

		private void LwDevices_DoubleClick(object sender, EventArgs e) {
			if(LwDevices.SelectedItems.Count == 0) {
				return;
			}
			var ip = LwDevices.SelectedItems[0].SubItems[1].Text;
			var domain = "unifi.example.com";
			var newcmd = $"/usr/bin/syswrapper.sh set-inform http://{domain}:8080/inform ";
			try {
				using var client = new SshClient(ip, "ubnt", "ubnt");
				client.Connect();
				var ret = client.RunCommand(newcmd);
				var result = ret.Result;
				if(ret.ExitStatus != 0) {
					MessageBox.Show("Wrong response from device: " + result);
				} else {
					MessageBox.Show("OK");
				}
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}
	}
}
