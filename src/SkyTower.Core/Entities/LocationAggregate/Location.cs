using NetTopologySuite.Geometries;
using SkyTower.Core.Abstractions;
using SkyTower.Core.Interfaces;

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
	/// The latitude and longitude of the location
	/// </summary>
	public Point Position { get; private set; } = new(longitude, latitude);
	
	/// <summary>
	/// The common name for a location
	/// </summary>
	public string LocalName { get; private set; }

	/// <summary>
	/// The state or province of the location
	/// </summary>
	public string State { get; private set; }

	/// <summary>
	/// The time zone for the location
	/// </summary>
	public TimeZoneInfo TimeZone { get; private set; } = timeZone;
	
	/// <summary>
	/// The NWS County Warning Area (office) that monitors this location
	/// </summary>
	public string CountyWarningArea { get; private set; }
}