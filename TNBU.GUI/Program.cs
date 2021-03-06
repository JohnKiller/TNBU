using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using TNBU.GUI.EF;

namespace TNBU.GUI {
	public class Program {
		public static void Main(string[] args) {
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddRazorPages();
			builder.Services.AddServerSideBlazor();
			builder.Services.AddMudServices();
			builder.Services.AddDbContextFactory<DB>(opt => DB.ConfigureBuilder(opt));

			var app = builder.Build();

			using(var scope = app.Services.CreateScope()) {
				var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<DB>>();
				using var db = dbFactory.CreateDbContext();
				db.InitDB();
			}

			app.UseStaticFiles();

			app.UseRouting();

			app.MapBlazorHub();
			app.MapFallbackToPage("/_Host");

			app.Run();
		}
	}
}
