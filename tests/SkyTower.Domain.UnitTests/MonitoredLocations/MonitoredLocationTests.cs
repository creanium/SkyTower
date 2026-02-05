using SkyTower.Domain.Exceptions;
using SkyTower.Domain.Locations;
using SkyTower.Domain.MonitoredLocations;
using SkyTower.Domain.Users;
using StrictId;

namespace SkyTower.Domain.UnitTests.Core.MonitoredLocationAggregate;

[TestFixture]
internal sealed class MonitoredLocationTests
{
	[TestCase("2025-03-03", "2025-03-13", -7, -6)]
	[TestCase("2025-10-31", "2025-11-05", -6, -7)]
	public void MonitoredDates_HandleDaylightSavingTime(DateTime notBefore, DateTime notAfter, double expectedStartOffset, double expectedEndOffset)
	{
		var foundTimeZone = TimeZoneInfo.TryFindSystemTimeZoneById("America/Denver", out var timeZone);
		Assert.That(foundTimeZone, Is.True, "Did not find America/Denver time zone");

		var location = Location.Create("Denver", new GeographicCoordinate(39.7392, -104.9903), timeZone!);
		var monitoredLocation = new MonitoredLocation(location, new User());
		Assert.That(monitoredLocation.Location, Is.EqualTo(location));

		monitoredLocation.SetMonitoringDates(new MonitoringPeriod(DateOnly.FromDateTime(notBefore), DateOnly.FromDateTime(notAfter)));

		using (Assert.EnterMultipleScope())
		{
			Assert.That(monitoredLocation.NotBeforeDate, Is.Not.Null, "NotBeforeDate is unexpectedly null");
			Assert.That(monitoredLocation.NotAfterDate, Is.Not.Null, "NotAfterDate is unexpectedly null");
			Assert.That(monitoredLocation.NotBeforeDate.Value.Offset, Is.EqualTo(TimeSpan.FromHours(expectedStartOffset)));
			Assert.That(monitoredLocation.NotAfterDate.Value.Offset, Is.EqualTo(TimeSpan.FromHours(expectedEndOffset)));
		}
	}

	[TestCase("2025-07-01", "2025-07-05", "America/Los_Angeles", -7, "America/New_York", -4)]
	[TestCase("2025-07-01", "2025-07-05", "America/New_York", -4, "America/St_Johns", -2.5)]
	public void MonitoredDates_UpdatedWhenChangingDates(DateTime notBefore, DateTime notAfter, string startTimeZone, double expectedStartOffset, string endTimeZone, double expectedEndOffset)
	{
		var foundSourceTimeZone = TimeZoneInfo.TryFindSystemTimeZoneById(startTimeZone, out var sourceTimeZone);
		Assert.That(foundSourceTimeZone, Is.True, $"Did not find {startTimeZone} time zone");

		var firstLocation = Location.Create("Los Baños", new GeographicCoordinate(37.058333, -120.85), sourceTimeZone!);

		var monitoredLocation = new MonitoredLocation(firstLocation, new User());
		Assert.That(monitoredLocation.Location, Is.EqualTo(firstLocation));

		monitoredLocation.SetMonitoringDates(new MonitoringPeriod(DateOnly.FromDateTime(notBefore), DateOnly.FromDateTime(notAfter)));

		using (Assert.EnterMultipleScope())
		{
			Assert.That(monitoredLocation.NotBeforeDate, Is.Not.Null, "NotBeforeDate is unexpectedly null");
			Assert.That(monitoredLocation.NotAfterDate, Is.Not.Null, "NotAfterDate is unexpectedly null");
			Assert.That(monitoredLocation.NotBeforeDate.Value.Offset, Is.EqualTo(TimeSpan.FromHours(expectedStartOffset)));
			Assert.That(monitoredLocation.NotAfterDate.Value.Offset, Is.EqualTo(TimeSpan.FromHours(expectedStartOffset)));
		}
	}

	[Test]
	public void SubscribeToDigest_ThrowsException_WhenOverlapping()
	{
		var foundTimeZone = TimeZoneInfo.TryFindSystemTimeZoneById("America/Denver", out var timeZone);
		Assert.That(foundTimeZone, Is.True, "Did not find America/Denver time zone");

		var location = Location.Create("Denver", new GeographicCoordinate(39.7392, -104.9903), timeZone!);
		var monitoredLocation = new MonitoredLocation(location, new User());

		monitoredLocation.SubscribeToDigest(DaysOfWeek.Weekdays, new TimeOnly(6, 30));
		monitoredLocation.SubscribeToDigest(DaysOfWeek.Weekends, new TimeOnly(7, 30));
		monitoredLocation.SubscribeToDigest(DaysOfWeek.Monday | DaysOfWeek.Wednesday | DaysOfWeek.Friday, new TimeOnly(12, 30));

		using (Assert.EnterMultipleScope())
		{
			// Subscription is considered overlapping if it's within an hour of another subscription on the same day
			Assert.Throws<InvalidOperationException>(() => monitoredLocation.SubscribeToDigest(DaysOfWeek.Monday, new TimeOnly(7, 00)));
			Assert.Throws<InvalidOperationException>(() => monitoredLocation.SubscribeToDigest(DaysOfWeek.Saturday, new TimeOnly(7, 30)));
			Assert.Throws<InvalidOperationException>(() => monitoredLocation.SubscribeToDigest(DaysOfWeek.Tuesday | DaysOfWeek.Wednesday | DaysOfWeek.Thursday, new TimeOnly(7, 00)));
			Assert.Throws<InvalidOperationException>(() => monitoredLocation.SubscribeToDigest(DaysOfWeek.Tuesday | DaysOfWeek.Wednesday | DaysOfWeek.Thursday, new TimeOnly(12, 30)));
			Assert.DoesNotThrow(() => monitoredLocation.SubscribeToDigest(DaysOfWeek.Tuesday | DaysOfWeek.Wednesday | DaysOfWeek.Thursday, new TimeOnly(17, 00)));
			Assert.DoesNotThrow(() => monitoredLocation.SubscribeToDigest(DaysOfWeek.Tuesday | DaysOfWeek.Thursday, new TimeOnly(12, 30)));
		}
		
		Assert.That(monitoredLocation.DigestSubscriptions, Has.Count.EqualTo(5));
	}

	[Test]
	public void SubscribeToDigest_ThrowsException_WhenMonitoringHasEnded()
	{
		var foundTimeZone = TimeZoneInfo.TryFindSystemTimeZoneById("America/Denver", out var timeZone);
		Assert.That(foundTimeZone, Is.True, "Did not find America/Denver time zone");

		var location = Location.Create("Denver", new GeographicCoordinate(39.7392, -104.9903), timeZone!);
		var monitoredLocation = new MonitoredLocation(location, new User());

		monitoredLocation.SetMonitoringDates(new MonitoringPeriod(null, DateOnly.FromDateTime(DateTime.Now.AddDays(-7))));
		Assert.Throws<InvalidOperationException>(() => monitoredLocation.SubscribeToDigest(DaysOfWeek.Monday, new TimeOnly(7, 00)));
	}
	
	[Test]
	public void UnsubscribeFromDigest_RemovesSubscription()
	{
		var foundTimeZone = TimeZoneInfo.TryFindSystemTimeZoneById("America/Denver", out var timeZone);
		Assert.That(foundTimeZone, Is.True, "Did not find America/Denver time zone");

		var location = Location.Create("Denver", new GeographicCoordinate(39.7392, -104.9903), timeZone!);
		var monitoredLocation = new MonitoredLocation(location, new User());

		monitoredLocation.SubscribeToDigest(DaysOfWeek.Monday, new TimeOnly(7, 00));
		monitoredLocation.SubscribeToDigest(DaysOfWeek.Friday, new TimeOnly(9, 00));
		Assert.That(monitoredLocation.DigestSubscriptions, Has.Count.EqualTo(2));

		var subscriptionId = monitoredLocation.DigestSubscriptions.First().Id;
		Assert.DoesNotThrow(() => monitoredLocation.UnsubscribeFromDigest(subscriptionId));
		Assert.That(monitoredLocation.DigestSubscriptions, Has.Count.EqualTo(1));
	}

	[Test]
	public void UnsubscribeFromDigest_ThrowsException_WhenSubscriptionNotFound()
	{
		var foundTimeZone = TimeZoneInfo.TryFindSystemTimeZoneById("America/Denver", out var timeZone);
		Assert.That(foundTimeZone, Is.True, "Did not find America/Denver time zone");

		var location = Location.Create("Denver", new GeographicCoordinate(39.7392, -104.9903), timeZone!);
		var monitoredLocation = new MonitoredLocation(location, new User());
		monitoredLocation.SubscribeToDigest(DaysOfWeek.Monday, new TimeOnly(7, 00));
		monitoredLocation.SubscribeToDigest(DaysOfWeek.Friday, new TimeOnly(9, 00));
		Assert.That(monitoredLocation.DigestSubscriptions, Has.Count.EqualTo(2));

		Assert.Throws<EntityNotFoundException<DigestSubscription>>(() => monitoredLocation.UnsubscribeFromDigest(Id<DigestSubscription>.NewId()));
		Assert.That(monitoredLocation.DigestSubscriptions, Has.Count.EqualTo(2));
	}
}