using SkyTower.Domain.Entities.MonitoredLocationAggregate;

namespace SkyTower.Application.MonitoredLocations.RemoveDigestSubscription;

public sealed record RemoveDigestSubscriptionCommand(Id<MonitoredLocation> LocationId, Id<DigestSubscription> SubscriptionId) : ICommand;   
