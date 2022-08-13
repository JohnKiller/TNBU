using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using TNBU.GUI.EF;

namespace TNBU.GUI.Pages.Configuration.WiFi {
	public partial class WiFiList : IDisposable {
		[Inject] public IDbContextFactory<DB> DBS { get; set; } = null!;
		[Inject] public NavigationManager NavigationManager { get; set; } = null!;

		private DB? dbContext;

		protected override void OnInitialized() {
			base.OnInitialized();
			dbContext = DBS.CreateDbContext();
		}

		public void Dispose() {
			GC.SuppressFinalize(this);
			if(dbContext != null) {
				dbContext.Dispose();
			}
		}
	}
}
