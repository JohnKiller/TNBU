namespace TNBU.GUI.Models {
	public class PhysicalRadio {
		public string Name { get; }
		public bool Is11AC { get; }

		public PhysicalRadio(string name, bool is11AC) {
			Name = name;
			Is11AC = is11AC;
		}
	}
}
