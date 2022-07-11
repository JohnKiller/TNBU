using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TNBU.GUI.EF.Models;

namespace TNBU.GUI.EF {
	public class DB : DbContext {
		public DbSet<WiFiNetwork> WiFiNetworks { get; set; } = null!;

		//Empty constructor required for D.I.
		public DB(DbContextOptions options) : base(options) { }

		public static void ConfigureBuilder(DbContextOptionsBuilder opt) {
			opt.UseSqlite("Data Source=Application.db")
				.EnableSensitiveDataLogging()
				.EnableDetailedErrors();
		}

		public void InitDB() {
			Database.EnsureDeleted();
			Database.EnsureCreated();
			//Database.Migrate();
		}
	}

	public class MigrationsDBContextFactory : IDesignTimeDbContextFactory<DB> {
		public DB CreateDbContext(string[] args) {
			var opt = new DbContextOptionsBuilder<DB>();
			DB.ConfigureBuilder(opt);
			return new DB(opt.Options);
		}
	}
}
