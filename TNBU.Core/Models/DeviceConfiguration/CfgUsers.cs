namespace TNBU.Core.Models.DeviceConfiguration {
	public class CfgUsers {
		public override string ToString() {
			return $@"
users.status=enabled
users.1.name=admin
users.1.password=$6$z2NqRZE8aT.LtBEL$3uPEJ2jHxDA9m.smPwchZVrThfwP0Y1LAe8XKuC47vV9L9o9z2Gyese0ahP971bOuLuHvaRIy7mBEhKAuyx9B0
users.1.status=enabled
users.2.name=nobody
users.2.password=x
users.2.shell=/bin/false
users.2.status=enabled
";
		}
	}
}
