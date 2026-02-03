using FluentAssertions;

namespace SkyTower.UseCases.UnitTests.MonitoredLocations;

[TestFixture]
internal sealed class AddMonitoredLocationCommandTests
{
	[Test]
	public void AddMonitoredLocationCommand_CanBeCreated()
	{
		true.Should().BeTrue();
	}
}