using SkyTower.Core.Entities.LocationAggregate;
using SkyTower.Core.Entities.MonitoredLocationAggregate;
using SkyTower.Core.Entities.UserAggregate;

namespace SkyTower.UseCases.MonitoredLocations.AddMonitoredLocation;

public record AddMonitoredLocationCommand(Id<Location> LocationId, Id<User> UserId) : ICommand<Result<Id<MonitoredLocation>>>;