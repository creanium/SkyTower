using Ardalis.Result;
using GeoTimeZone;
using Microsoft.Extensions.Logging;
using SkyTower.Domain.Extensions;
using SkyTower.Domain.Locations;
using TimeZoneConverter;

namespace SkyTower.Infrastructure.Geospatial;

public class TimeZoneDataProvider(ILogger<TimeZoneDataProvider> logger) : ITimeZoneDataProvider
{
	public Result<TimeZoneInfo> GetTimeZoneInfo(double latitude, double longitude)
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
#pragma warning disable CA1848 LoggerMessage delegates don't work well with exceptions 
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