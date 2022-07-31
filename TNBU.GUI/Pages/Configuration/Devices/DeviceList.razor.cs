using Microsoft.AspNetCore.Components;
using TNBU.GUI.Services;

namespace TNBU.GUI.Pages.Configuration.Devices {
	public partial class DeviceList : IDisposable{
		[Inject] public DeviceManagerService DeviceManager { get; set; } = null!;

		protected override void OnInitialized() {
			DeviceManager.OnDeviceChange += DeviceManager_OnDeviceChange;
		}

		private void DeviceManager_OnDeviceChange(object? sender, EventArgs e) {
			InvokeAsync(() => StateHasChanged());
		}

		public void Dispose() {
			GC.SuppressFinalize(this);
			DeviceManager.OnDeviceChange -= DeviceManager_OnDeviceChange;
		}
	}
}
