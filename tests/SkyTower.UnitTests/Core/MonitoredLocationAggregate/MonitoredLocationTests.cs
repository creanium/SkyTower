using SkyTower.Core.Entities.LocationAggregate;
using SkyTower.Core.Entities.MonitoredLocationAggregate;
using SkyTower.Core.Entities.UserAggregate;
using StrictId;

namespace SkyTower.UnitTests.Core.MonitoredLocationAggregate;

[TestFixture]
public class MonitoredLocationTests
{
	[TestCase("2025-03-03", "2025-03-13", -7, -6)]
	[TestCase("2025-10-31", "2025-11-05", -6, -7)]
	public void MonitoredDates_HandleDaylightSavingTime(DateTime notBefore, DateTime notAfter, double expectedStartOffset, double expectedEndOffset)
	{
		var foundTimeZone = TimeZoneInfo.TryFindSystemTimeZoneById("America/Denver", out var timeZone);
		Assert.That(foundTimeZone, Is.True, "Did not find America/Denver time zone");
		
		var location = new Location("Denver", 39.7392, -104.9903, timeZone!);
		var monitoredLocation = new MonitoredLocation(Id<Location>.NewId(), Id<User>.NewId());
		monitoredLocation.SetLocation(location);
		Assert.That(monitoredLocation.Location, Is.EqualTo(location));
		
		monitoredLocation.SetMonitoringDates(DateOnly.FromDateTime(notBefore), DateOnly.FromDateTime(notAfter));
        Assert.Multiple(() =>
        {
            Assert.That(monitoredLocation.NotBeforeDate, Is.Not.Null, "NotBeforeDate is unexpectedly null");
            Assert.That(monitoredLocation.NotAfterDate, Is.Not.Null, "NotAfterDate is unexpectedly null");
        });
        
        Assert.Multiple(() =>
        {
	        Assert.That(monitoredLocation.NotBeforeDate.Value.Offset, Is.EqualTo(TimeSpan.FromHours(expectedStartOffset)));
	        Assert.That(monitoredLocation.NotAfterDate.Value.Offset, Is.EqualTo(TimeSpan.FromHours(expectedEndOffset)));
        });
    }
	
	[TestCase("2025-07-01", "2025-07-05", "America/Los_Angeles", -7, "America/New_York", -4)]
	[TestCase("2025-07-01", "2025-07-05", "America/New_York", -4, "America/St_Johns", -2.5)]
	public void MonitoredDates_UpdatedWhenChangingLocations(DateTime notBefore, DateTime notAfter, string startTimeZone, double expectedStartOffset, string endTimeZone, double expectedEndOffset)
	{
		var foundSourceTimeZone = TimeZoneInfo.TryFindSystemTimeZoneById(startTimeZone, out var sourceTimeZone);
		Assert.That(foundSourceTimeZone, Is.True, $"Did not find {startTimeZone} time zone");
		
		var foundDestinationTimeZone = TimeZoneInfo.TryFindSystemTimeZoneById(endTimeZone, out var destinationTimeZone);
		Assert.That(foundDestinationTimeZone, Is.True, $"Did not find {endTimeZone} time zone");
		
		var firstLocation = new Location("Los Baños", 37.058333, -120.85, sourceTimeZone!);
		var secondLocation = new Location("Bread Loaf", 43.953333, -72.9925, destinationTimeZone!);
		
		var monitoredLocation = new MonitoredLocation(Id<Location>.NewId(), Id<User>.NewId());
		monitoredLocation.SetLocation(firstLocation);
		Assert.That(monitoredLocation.Location, Is.EqualTo(firstLocation));
		
		monitoredLocation.SetMonitoringDates(DateOnly.FromDateTime(notBefore), DateOnly.FromDateTime(notAfter));
		
		Assert.Multiple(() =>
		{
			Assert.That(monitoredLocation.NotBeforeDate, Is.Not.Null, "NotBeforeDate is unexpectedly null");
			Assert.That(monitoredLocation.NotAfterDate, Is.Not.Null, "NotAfterDate is unexpectedly null");
		});
        
		Assert.Multiple(() =>
		{
			Assert.That(monitoredLocation.NotBeforeDate.Value.Offset, Is.EqualTo(TimeSpan.FromHours(expectedStartOffset)));
			Assert.That(monitoredLocation.NotAfterDate.Value.Offset, Is.EqualTo(TimeSpan.FromHours(expectedStartOffset)));
		});
		
		monitoredLocation.SetLocation(secondLocation);
        
		Assert.Multiple(() =>
		{
			Assert.That(monitoredLocation.NotBeforeDate.Value.Offset, Is.EqualTo(TimeSpan.FromHours(expectedEndOffset)));
			Assert.That(monitoredLocation.NotAfterDate.Value.Offset, Is.EqualTo(TimeSpan.FromHours(expectedEndOffset)));
		});
	}
}