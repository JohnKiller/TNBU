using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using TNBU.Core.Models;
using TNBU.MitM.Utils;

namespace TNBU.MitM.Models;

public class DeviceRelay {
	public PhysicalAddress Mac { get; }
	public PhysicalAddress FakeMac { get; }
	public IPAddress IP { get; }
	public IPAddress FakeIP { get; }
	public IPAddress Netmask { get; }
	public IPAddress BroadcastIP { get; }
	public IPEndPoint BroadcastEP { get; }

	private readonly Socket udpClient = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

	public DeviceRelay(DiscoveryPacket dp) {
		Mac = dp.Mac;
		FakeMac = NetworkUtils.GetFakeMacAddress(Mac);
		IP = dp.IP;
		(FakeIP, Netmask) = NetworkUtils.GetMyIPOnThisSubnet(IP);
		BroadcastIP = FakeIP.GetBroadcastAddress(Netmask);
		BroadcastEP = new(BroadcastIP, 10001);
	}

	public void ReplayDiscovery(DiscoveryPacket dp) {
		dp.SetPayloadAsMac(DiscoveryPacket.PAYLOAD_MAC, FakeMac);
		dp.SetPayloadAsMacIp(DiscoveryPacket.PAYLOAD_MACIP, FakeMac, FakeIP);
		dp.SetPayloadAsMac(DiscoveryPacket.PAYLOAD_SERIAL, FakeMac);
		udpClient.SendTo(dp.Encode(), BroadcastEP);
	}
}
