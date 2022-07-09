using System.Net.Sockets;
using TNBU.Core.Models;
using TNBU.Core.Utils;

namespace TNBU.MitM.Services;

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
				var decoded = DiscoveryPacket.Decode(pkt.Buffer);
				if(NetworkUtils.IsFakeMac(decoded.Mac)) {
					continue;
				}
				logger.LogInformation("Got discovery packet from {ip}:\n{decoded}", pkt.RemoteEndPoint, decoded.ToString());
				deviceManagerService.GotDiscovery(decoded);
			} catch(OperationCanceledException) {
				break;
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
