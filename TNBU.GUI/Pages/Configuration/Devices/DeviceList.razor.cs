using Microsoft.AspNetCore.Components;
using MudBlazor;
using TNBU.GUI.Models;
using TNBU.GUI.Services;

namespace TNBU.GUI.Pages.Configuration.Devices {
	public partial class DeviceList : IDisposable{
		[Inject] public DeviceManagerService DeviceManager { get; set; } = null!;
		[Inject] public ISnackbar Snackbar { get; set; } = null!;

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
				Snackbar.Add("Adoption request sent!", Severity.Success);
			}catch(Exception ex) {
				Snackbar.Add(ex.Message, Severity.Error);
			}
		}
	}
}
