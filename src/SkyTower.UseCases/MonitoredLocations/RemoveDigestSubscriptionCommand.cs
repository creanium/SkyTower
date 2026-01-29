using SkyTower.Core.Entities.MonitoredLocationAggregate;
using SkyTower.UseCases.Messaging;

namespace SkyTower.UseCases.MonitoredLocations;

public sealed record RemoveDigestSubscriptionCommand(Id<MonitoredLocation> LocationId, Id<DigestSubscription> SubscriptionId) : ICommand;   
