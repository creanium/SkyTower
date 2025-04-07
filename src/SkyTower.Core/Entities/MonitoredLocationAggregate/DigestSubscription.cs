using SkyTower.Core.Abstractions;
using SkyTower.Core.Enums;

namespace SkyTower.Core.Entities.MonitoredLocationAggregate;

public class DigestSubscription(MonitoredLocation monitoredLocation, DaysOfWeek daysToSend, TimeOnly sendTime) : Entity<DigestSubscription>
{
	/// <summary>
	/// The ID of the parent monitored location.
	/// </summary>
	public Id<MonitoredLocation> MonitoredLocationId { get; private set; } = monitoredLocation.Id;

	/// <summary>
	/// The parent monitored location.
	/// </summary>
	public MonitoredLocation MonitoredLocation { get; private set; } = monitoredLocation;
	
	/// <summary>
	/// Which days of the week to send the digest.
	/// </summary>
	public DaysOfWeek DaysToSend { get; private set; } = daysToSend;

	/// <summary>
	/// The time in the location's time zone to send the digest.
	/// </summary>
 	public TimeOnly SendTime { get; private set; } = sendTime;

	/// <summary>
	/// The last time the digest was sent.
	/// </summary>
	public DateTimeOffset? LastSent { get; private set; }
	
	/// <summary>
	/// The next time the digest should be sent.
	/// </summary>
	public DateTimeOffset? NextSendDate { get; private set; }
}