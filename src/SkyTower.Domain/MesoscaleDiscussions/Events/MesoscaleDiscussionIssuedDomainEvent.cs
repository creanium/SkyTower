using SkyTower.Domain.Abstractions;

namespace SkyTower.Domain.MesoscaleDiscussions.Events;

public record MesoscaleDiscussionIssuedDomainEvent(Id<MesoscaleDiscussion> Id) : IDomainEvent;