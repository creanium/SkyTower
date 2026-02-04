using SkyTower.Domain.MonitoredLocations;

namespace SkyTower.Application.MonitoredLocations.RemoveDigestSubscription;

public sealed record UnsubscribeFromDigestCommand(Id<MonitoredLocation> LocationId, Id<DigestSubscription> SubscriptionId) : ICommand;   
