using SkyTower.Domain.Locations;
using SkyTower.Domain.MonitoredLocations;
using SkyTower.Domain.Users;

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
		
		monitoredLocation.SetMonitoringDates(DateOnly.FromDateTime(notBefore), DateOnly.FromDateTime(notAfter));
        
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
		
		monitoredLocation.SetMonitoringDates(DateOnly.FromDateTime(notBefore), DateOnly.FromDateTime(notAfter));

        using (Assert.EnterMultipleScope())
        {
			Assert.That(monitoredLocation.NotBeforeDate, Is.Not.Null, "NotBeforeDate is unexpectedly null");
			Assert.That(monitoredLocation.NotAfterDate, Is.Not.Null, "NotAfterDate is unexpectedly null");
			Assert.That(monitoredLocation.NotBeforeDate.Value.Offset, Is.EqualTo(TimeSpan.FromHours(expectedStartOffset)));
			Assert.That(monitoredLocation.NotAfterDate.Value.Offset, Is.EqualTo(TimeSpan.FromHours(expectedStartOffset)));
		}
	}
}