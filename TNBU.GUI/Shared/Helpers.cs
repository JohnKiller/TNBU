using System.ComponentModel;
using System.Globalization;

namespace TNBU.GUI.Shared {
	public static class Helpers {
		public static string GetDescription<T>(this T e) where T : IConvertible {
			if(e is Enum) {
				Type type = e.GetType();
				var val = e.ToInt32(CultureInfo.InvariantCulture);
				var fieldName = type.GetEnumName(val);
				if(fieldName != null) {
					var fieldInfo = type.GetField(fieldName)!;
					if(Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute)) is DescriptionAttribute attr) {
						return attr.Description;
					}
					return fieldName;
				}
			}
			return string.Empty;
		}

		public static bool HasDescription<T>(this T e) where T : IConvertible {
			if(e is Enum) {
				Type type = e.GetType();
				var val = e.ToInt32(CultureInfo.InvariantCulture);
				var fieldName = type.GetEnumName(val);
				if(fieldName != null) {
					var fieldInfo = type.GetField(fieldName)!;
					if(Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute)) is DescriptionAttribute) {
						return true;
					}
				}
			}
			return false;
		}
	}
}
