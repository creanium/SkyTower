using Ardalis.GuardClauses;
using NetTopologySuite.Geometries;
using SkyTower.Domain.Locations;

namespace SkyTower.Domain.Extensions;

public static class GeographicCoordinateExtensions
{
	/// <summary>
	/// Converts a <see cref="GeographicCoordinate"/> to a <see cref="Point"/>.
	/// </summary>
	/// <param name="coordinate">The source coordinate to convert into a Point</param>
	/// <returns>A new <see cref="Point"/> instance set to the coordinates passed in through <paramref name="coordinate"/>.</returns>
	/// <remarks>
	/// Coordinates in NTS are in terms of X and Y values. To represent longitude and latitude, use X for longitude and Y for latitude.
	/// Note that this is backwards from the latitude, longitude format in which you typically see these values.
	/// </remarks>
	public static Point ToPoint(this GeographicCoordinate coordinate)
	{
		Guard.Against.Null(coordinate);
		return new Point(coordinate.Longitude, coordinate.Latitude);
	}
}