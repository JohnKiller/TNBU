namespace TNBU.Core.Utils;

public static class HexConversions {
	public static string BytesToColon(this byte[] bytes) {
		return string.Join(":", Array.ConvertAll(bytes, b => b.ToString("x2")));
	}
}
