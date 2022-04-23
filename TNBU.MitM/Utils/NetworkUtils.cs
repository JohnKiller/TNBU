using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace TNBU.MitM.Utils;
public static class NetworkUtils {
	public static bool IsFakeMac(PhysicalAddress mac) {
		var b = mac.GetAddressBytes();
		return b[0] == 0 && b[1] == 0 && b[2] == 0;
	}

	public static PhysicalAddress GetFakeMacAddress(PhysicalAddress mac) {
		var b = mac.GetAddressBytes();
		b[0] = 0;
		b[1] = 0;
		b[2] = 0;
		return new PhysicalAddress(b);
	}

	public static (IPAddress IP, IPAddress Netmask) GetMyIPOnThisSubnet(IPAddress otherIP) {
		foreach(var adapter in NetworkInterface.GetAllNetworkInterfaces()) {
			if(adapter.Supports(NetworkInterfaceComponent.IPv4) == false) {
				continue;
			}
			foreach(var ip in adapter.GetIPProperties().UnicastAddresses) {
				if(ip.Address.AddressFamily != AddressFamily.InterNetwork) {
					continue;
				}
				if(otherIP.IsInSameSubnet(ip.Address, ip.IPv4Mask)) {
					return (ip.Address, ip.IPv4Mask);
				}
			}
		}
		throw new Exception($"No interface available on same net as {otherIP}");
	}

	public static IPAddress GetBroadcastAddress(this IPAddress address, IPAddress subnetMask) {
		var ipAdressBytes = address.GetAddressBytes();
		var subnetMaskBytes = subnetMask.GetAddressBytes();
		var broadcastAddress = new byte[ipAdressBytes.Length];
		for(var i = 0; i < broadcastAddress.Length; i++) {
			broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
		}
		return new IPAddress(broadcastAddress);
	}

	public static IPAddress GetNetworkAddress(this IPAddress address, IPAddress subnetMask) {
		var ipAdressBytes = address.GetAddressBytes();
		var subnetMaskBytes = subnetMask.GetAddressBytes();
		var broadcastAddress = new byte[ipAdressBytes.Length];
		for(var i = 0; i < broadcastAddress.Length; i++) {
			broadcastAddress[i] = (byte)(ipAdressBytes[i] & (subnetMaskBytes[i]));
		}
		return new IPAddress(broadcastAddress);
	}

	public static bool IsInSameSubnet(this IPAddress address2, IPAddress address, IPAddress subnetMask) {
		var network1 = address.GetNetworkAddress(subnetMask);
		var network2 = address2.GetNetworkAddress(subnetMask);
		return network1.Equals(network2);
	}
}
