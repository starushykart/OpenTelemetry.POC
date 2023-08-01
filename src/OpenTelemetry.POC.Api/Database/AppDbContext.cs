using Microsoft.EntityFrameworkCore;

namespace OpenTelemetry.POC.Api.Database;

public sealed class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options)
		: base(options)
	{
		Database.EnsureCreated();
	}

	public DbSet<TestModel> TesModels => Set<TestModel>();
}