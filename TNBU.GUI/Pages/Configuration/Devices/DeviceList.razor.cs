using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using TNBU.GUI.Models;
using TNBU.GUI.Services;

namespace TNBU.GUI.Pages.Configuration.Devices {
	public partial class DeviceList : IDisposable {
		[Inject] public DeviceManagerService DeviceManager { get; set; } = null!;
		[Inject] public IToastService ToastService { get; set; } = null!;

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

		async Task Adopt(Device device) {
			try {
				await DeviceManager.Adopt(device);
				ToastService.ShowSuccess("Adoption request sent!");
			} catch(Exception ex) {
				ToastService.ShowError(ex.Message);
			}
		}
	}
}
