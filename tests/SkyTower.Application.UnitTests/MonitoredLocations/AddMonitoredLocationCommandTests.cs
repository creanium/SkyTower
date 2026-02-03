using FluentAssertions;

namespace SkyTower.Application.UnitTests.MonitoredLocations;

[TestFixture]
internal sealed class AddMonitoredLocationCommandTests
{
	[Test]
	public void AddMonitoredLocationCommand_CanBeCreated()
	{
		true.Should().BeTrue();
	}
}