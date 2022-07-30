using Microsoft.AspNetCore.Components;
using TNBU.GUI.Services;

namespace TNBU.GUI.Pages.Configuration.Devices {
	public partial class DeviceList {
		[Inject] public DeviceManagerService DeviceManager { get; set; } = null!;
	}
}
