using SkyTower.Domain.Abstractions;

namespace SkyTower.Domain.MonitoredLocations;

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
	/// The next time the digest should be sent.
	/// </summary>
	public DateTimeOffset? NextSendDate { get; private set; }

	/// <summary>
	/// The last time the digest was sent.
	/// </summary>
	public DateTimeOffset? LastSent { get; private set; }
	
	/// <summary>
	/// Calculates the next send date based on the current time, the location's time zone, the days to send, and the send time.
	/// </summary>
	/// <returns>This instance</returns>
	public DigestSubscription CalculateNextSendDate()
	{
		var locationTimeZone = MonitoredLocation.Location.TimeZone;
		var localDateTime = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, locationTimeZone);
		var nextSendDateTime = localDateTime.Date + SendTime.ToTimeSpan();

		// If the send time has already passed today, move to the next day
		if (nextSendDateTime <= localDateTime)
		{
			nextSendDateTime = nextSendDateTime.AddDays(1);
		}

		// Find the next valid day to send
		// DayOfWeek will return an integer between 0 and 6 so we can use bit shifting to check the flag
		while (!DaysToSend.HasFlag((DaysOfWeek)(1 << (int)nextSendDateTime.DayOfWeek)))
		{
			nextSendDateTime = nextSendDateTime.AddDays(1);
		}

		// Convert back to UTC
		NextSendDate = TimeZoneInfo.ConvertTimeToUtc(nextSendDateTime, locationTimeZone);
		return this;
	}
	
	/// <summary>
	/// Updates the last sent date to now and recalculates the next send date.
	/// </summary>
	/// <returns>This instance</returns>
	public DigestSubscription MarkAsSent()
	{
		LastSent = DateTimeOffset.UtcNow;
		return CalculateNextSendDate();
	}
}