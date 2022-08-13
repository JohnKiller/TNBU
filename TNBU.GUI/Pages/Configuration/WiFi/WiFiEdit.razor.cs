using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using TNBU.GUI.EF;
using TNBU.GUI.EF.Models;
using TNBU.GUI.Services;
using TNBU.GUI.Shared;

namespace TNBU.GUI.Pages.Configuration.WiFi {
	public partial class WiFiEdit : IDisposable {
		[Inject] public IDbContextFactory<DB> DBS { get; set; } = null!;
		[Inject] public NavigationManager NavigationManager { get; set; } = null!;
		[Inject] public IToastService ToastService { get; set; } = null!;
		[Inject] public MessageService MessageService { get; set; } = null!;

		[Parameter] public long ID { get; set; }

		private DB dbContext = null!;
		private EditContext? FormContext;
		private WiFiNetwork row = new();

		protected override void OnParametersSet() {
			base.OnParametersSet();
			if(ID != 0) {
				row = dbContext.WiFiNetworks.Where(x => x.ID == ID).First();
			}
			FormContext = new(row);
			FormContext.SetFieldCssClassProvider(new TablerFieldCssClassProvider());
		}

		protected override void OnInitialized() {
			base.OnInitialized();
			dbContext = DBS.CreateDbContext();
		}

		void OnSave() {
			if(ID == 0) {
				dbContext.WiFiNetworks.Add(row);
			}
			dbContext.SaveChanges();
			ToastService.ShowSuccess($"WiFi network \"{row.SSID}\" saved");
			NavigationManager.NavigateTo("/configuration/wifi");
		}

		async void OnDelete() {
			var ret = await MessageService.ShowDanger("Confirm deletion", $"Do you really want to delete \"{row.SSID}\"?", MessageService.MessageButton.YesNo, MessageService.MessageButton.Yes);
			if(ret == MessageService.MessageButton.Yes) {
				dbContext.WiFiNetworks.Remove(row);
				dbContext.SaveChanges();
				ToastService.ShowSuccess($"WiFi network \"{row.SSID}\" deleted");
				NavigationManager.NavigateTo("/configuration/wifi");
			}
		}

		public void Dispose() {
			GC.SuppressFinalize(this);
			if(dbContext != null) {
				dbContext.Dispose();
			}
		}
	}
}
