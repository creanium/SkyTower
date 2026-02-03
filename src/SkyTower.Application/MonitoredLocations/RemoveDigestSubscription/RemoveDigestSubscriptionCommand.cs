using SkyTower.Application.Messaging;
using SkyTower.Core.Entities.MonitoredLocationAggregate;

namespace SkyTower.Application.MonitoredLocations.RemoveDigestSubscription;

public sealed record RemoveDigestSubscriptionCommand(Id<MonitoredLocation> LocationId, Id<DigestSubscription> SubscriptionId) : ICommand;   
