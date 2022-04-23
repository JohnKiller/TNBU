namespace TNBU.MitM.Services;

public class SSHServerFactoryService {
	private readonly ILogger<SSHServerFactoryService> logger;
	private ushort port = 3000;

	public SSHServerFactoryService(ILogger<SSHServerFactoryService> _logger) {
		logger = _logger;
	}

	public DeviceSSHService CreateSSHServer() {
		return new DeviceSSHService(logger, port++);
	}
}
