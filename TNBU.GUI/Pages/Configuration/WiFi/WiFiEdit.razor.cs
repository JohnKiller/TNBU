using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using TNBU.GUI.Components.Dialogs;
using TNBU.GUI.EF;
using TNBU.GUI.EF.Models;

namespace TNBU.GUI.Pages.Configuration.WiFi {
	public partial class WiFiEdit : IDisposable {
		[Inject] public IDbContextFactory<DB> DBS { get; set; } = null!;
		[Inject] public NavigationManager NavigationManager { get; set; } = null!;
		[Inject] public IDialogService DialogService { get; set; } = null!;
		[Inject] public ISnackbar Snackbar { get; set; } = null!;

		[Parameter] public long ID { get; set; }

		private DB dbContext = null!;
		private WiFiNetwork row = new();
		private bool IsFormValid;

		protected override void OnParametersSet() {
			base.OnParametersSet();
			if(ID != 0) {
				row = dbContext.WiFiNetworks.Where(x => x.ID == ID).First();
			}
		}

		protected override void OnInitialized() {
			base.OnInitialized();
			dbContext = DBS.CreateDbContext();
		}

		void OnSave(MouseEventArgs e) {
			if(ID == 0) {
				dbContext.WiFiNetworks.Add(row);
			}
			dbContext.SaveChanges();
			Snackbar.Add($"WiFi network \"{row.SSID}\" saved", Severity.Success);
			NavigationManager.NavigateTo("/configuration/wifi");
		}

		async void OnDelete(MouseEventArgs e) {
			var parameters = new DialogParameters {
				{ "ContentText", $"Do you really want to delete \"{row.SSID}\"?" },
			};

			var options = new DialogOptions() {
				CloseButton = true,
				MaxWidth = MaxWidth.ExtraSmall,
			};

			var dialog = DialogService.Show<DeleteDialog>("Delete", parameters, options);
			var result = await dialog.Result;
			if(!result.Cancelled) {
				dbContext.WiFiNetworks.Remove(row);
				dbContext.SaveChanges();
				Snackbar.Add($"WiFi network \"{row.SSID}\" deleted", Severity.Success);
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
