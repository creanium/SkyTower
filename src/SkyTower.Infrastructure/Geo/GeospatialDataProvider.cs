using Ardalis.Result;
using SkyTower.Core.Interfaces;
using GeoTimeZone;
using Microsoft.Extensions.Logging;
using SkyTower.Core.Extensions;
using TimeZoneConverter;

namespace SkyTower.Infrastructure.Geo;

public class GeospatialDataProvider(ILogger<GeospatialDataProvider> logger) : IGeospatialDataProvider
{
	public Result<TimeZoneInfo> GetTimeZoneInfo(double latitude, double longitude, CancellationToken cancellationToken = default)
	{
		var lookupResult = TimeZoneLookup.GetTimeZone(latitude, longitude);

		if (lookupResult.Result.IsNullOrWhiteSpace())
		{
			return Result.NotFound();
		}
		
		try
		{
			var timeZoneInfo = TZConvert.GetTimeZoneInfo(lookupResult.Result);
			return Result.Success(timeZoneInfo);
		}
		catch (TimeZoneNotFoundException nfEx)
		{
#pragma warning disable CA1848
			logger.LogError(nfEx, "'{TimeZoneId}' could not be found", lookupResult.Result);
			return Result.NotFound();
		}
		catch (InvalidTimeZoneException tzEx)
		{
			logger.LogError(tzEx, "Failed to get TimeZoneInfo for timezone ID '{TimeZoneId}'", lookupResult.Result);
			return Result.Error(tzEx.Message);
		}
#pragma warning restore CA1848
	}
}