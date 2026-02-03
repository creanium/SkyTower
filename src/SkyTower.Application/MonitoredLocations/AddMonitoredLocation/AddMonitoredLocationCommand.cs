using SkyTower.Application.Messaging;
using SkyTower.Domain.Entities.LocationAggregate;
using SkyTower.Domain.Entities.MonitoredLocationAggregate;
using SkyTower.Domain.Entities.UserAggregate;

namespace SkyTower.Application.MonitoredLocations.AddMonitoredLocation;

public sealed record AddMonitoredLocationCommand(
	Id<Location> LocationId,
	Id<User> UserId,
	bool ShouldMonitorConvectiveOutlooks,
	bool ShouldMonitorMesoscaleDiscussions,
	DateOnly? MonitoringStartDate = null,
	DateOnly? MonitoringEndDate = null
) : ICommand<Id<MonitoredLocation>>;