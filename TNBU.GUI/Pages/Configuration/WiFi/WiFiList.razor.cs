using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using TNBU.GUI.EF;
using TNBU.GUI.EF.Models;

namespace TNBU.GUI.Pages.Configuration.WiFi {
	public partial class WiFiList {
		[Inject] public IDbContextFactory<DB> DBS { get; set; } = null!;
		[Inject] public NavigationManager NavigationManager { get; set; } = null!;

		private DB? dbContext;

		protected override void OnInitialized() {
			base.OnInitialized();
			dbContext = DBS.CreateDbContext();
		}

		void OnRowClick(TableRowClickEventArgs<WiFiNetwork> e) {
			NavigationManager.NavigateTo("/configuration/wifi/edit/" + e.Item.ID);
		}
	}
}
