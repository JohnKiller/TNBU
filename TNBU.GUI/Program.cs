using MudBlazor.Services;

namespace TNBU.GUI {
	public class Program {
		public static void Main(string[] args) {
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddRazorPages();
			builder.Services.AddServerSideBlazor();
			builder.Services.AddMudServices();

			var app = builder.Build();

			app.UseStaticFiles();

			app.UseRouting();

			app.MapBlazorHub();
			app.MapFallbackToPage("/_Host");

			app.Run();
		}
	}
}
