using SkyTower.Domain.MonitoredLocations;

namespace SkyTower.Application.MonitoredLocations.SubscribeToDigest;

public record SubscribeToDigestCommand(Id<MonitoredLocation> MonitoredLocationId, DaysOfWeek DaysOfWeek, TimeOnly LocalDeliveryTime) : ICommand<MonitoredLocation>;