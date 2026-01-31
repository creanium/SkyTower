using Ardalis.Result;

namespace SkyTower.Core.Interfaces;

public interface IGeospatialDataProvider
{
	/// <summary>
	/// Gets the TimeZoneInfo for the specified latitude and longitude.
	/// </summary>
	/// <param name="latitude">Latitude for the point on Earth.</param>
	/// <param name="longitude">Longitude for the point on Earth.</param>
	/// <param name="cancellationToken"></param>
	/// <returns>The result of the lookup</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Thrown if <paramref name="latitude" /> is not between -90.0 and 90.0,
	/// or <paramref name="longitude" /> is not between -180 and 180.
	/// </exception>
	Result<TimeZoneInfo> GetTimeZoneInfo(double latitude, double longitude, CancellationToken cancellationToken = default);
}