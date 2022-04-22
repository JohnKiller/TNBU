using System.Buffers.Binary;

namespace TNBU.Core.Utils;

public class BigEndianReader : BinaryReader {
	public BigEndianReader(Stream input) : base(input) { }

	public override short ReadInt16() {
		return BinaryPrimitives.ReadInt16BigEndian(ReadBytes(sizeof(short)));
	}

	public override int ReadInt32() {
		return BinaryPrimitives.ReadInt32BigEndian(ReadBytes(sizeof(int)));
	}

	public override long ReadInt64() {
		return BinaryPrimitives.ReadInt64BigEndian(ReadBytes(sizeof(long)));
	}

	public override ushort ReadUInt16() {
		return BinaryPrimitives.ReadUInt16BigEndian(ReadBytes(sizeof(ushort)));
	}

	public override uint ReadUInt32() {
		return BinaryPrimitives.ReadUInt32BigEndian(ReadBytes(sizeof(uint)));
	}

	public override ulong ReadUInt64() {
		return BinaryPrimitives.ReadUInt64BigEndian(ReadBytes(sizeof(ulong)));
	}

	public static byte[] GetUShortBytes(ushort len) {
		var ret = new byte[sizeof(ushort)];
		BinaryPrimitives.WriteUInt16BigEndian(ret, len);
		return ret;
	}
}
