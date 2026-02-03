using SkyTower.Application.Messaging;
using SkyTower.Core.Entities.LocationAggregate;
using SkyTower.Core.Entities.MonitoredLocationAggregate;
using SkyTower.Core.Entities.UserAggregate;

namespace SkyTower.Application.MonitoredLocations.AddMonitoredLocation;

public sealed record AddMonitoredLocationCommand(
	Id<Location> LocationId,
	Id<User> UserId,
	bool ShouldMonitorConvectiveOutlooks,
	bool ShouldMonitorMesoscaleDiscussions,
	DateOnly? MonitoringStartDate = null,
	DateOnly? MonitoringEndDate = null
) : ICommand<Id<MonitoredLocation>>;