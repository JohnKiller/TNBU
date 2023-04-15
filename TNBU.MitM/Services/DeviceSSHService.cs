using FxSsh;
using FxSsh.Services;
using System.Text;
using TNBU.MitM.Models;

namespace TNBU.MitM.Services;

public class DeviceSSHService : IDisposable {
	public string Key { get; }
	public string Hash { get; }
	public ushort Port { get; }
	public DeviceRelay? Owner { get; set; }

	private readonly ILogger logger;
	private readonly SshServer server;

	public DeviceSSHService(ILogger _logger, ushort _port, string _key, string _hash) {
		logger = _logger;
		Port = _port;
		Key = _key;
		Hash = _hash;

		server = new SshServer(new StartingInfo(System.Net.IPAddress.IPv6Any, Port, "SSH-2.0-TNBU"));
		server.AddHostKey("ssh-rsa", Key);
		server.ConnectionAccepted += ConnectionAccepted;
		server.Start();
	}

	private void ConnectionAccepted(object? sender, Session e) {
		logger.LogInformation("Connection accepted");
		e.ServiceRegistered += ServiceRegistered;
	}

	private void ServiceRegistered(object? sender, SshService e) {
		var session = (Session)sender!;
		logger.LogInformation("Session {session} requesting {type}", Convert.ToHexString(session.SessionId), e.GetType().Name);
		if(e is UserauthService userauthService) {
			userauthService.Userauth += UserAuth;
		} else if(e is ConnectionService connectionService) {
			connectionService.CommandOpened += CommandOpened;
		}
	}

	private void UserAuth(object? sender, UserauthArgs e) {
		var session = e.Session;
		logger.LogInformation("Session {session} authenticated as {user}:{pw}", Convert.ToHexString(session.SessionId), e.Username, e.Password);
		if(Owner == null) {
			throw new NullReferenceException(nameof(Owner));
		}
		e.Result = Owner.HandleSSHAuth(e.Username, e.Password);
	}

	private void CommandOpened(object? sender, CommandRequestedArgs e) {
		var session = e.AttachedUserauthArgs.Session;
		logger.LogInformation("Channel {channel} in session {session} runs {st}: \"{ct}\".", e.Channel.ServerChannelId, Convert.ToHexString(session.SessionId).ToLowerInvariant(), e.ShellType, e.CommandText);
		if(e.ShellType == "shell") {
			throw new NotImplementedException("Shell");
		} else if(e.ShellType == "exec") {
			if(Owner == null) {
				throw new NullReferenceException(nameof(Owner));
			}
			var (response, exitcode) = Owner.HandleSSHExec(e.CommandText, e.AttachedUserauthArgs.Username, e.AttachedUserauthArgs.Password);
			e.Channel.SendData(Encoding.ASCII.GetBytes(response));
			e.Channel.SendClose((uint)exitcode);
		} else if(e.ShellType == "subsystem") {
			throw new NotImplementedException("Subsystem");
		}
	}

	public void Dispose() {
		GC.SuppressFinalize(this);
		server.Stop();
		server.Dispose();
	}
}
