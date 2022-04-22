using IronSnappy;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using System.Buffers.Binary;
using System.IO.Compression;
using System.Net.NetworkInformation;
using System.Text;
using TNBU.Core.Utils;

namespace TNBU.Core.Models;

public class InformPacket {
	public const string DEFAULT_KEY = "ba86f2bbe107c7c57eb5f2690775c712"; //md5("ubnt")

	const string HEADER_MAGIC = "TNBU";

	const ushort FLAG_ENC_AES = 1;
	const ushort FLAG_COMP_ZLIB = 2;
	const ushort FLAG_COMP_SNAPPY = 4;
	const ushort FLAG_ENC_GCM = 8;
	
	public uint Version { get; set; }
	public uint PayloadVersion { get; set; }
	public PhysicalAddress MACAddress { get; set; } = null!;
	public bool IsAES { get; set; }
	public bool IsZLIB { get; set; }
	public bool IsSnappy { get; set; }
	public bool IsGCM { get; set; }
	public byte[] IV { get; set; } = null!;
	public string Body { get; set; } = null!;
	
	private byte[] aad = null!;
	private byte[] payload = null!;

	public static InformPacket Decode(Stream data) {
		var ret = new InformPacket();
		using var source = new BigEndianReader(data);
		ret.aad = source.ReadBytes(40);
		source.BaseStream.Position = 0;
		if(Encoding.ASCII.GetString(source.ReadBytes(4)) != HEADER_MAGIC) {
			throw new Exception("Invalid packet header");
		}
		ret.Version = source.ReadUInt32();
		ret.MACAddress = new PhysicalAddress(source.ReadBytes(6));
		var flagmask = source.ReadUInt16();
		ret.IsAES = (flagmask & FLAG_ENC_AES) == FLAG_ENC_AES;
		ret.IsZLIB = (flagmask & FLAG_COMP_ZLIB) == FLAG_COMP_ZLIB;
		ret.IsSnappy = (flagmask & FLAG_COMP_SNAPPY) == FLAG_COMP_SNAPPY;
		ret.IsGCM = (flagmask & FLAG_ENC_GCM) == FLAG_ENC_GCM;
		ret.IV = source.ReadBytes(16);
		ret.PayloadVersion = source.ReadUInt32();
		var payloadSize = source.ReadUInt32();
		ret.payload = source.ReadBytes((int)payloadSize);
		return ret;
	}

	public void Decrypt(string key = DEFAULT_KEY) {
		byte[] plaintextBytes;
		if(IsAES) {
			var keybytes = Convert.FromHexString(key);
			if(IsGCM) {
				plaintextBytes = new byte[payload.Length - keybytes.Length];
				var cipher = new GcmBlockCipher(new AesEngine());
				var parameters = new AeadParameters(new KeyParameter(keybytes), keybytes.Length * 8, IV, aad);
				cipher.Init(false, parameters);
				var offset = cipher.ProcessBytes(payload, 0, payload.Length, plaintextBytes, 0);
				cipher.DoFinal(plaintextBytes, offset);
			} else {
				plaintextBytes = new byte[payload.Length];
				var cipher = new CbcBlockCipher(new AesEngine());
				var parameters = new ParametersWithIV(new KeyParameter(keybytes), IV);
				cipher.Init(false, parameters);
				var offset = 0;
				while(offset < payload.Length) {
					offset += cipher.ProcessBlock(payload, offset, plaintextBytes, offset);
				}
				var padding = new Pkcs7Padding();
				plaintextBytes = plaintextBytes.Take(plaintextBytes.Length - padding.PadCount(plaintextBytes)).ToArray();
			}
		} else {
			plaintextBytes = payload;
		}
		if(IsZLIB) {
			using var decompressor = new ZLibStream(new MemoryStream(plaintextBytes), CompressionMode.Decompress);
			using var decompressed = new MemoryStream();
			decompressor.CopyTo(decompressed);
			Body = Encoding.UTF8.GetString(decompressed.ToArray());
		} else if(IsSnappy) {
			var decompressed = Snappy.Decode(plaintextBytes);
			Body = Encoding.UTF8.GetString(decompressed.ToArray());
		} else {
			Body = Encoding.UTF8.GetString(plaintextBytes);
		}
	}

	public byte[] Encode(string key = DEFAULT_KEY) {
		aad = new byte[40];
		Encoding.ASCII.GetBytes(HEADER_MAGIC).CopyTo(aad, 0);
		BinaryPrimitives.WriteUInt32BigEndian(aad.AsSpan(4), Version);
		MACAddress.GetAddressBytes().CopyTo(aad, 8);
		ushort flagmask = 0;
		if(IsAES) {
			flagmask |= FLAG_ENC_AES;
		}
		if(IsZLIB) {
			flagmask |= FLAG_COMP_ZLIB;
		}
		if(IsSnappy) {
			flagmask |= FLAG_COMP_SNAPPY;
		}
		if(IsGCM) {
			flagmask |= FLAG_ENC_GCM;
		}
		BinaryPrimitives.WriteUInt16BigEndian(aad.AsSpan(14), flagmask);
		IV.CopyTo(aad, 16);
		BinaryPrimitives.WriteUInt32BigEndian(aad.AsSpan(32), PayloadVersion);
		var plaintextBytes = Encoding.UTF8.GetBytes(Body);
		byte[] compressed;
		if(IsZLIB) {
			using var uncompressed = new MemoryStream(plaintextBytes);
			using var compressedStream = new MemoryStream();
			using var compressor = new ZLibStream(compressedStream, CompressionMode.Compress, true);
			uncompressed.CopyTo(compressor);
			compressor.Flush();
			compressor.Close();
			compressed = compressedStream.ToArray();
		} else if(IsSnappy) {
			compressed = Snappy.Encode(plaintextBytes);
		} else {
			compressed = plaintextBytes;
		}
		if(IsAES) {
			var keybytes = Convert.FromHexString(key);
			if(IsGCM) {
				payload = new byte[compressed.Length + keybytes.Length];
				BinaryPrimitives.WriteUInt32BigEndian(aad.AsSpan(36), (uint)payload.Length);
				var cipher = new GcmBlockCipher(new AesEngine());
				var parameters = new AeadParameters(new KeyParameter(keybytes), keybytes.Length * 8, IV, aad);
				cipher.Init(true, parameters);
				var offset = cipher.ProcessBytes(compressed, 0, compressed.Length, payload, 0);
				cipher.DoFinal(payload, offset);
			} else {
				var padding = new Pkcs7Padding();
				var cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new AesEngine()), padding);
				var parameters = new ParametersWithIV(new KeyParameter(keybytes), IV);
				cipher.Init(true, parameters);
				payload = new byte[cipher.GetOutputSize(compressed.Length)];
				BinaryPrimitives.WriteUInt32BigEndian(aad.AsSpan(36), (uint)payload.Length);
				var offset = cipher.ProcessBytes(compressed, payload, 0);
				cipher.DoFinal(payload, offset);
			}
		} else {
			payload = compressed;
			BinaryPrimitives.WriteUInt32BigEndian(aad.AsSpan(36), (uint)payload.Length);
		}
		var ret = new byte[aad.Length + payload.Length];
		aad.CopyTo(ret, 0);
		payload.CopyTo(ret, aad.Length);
		return ret;
	}
}
