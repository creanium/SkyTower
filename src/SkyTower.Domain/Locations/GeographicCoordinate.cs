using Ardalis.GuardClauses;
using NetTopologySuite.Geometries;
using SkyTower.Domain.Abstractions;

namespace SkyTower.Domain.Locations;

public sealed class GeographicCoordinate : ValueObject
{
	public double Latitude { get; init; }
	public double Longitude { get; init; }

	/// <summary>
	/// 
	/// </summary>
	/// <param name="latitude"></param>
	/// <param name="longitude"></param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="latitude"/> is outside the range of -90.0 to 90.0, or <paramref name="longitude"/> is outside the range -180.0 to 180.0.</exception>
	public GeographicCoordinate(double latitude, double longitude)
	{
		Latitude = Guard.Against.OutOfRange(latitude, nameof(latitude), -90.0, 90.0);
		Longitude = Guard.Against.OutOfRange(longitude, nameof(longitude), -180.0, 180.0);
	}

	public static bool TryCreate(double latitude, double longitude, out GeographicCoordinate? geographicCoordinate)
	{
		try
		{
			geographicCoordinate = new GeographicCoordinate(latitude, longitude);
			return true;
		}
		catch(ArgumentOutOfRangeException)
		{
			geographicCoordinate = null;
			return false;
		}
	}
	
	public static GeographicCoordinate Zero() => new(0, 0);
	
	public static GeographicCoordinate FromPoint(Point point) => new(Guard.Against.Null(point).Y, Guard.Against.Null(point).X); 
}