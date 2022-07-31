using System.Net.Sockets;
using TNBU.Core.Models;

namespace TNBU.GUI.Services {
	public class DiscoveryService : IHostedService {
		private readonly Thread backgroundThread;
		private CancellationToken cancellationToken;
		private readonly DeviceManagerService deviceManagerService;
		private readonly ILogger<DiscoveryService> logger;

		public DiscoveryService(DeviceManagerService _dms, ILogger<DiscoveryService> _logger) {
			deviceManagerService = _dms;
			logger = _logger;
			backgroundThread = new(new ThreadStart(Run)) {
				IsBackground = true
			};
		}

		private async void Run() {
			using var client = new UdpClient(10001) {
				EnableBroadcast = true
			};
			logger.LogInformation("Waiting for discovery packet...");
			while(!cancellationToken.IsCancellationRequested) {
				try {
					var pkt = await client.ReceiveAsync(cancellationToken);
					if(pkt.Buffer.Length < 5) {
						logger.LogInformation("Got probe packet from {ip}", pkt.RemoteEndPoint);
						continue;
					}
					DiscoveryPacket decoded;
					try {
						decoded = DiscoveryPacket.Decode(pkt.Buffer);
						logger.LogInformation("Got discovery packet from {ip}:\n{decoded}", pkt.RemoteEndPoint, decoded.ToString());
					} catch(Exception ex) {
						logger.LogError("Failed decoding discovery packet from {ip}: {message}", pkt.RemoteEndPoint, ex.Message);
						continue;
					}
					deviceManagerService.GotDiscovery(decoded);
				} catch(OperationCanceledException) {
					break;
				} catch(Exception ex) {
					logger.LogError("DiscoveryService uncatched main-loop error: {message}", ex.Message);
				}
			}
		}

		public Task StartAsync(CancellationToken cancellationToken) {
			this.cancellationToken = cancellationToken;
			backgroundThread.Start();
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken) {
			backgroundThread.Join();
			return Task.CompletedTask;
		}
	}
}
