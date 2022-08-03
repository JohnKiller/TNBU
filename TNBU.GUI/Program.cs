using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using TNBU.GUI.EF;
using TNBU.GUI.Services;
using TNBU.GUI.Services.ConfigurationBuilder;

namespace TNBU.GUI {
	public class Program {
		public static void Main(string[] args) {
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddControllers();
			builder.Services.AddRazorPages();
			builder.Services.AddServerSideBlazor();
			builder.Services.AddMudServices();
			builder.Services.AddDbContextFactory<DB>(opt => DB.ConfigureBuilder(opt));

			builder.Services.AddSingleton<DeviceManagerService>();
			builder.Services.AddSingleton<ConfigurationBuilderService>();
			builder.Services.AddSingleton<IHostedService, DiscoveryService>();

			var app = builder.Build();

			using(var scope = app.Services.CreateScope()) {
				var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<DB>>();
				using var db = dbFactory.CreateDbContext();
				db.InitDB();
			}

			app.UseStaticFiles();

			app.UseRouting();

			app.MapControllers();
			app.MapBlazorHub();
			app.MapFallbackToPage("/_Host");

			app.Run("http://0.0.0.0:8081/");
		}
	}
}
