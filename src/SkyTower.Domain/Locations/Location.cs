using Ardalis.GuardClauses;
using Ardalis.SharedKernel;
using NetTopologySuite.Geometries;
using SkyTower.Domain.Abstractions;
using SkyTower.Domain.ConvectiveOutlooks;

namespace SkyTower.Domain.Locations;

public sealed class Location : Entity<Location>, IAggregateRoot
{
	/// <summary>
	/// Needed for EF
	/// </summary>
	private Location() : this("Unknown", GeographicCoordinate.Zero(), TimeZoneInfo.Utc)
	{
	}

	private Location(string name, GeographicCoordinate position, TimeZoneInfo timeZone) : base(Id<Location>.NewId())
	{
		Name = Guard.Against.NullOrWhiteSpace(name);
		Position = position;
		TimeZone = timeZone;
	}

	/// <summary>
	/// Location Name
	/// </summary>
	public string Name { get; private set; }

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
	public GeographicCoordinate Position { get; private set; }

	/// <summary>
	/// The time zone for the location
	/// </summary>
	public TimeZoneInfo TimeZone { get; private set; }

	/// <summary>
	/// The NWS County Warning Area (office) that monitors this location
	/// </summary>
	public string? CountyWarningArea { get; private set; }

	/// <summary>
	/// The convective risk level for this location
	/// </summary>
	public CategoricalRisk ConvectiveRisk { get; private set; }

	/// <summary>
	/// Using a factory method hides the constructor which may have other implementation details we don't want to leak outside the entity.
	/// Encapsulates behavior and also allows us to introduce side effects which we would not want to do in a constructor, for instance domain events.
	/// If we were to raise a domain event in a constructor, then anything that uses it would end up creating an "entity created" domain event, even
	/// if it wasn't actually created. This is an explicit action.
	/// </summary>
	/// <param name="name">The location's name</param>
	/// <param name="position">The geographic coordinate for this location</param>
	/// <param name="timeZone">The time zone in which this location lies</param>
	/// <returns></returns>
	public static Location Create(string name, GeographicCoordinate position, TimeZoneInfo timeZone)
	{
		return new Location(Guard.Against.NullOrWhiteSpace(name), position, timeZone);
	}

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