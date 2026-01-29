using Ardalis.GuardClauses;
using Ardalis.SharedKernel;
using NetTopologySuite.Geometries;
using SkyTower.Core.Abstractions;
using SkyTower.Core.Enums;

namespace SkyTower.Core.Entities.LocationAggregate;

public sealed class Location(string name, double latitude, double longitude, TimeZoneInfo timeZone) : Entity<Location>, IAggregateRoot
{
	/// <summary>
	/// Needed for EF
	/// </summary>
	private Location() : this(string.Empty, 0, 0, TimeZoneInfo.Utc)
	{ }
	
	/// <summary>
	/// Location Name
	/// </summary>
	public string Name { get; private set; } = name;
	
	/// <summary>
	/// The common name for a location
	/// </summary>
	public string? LocalName { get; private set; }

	/// <summary>
	/// The state or province of the location
	/// </summary>
	public string? State { get; private set; }

	/// <summary>
	/// The latitude and longitude of the location
	/// </summary>
	public Point Position { get; private set; } = new(longitude, latitude);
	
	/// <summary>
	/// The time zone for the location
	/// </summary>
	public TimeZoneInfo TimeZone { get; private set; } = timeZone;
	
	/// <summary>
	/// The NWS County Warning Area (office) that monitors this location
	/// </summary>
	public string CountyWarningArea { get; private set; }

	/// <summary>
	/// The convective risk level for this location
	/// </summary>
	public CategoricalRisk ConvectiveRisk { get; private set; }
	
	/// <summary>
	/// Updates the locale information for this location
	/// </summary>
	/// <param name="localName">The local name for this location (typically the city or town)</param>
	/// <param name="state">The ISO-2 state in which the location resides</param>
	/// <returns>This instance</returns>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="localName"/> or <paramref name="state"/> is null</exception>
	/// <exception cref="ArgumentException">Thrown if <paramref name="localName"/> is composed of only whitespace.
	/// Also thrown if <paramref name="state"/> is not exactly 2 non-whitespace characters in length.</exception>
	public Location SetLocale(string localName, string state)
	{
		Guard.Against.NullOrWhiteSpace(state);
		LocalName = Guard.Against.NullOrWhiteSpace(localName);
		State = Guard.Against.LengthOutOfRange(state.Trim(), 2, 2);
		return this;
	}
	
	/// <summary>
	/// Sets the County Warning Area (CWA) for this location (e.g., "BOU" for Boulder, CO)
	/// </summary>
	/// <param name="cwa">The three-digit County Warning Area (CWA)</param>
	/// <returns>This instance</returns>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="cwa"/> is null</exception>
	/// <exception cref="ArgumentException">Thrown if <paramref name="cwa"/> is not exactly 3 non-whitespace characters in length.</exception>
	public Location SetCountyWarningArea(string cwa)
	{
		Guard.Against.NullOrWhiteSpace(cwa);
		CountyWarningArea = Guard.Against.LengthOutOfRange(cwa.Trim(), 3, 3);
		return this;
	}
	
	public Location SetConvectiveRisk(CategoricalRisk risk)
	{
		ConvectiveRisk = risk;
		return this;
	}
}