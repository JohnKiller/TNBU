#pragma warning disable IDE1006
namespace TNBU.GUI.Services.FirmwareUpdate {
	public class SrvFirmwareJson {
		public _Embedded _embedded { get; set; }

		public class _Embedded {
			public Firmware[] firmware { get; set; }
		}

		public class Firmware {
			public string md5 { get; set; }
			public string platform { get; set; }
			public int version_major { get; set; }
			public int version_minor { get; set; }
			public int version_patch { get; set; }
			public string version_build { get; set; }
			public _Links _links { get; set; }
		}

		public class _Links {
			public Data data { get; set; }
		}

		public class Data {
			public string href { get; set; }
		}
	}
}
#pragma warning restore IDE1006
