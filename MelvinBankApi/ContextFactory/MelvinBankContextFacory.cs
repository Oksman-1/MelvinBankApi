using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Repository;

namespace MelvinBankApi.ContextFactory;

public class MelvinBankContextFacory : IDesignTimeDbContextFactory<MelvinBankContext>
{
	public MelvinBankContext CreateDbContext(string[] args)
	{
		var configuration = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json")
			.Build();

		var builder = new DbContextOptionsBuilder<MelvinBankContext>()
			.UseSqlServer(configuration.GetConnectionString("SQLConnectionString"),
			   b => b.MigrationsAssembly("MelvinBankApi"));

		return new MelvinBankContext(builder.Options);
	}
}
