using System.Net.Mail;
using System.Reflection;
using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using SkyTower.Infrastructure.Data.ValueConverters;
using StrictId.EFCore;

namespace SkyTower.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		Guard.Against.Null(modelBuilder);
		
		base.OnModelCreating(modelBuilder);
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
	}
	
	protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
	{
		Guard.Against.Null(configurationBuilder);
		
		base.ConfigureConventions(configurationBuilder);

		configurationBuilder.ConfigureStrictId();

		configurationBuilder.Properties<TimeZoneInfo>()
			.HaveConversion<TimeZoneInfoConverter>();
		
		configurationBuilder.Properties<MailAddress>()
			.HaveConversion<MailAddressConverter>();
	}
}