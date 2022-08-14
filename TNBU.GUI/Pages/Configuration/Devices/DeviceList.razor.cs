using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using TNBU.GUI.Models;
using TNBU.GUI.Services;

namespace TNBU.GUI.Pages.Configuration.Devices {
	public partial class DeviceList : IDisposable {
		[Inject] public DeviceManagerService DeviceManager { get; set; } = null!;
		[Inject] public IToastService ToastService { get; set; } = null!;

		public IEnumerable<Device> DevicesToAdopt => DeviceManager.Devices.Values.Where(x => !x.IsAdopted);
		public IEnumerable<Device> DevicesAdopted => DeviceManager.Devices.Values.Where(x => x.IsAdopted);

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

		async Task AdoptAll() {
			foreach(var d in DevicesToAdopt.Where(x => x.IsAdoptable)) {
				await Adopt(d);
			}
		}

		async Task Adopt(Device device) {
			try {
				await DeviceManager.Adopt(device);
				ToastService.ShowSuccess($"Adoption request sent to {device.ModelDisplay}!");
			} catch(Exception ex) {
				ToastService.ShowError(ex.Message);
			}
		}
	}
}
