using Ardalis.GuardClauses;
using JetBrains.Annotations;
using SkyTower.Core.Abstractions;
using SkyTower.Core.Entities.LocationAggregate;
using SkyTower.Core.Entities.UserAggregate;
using SkyTower.Core.Enums;
using SkyTower.Core.Interfaces;

namespace SkyTower.Core.Entities.MonitoredLocationAggregate;

public sealed class MonitoredLocation(Id<Location> locationId, Id<User> userId) : Entity<MonitoredLocation>, IAggregateRoot
{
	public Id<Location> LocationId { get; private set; } = Guard.Against.Default(locationId);
	public Location Location { get; [UsedImplicitly] private set; } = null!;

	public Id<User> UserId { get; private set; } = Guard.Against.Default(userId);
	public User User { get; [UsedImplicitly] private set; } = null!;

	/// <summary>
	/// Indicates whether this monitored location is enabled.
	/// </summary>
	public bool IsEnabled { get; private set; } = true;
	
	/// <summary>
	/// Calculated property to indicate if monitoring is active, based on the <see cref="IsEnabled" />,
	/// <see cref="NotBeforeDate" /> and <see cref="NotAfterDate" /> properties.
	/// </summary>
	public bool IsActive => IsEnabled 
	                        && DateTimeOffset.UtcNow < (NotAfterDate ?? DateTimeOffset.MaxValue) 
	                        && DateTimeOffset.UtcNow > (NotBeforeDate ?? DateTimeOffset.MinValue);
	
	/// <summary>
	/// Do not begin monitoring until this date. Calculated from the start of the day in the <see cref="Location" />'s time zone.
	/// </summary>
	public DateTimeOffset? NotBeforeDate { get; private set; }

	/// <summary>
	/// Do not begin monitoring until this date. Calculated from the end of the day in the <see cref="Location" />'s time zone.
	/// </summary>
	public DateTimeOffset? NotAfterDate { get; private set; }

	/// <summary>
	/// Indicates whether this location should be monitored for Convective Outlooks.
	/// </summary>
	public bool ConvectiveOutlookMonitoringEnabled { get; private set; }
	
	/// <summary>
	/// Indicates whether this location should be monitored for Convective Outlooks.
	/// </summary>
	public bool MesoscaleDiscussionMonitoringEnabled { get; private set; }

	private List<DigestSubscription> _digestSubscriptions = [];
	public IReadOnlyCollection<DigestSubscription> DigestSubscriptions => _digestSubscriptions.AsReadOnly(); 

	/// <summary>
	/// Enables overall monitoring for this location.
	/// </summary>
	/// <param name="isEnabled"><c>true</c> if this location should be monitored</param>
	/// <returns>This instance</returns>
	public MonitoredLocation SetEnabled(bool isEnabled)
	{
		IsEnabled = isEnabled;
		return this;
	}

	/// <summary>
	/// Sets whether Convective Outlook monitoring and alerting is enabled for this location.
	/// </summary>
	/// <param name="isEnabled"><c>true</c> if monitoring should be enabled</param>
	/// <returns>This instance</returns>
	public MonitoredLocation SetConvectiveOutlookMonitoringEnabled(bool isEnabled)
	{
		ConvectiveOutlookMonitoringEnabled = isEnabled;
		return this;
	}
	
	/// <summary>
	/// Sets whether Mesoscale Discussion monitoring and alerting is enabled for this location.
	/// </summary>
	/// <param name="isEnabled"><c>true</c> if monitoring should be enabled</param>
	/// <returns>This instance</returns>
	public MonitoredLocation SetMesoscaleDiscussionMonitoringEnabled(bool isEnabled)
	{
		MesoscaleDiscussionMonitoringEnabled = isEnabled;
		return this;
	}

	/// <summary>
	/// Changes the location for this monitored location.
	/// </summary>
	/// <param name="location"></param>
	/// <returns></returns>
	public MonitoredLocation SetLocation(Location location)
	{
		Location = Guard.Against.Default(location);
		return RecalculateMonitoringDates();
	}

	private MonitoredLocation RecalculateMonitoringDates()
	{
		var notBefore = NotBeforeDate.HasValue ? DateOnly.FromDateTime(NotBeforeDate.Value.Date) : (DateOnly?)null;
		var notAfter = NotAfterDate.HasValue ? DateOnly.FromDateTime(NotAfterDate.Value.Date) : (DateOnly?)null;

		return SetMonitoringDates(notBefore, notAfter);
	}
	
	/// <summary>
	/// Sets the date range for monitoring. The start and end dates are both inclusive.
	/// </summary>
	/// <param name="notBefore">The date to begin monitoring for this location. If <c>null</c>, then monitoring will begin immediately.</param>
	/// <param name="notAfter">The date to end monitoring for this location. If <c>null</c>, then monitoring will continue indefinitely.</param>
	/// <returns>This instance</returns>
	public MonitoredLocation SetMonitoringDates(DateOnly? notBefore, DateOnly? notAfter)
	{
		Guard.Against.Null(Location, message: "Location is null and must be available to set monitoring dates.");
		Guard.Against.Null(Location.TimeZone, message: "Time Zone is not set for the location, and must be available in order to set monitoring dates.");

		NotBeforeDate = null;
		if (notBefore.HasValue)
		{
			var localStartDate = new DateTime(notBefore.Value, TimeOnly.MinValue, DateTimeKind.Unspecified);
			var offset = Location.TimeZone.GetUtcOffset(localStartDate);
			NotBeforeDate = new DateTimeOffset(localStartDate, offset);
		}
		
		NotAfterDate = null;
		if (notAfter.HasValue)
		{
			var localEndDate = new DateTime(notAfter.Value, TimeOnly.MaxValue, DateTimeKind.Unspecified);
			var offset = Location.TimeZone.GetUtcOffset(localEndDate);
			NotAfterDate = new DateTimeOffset(localEndDate, offset);
		}

		return this;
	}
	
	public MonitoredLocation AddDigestSubscription(DaysOfWeek daysToSend, TimeOnly sendTime)
	{
		var subscription = new DigestSubscription(this, daysToSend, sendTime);
		_digestSubscriptions.Add(subscription);
		
		return this;
	}
	
	public MonitoredLocation RemoveDigestSubscription(DigestSubscription subscription)
	{
		Guard.Against.Null(subscription, message: "Cannot remove a null subscription.");
		_digestSubscriptions.Remove(subscription);
		
		return this;
	}
}