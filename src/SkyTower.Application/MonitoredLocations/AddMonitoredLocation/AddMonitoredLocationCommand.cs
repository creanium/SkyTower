using SkyTower.Domain.Locations;
using SkyTower.Domain.MonitoredLocations;
using SkyTower.Domain.Users;

namespace SkyTower.Application.MonitoredLocations.AddMonitoredLocation;

public sealed record AddMonitoredLocationCommand(
	Id<Location> LocationId,
	Id<User> UserId,
	bool ShouldMonitorConvectiveOutlooks,
	bool ShouldMonitorMesoscaleDiscussions,
	DateOnly? MonitoringStartDate = null,
	DateOnly? MonitoringEndDate = null
) : ICommand<Id<MonitoredLocation>>;