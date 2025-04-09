using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SkyTower.Infrastructure.Data;

public static class AppDbContextExtensions
{
	public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, string connectionString) =>
		services.AddDbContext<AppDbContext>(options =>
		{
			options.UseNpgsql(connectionString, x => x.UseNetTopologySuite());
		});
}