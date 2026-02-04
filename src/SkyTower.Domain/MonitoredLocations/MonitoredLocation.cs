using Ardalis.GuardClauses;
using Ardalis.SharedKernel;
using JetBrains.Annotations;
using SkyTower.Domain.Abstractions;
using SkyTower.Domain.Exceptions;
using SkyTower.Domain.Locations;
using SkyTower.Domain.Users;

namespace SkyTower.Domain.MonitoredLocations;

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
	public bool ShouldMonitorConvectiveOutlooks { get; private set; }

	/// <summary>
	/// Indicates whether this location should be monitored for Convective Outlooks.
	/// </summary>
	public bool ShouldMonitorMesoscaleDiscussions { get; private set; }

	private readonly List<DigestSubscription> _digestSubscriptions = [];
	public IReadOnlyCollection<DigestSubscription> DigestSubscriptions => _digestSubscriptions.AsReadOnly();

	public MonitoredLocation(Location location, User user) : this(Guard.Against.Null(location).Id, Guard.Against.Null(user).Id)
	{
		Location = Guard.Against.Null(location, message: "Location is null.");
		User = Guard.Against.Null(user, message: "User is null.");
		LocationId = location.Id;
		UserId = user.Id;
	}

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
		ShouldMonitorConvectiveOutlooks = isEnabled;
		return this;
	}

	/// <summary>
	/// Sets whether Mesoscale Discussion monitoring and alerting is enabled for this location.
	/// </summary>
	/// <param name="isEnabled"><c>true</c> if monitoring should be enabled</param>
	/// <returns>This instance</returns>
	public MonitoredLocation SetMesoscaleDiscussionMonitoringEnabled(bool isEnabled)
	{
		ShouldMonitorMesoscaleDiscussions = isEnabled;
		return this;
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

	public MonitoredLocation SubscribeToDigest(DaysOfWeek daysToSend, TimeOnly sendTime)
	{
		Guard.Against.Expression(d => d == DaysOfWeek.None, daysToSend, "At least one day must be selected to send the digest.");
		
		if (NotAfterDate.HasValue && NotAfterDate < DateTimeOffset.UtcNow)
		{
			throw new InvalidOperationException("Cannot add a digest subscription to a monitored location that has ended monitoring.");
		}
		
		var overlappingDigests = _digestSubscriptions
			.Where(s => s.SendTime < sendTime.AddHours(1) && s.SendTime > sendTime.AddHours(-1))
			.Where(s => (s.DaysToSend & daysToSend) > 0)
			.ToList();

		// Ensure no existing subscription matches the same days and time
		if (overlappingDigests.Count > 0)
		{
			throw new InvalidOperationException("The requested days and time overlap with an existing subscription. A digest cannot be sent within one hour of another digest on the same day.");
		}

		var subscription = new DigestSubscription(this, daysToSend, sendTime);
		_digestSubscriptions.Add(subscription);

		return this;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="subscriptionId"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentException">Thrown if <paramref name="subscriptionId"/> is the default value</exception>
	/// <exception cref="EntityNotFoundException{TEntity}">Thrown if a subscription matching <paramref name="subscriptionId"/> does not exist for this monitored location</exception>
	public MonitoredLocation UnsubscribeFromDigest(Id<DigestSubscription> subscriptionId)
	{
		Guard.Against.Default(subscriptionId, message: "A valid subscription ID was not provided.");
		var subscription = _digestSubscriptions.Find(s => s.Id == subscriptionId);

		Guard.Against.Default(subscription, message: $"No digest subscription found with ID {subscriptionId}.",
			exceptionCreator: () => new EntityNotFoundException<DigestSubscription>(subscriptionId));

		_digestSubscriptions.Remove(subscription);

		return this;
	}
}