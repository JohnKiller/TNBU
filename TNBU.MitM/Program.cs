using Microsoft.AspNetCore;
using TNBU.MitM.Services;

namespace TNBU.MitM;

public class Program {
	public static void Main() {
		var wb = WebHost.CreateDefaultBuilder();
		wb.UseUrls("http://*:8081").UseStartup<Startup>().Build().Run();
	}
}

public class Startup {
	public void ConfigureServices(IServiceCollection services) {
		services.AddControllersWithViews();
		services.AddSingleton<DeviceManagerService>();
		services.AddSingleton<IHostedService, DiscoveryService>();
	}

	public void Configure(IApplicationBuilder app) {
		app.UseRouting();
		app.UseEndpoints(endpoints => {
			endpoints.MapControllers();
		});
	}
}
