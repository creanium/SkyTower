using SkyTower.Core.Entities.MonitoredLocationAggregate;

namespace SkyTower.UseCases.MonitoredLocations.RemoveDigestSubscription;

public sealed record RemoveDigestSubscriptionCommand(Id<MonitoredLocation> LocationId, Id<DigestSubscription> SubscriptionId) : ICommand;   
