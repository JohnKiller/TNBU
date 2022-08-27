using System.Buffers.Binary;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using TNBU.Core.Utils;

namespace TNBU.Core.Models;

public class DiscoveryPacket {
	//TODO: convert to enum
	public const byte DISCOVERYTYPE_DISCOVERY = 0x06;

	public const byte PAYLOAD_MAC = 0x01;
	public const byte PAYLOAD_MACIP = 0x02;
	public const byte PAYLOAD_FIRMWARE = 0x03;
	public const byte PAYLOAD_UPTIME = 0x0a;
	public const byte PAYLOAD_HOSTNAME = 0x0b;
	public const byte PAYLOAD_SHORTMODEL2 = 0x0c;
	public const byte PAYLOAD_SEQUENCE = 0x12;
	public const byte PAYLOAD_SERIAL = 0x13;
	public const byte PAYLOAD_SHORTMODEL = 0x15;
	public const byte PAYLOAD_VERSION = 0x16;
	public const byte PAYLOAD_IS_DEFAULT = 0x17;
	public const byte PAYLOAD_IS_LOCATING = 0x18;
	public const byte PAYLOAD_IS_DHCP = 0x19;
	public const byte PAYLOAD_IS_DHCP_BOUND = 0x20;
	public const byte PAYLOAD_SSHPORT = 0x1c;

	public const byte PAYLOAD_IS_PROBE = 0x10; //not sure

	public byte Version { get; set; }
	public byte DiscoveryType { get; set; }
	public Dictionary<byte, byte[]> Payloads { get; } = new();
	public Dictionary<byte, List<byte[]>> PayloadsMulti { get; } = new();

	public PhysicalAddress Mac => GetPayloadAsMacIp(PAYLOAD_MACIP).Mac;
	public IPAddress IP => GetPayloadAsMacIp(PAYLOAD_MACIP).IP;
	public string Model {
		get {
			if(Payloads.ContainsKey(PAYLOAD_SHORTMODEL)) {
				return GetPayloadAsString(PAYLOAD_SHORTMODEL);
			}
			return GetPayloadAsString(PAYLOAD_SHORTMODEL2);
		}
	}
	public string HostName => GetPayloadAsString(PAYLOAD_HOSTNAME);
	public string? FirmwareVersion {
		get {
			if(Payloads.ContainsKey(PAYLOAD_VERSION)) {
				return GetPayloadAsString(PAYLOAD_VERSION);
			}
			return null;
		}
	}
	public bool IsProbe => Payloads.ContainsKey(PAYLOAD_IS_PROBE);
	public bool IsDefault => Payloads.ContainsKey(PAYLOAD_IS_DEFAULT) && GetPayloadAsBoolean(PAYLOAD_IS_DEFAULT);

	private static readonly Socket udpClient = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

	public static DiscoveryPacket Decode(byte[] data) {
		var ret = new DiscoveryPacket {
			Version = data[0],
			DiscoveryType = data[1]
		};
		var totLength = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(2, 3)) + 4;
		if(totLength != data.Length) {
			throw new Exception("Invalid length");
		}
		var index = 4;
		while(index < data.Length) {
			var key = data[index];
			var length = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(index + 1, 2));
			var start = index + 3;
			var end = start + length;
			var pdata = data[start..end];
			if(!ret.Payloads.ContainsKey(key)) {
				ret.Payloads[key] = pdata;
			}
			if(!ret.PayloadsMulti.ContainsKey(key)) {
				ret.PayloadsMulti[key] = new();
			}
			ret.PayloadsMulti[key].Add(pdata);
			index += length + 3;
		}
		return ret;
	}

	public byte[] Encode() {
		var payret = new List<byte>();
		foreach(var (type, payload) in Payloads) {
			payret.Add(type);
			payret.AddRange(BigEndianReader.GetUShortBytes((ushort)payload.Length));
			payret.AddRange(payload);
		}
		var ret = new List<byte>
		{
			Version,
			DiscoveryType
		};
		ret.AddRange(BigEndianReader.GetUShortBytes((ushort)payret.Count));
		ret.AddRange(payret);
		return ret.ToArray();
	}

	public void Broadcast() {
		var (ip, netmask) = NetworkUtils.GetMyIPOnThisSubnet(IP);
		var broadcastIP = ip.GetBroadcastAddress(netmask);
		var broadcastEP = new IPEndPoint(broadcastIP, 10001);
		udpClient.SendTo(Encode(), broadcastEP);
	}

	public override string ToString() {
		var type = DiscoveryType switch {
			DISCOVERYTYPE_DISCOVERY => "Discovery",
			_ => $"Unknown ({DiscoveryType})"
		};
		return
			$"Type: {type}\n" +
			$"Model: {Model}\n" +
			$"IP: {IP}\n" +
			$"MAC: {Mac}";
	}

	public string GetPayloadAsString(byte key) {
		return Encoding.UTF8.GetString(Payloads[key]);
	}

	public void SetPayloadAsString(byte key, string value) {
		Payloads[key] = Encoding.UTF8.GetBytes(value);
	}

	public PhysicalAddress GetPayloadAsMac(byte key) {
		return new PhysicalAddress(Payloads[key]);
	}

	public void SetPayloadAsMac(byte key, PhysicalAddress mac) {
		Payloads[key] = mac.GetAddressBytes();
	}

	public (PhysicalAddress Mac, IPAddress IP) GetPayloadAsMacIp(byte key) {
		var payload = Payloads[key];
		var mac = new PhysicalAddress(payload[0..6]);
		var ip = new IPAddress(payload[6..10]);
		return (mac, ip);
	}

	public void SetPayloadAsMacIp(byte key, PhysicalAddress mac, IPAddress ip) {
		Payloads[key] = mac.GetAddressBytes().Concat(ip.GetAddressBytes()).ToArray();
	}

	public ushort GetPayloadAsUShort(byte key) {
		return BinaryPrimitives.ReadUInt16BigEndian(Payloads[key]);
	}

	public void SetPayloadAsUShort(byte key, ushort data) {
		Payloads[key] = BigEndianReader.GetUShortBytes(data);
	}

	public uint GetPayloadAsUInt(byte key) {
		return BinaryPrimitives.ReadUInt32BigEndian(Payloads[key]);
	}

	public void SetPayloadAsUInt(byte key, uint data) {
		Payloads[key] = BigEndianReader.GetUIntBytes(data);
	}

	public bool GetPayloadAsBoolean(byte key) {
		return Payloads[key][0] != 0;
	}

	public void SetPayloadAsBoolean(byte key, bool data) {
		Payloads[key] = new byte[] { (byte)(data ? 0x01 : 0x00) };
	}
}
