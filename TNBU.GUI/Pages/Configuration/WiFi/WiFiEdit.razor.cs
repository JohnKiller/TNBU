using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using TNBU.GUI.EF;
using TNBU.GUI.EF.Models;

namespace TNBU.GUI.Pages.Configuration.WiFi {
	public partial class WiFiEdit : IDisposable {
		[Inject] public IDbContextFactory<DB> DBS { get; set; } = null!;
		[Inject] public NavigationManager NavigationManager { get; set; } = null!;

		[Parameter] public long ID { get; set; }

		private DB dbContext = null!;
		private WiFiNetwork row = new();

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
			NavigationManager.NavigateTo("/configuration/wifi");
		}

		void OnDelete(MouseEventArgs e) {
			dbContext.WiFiNetworks.Remove(row);
			dbContext.SaveChanges();
			NavigationManager.NavigateTo("/configuration/wifi");
		}

		public void Dispose() {
			GC.SuppressFinalize(this);
			if(dbContext != null) {
				dbContext.Dispose();
			}
		}
	}
}
