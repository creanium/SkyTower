using Microsoft.Extensions.DependencyInjection;
using SkyTower.Domain.Locations;
using SkyTower.Infrastructure.Geo;

namespace SkyTower.Infrastructure;

public static class InfrastructureServiceExtensions
{
	public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
	{
		services.AddTransient<ITimeZoneDataProvider, TimeZoneDataProvider>();
		
		return services;
	}
}