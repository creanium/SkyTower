using NetTopologySuite.Geometries;
using SkyTower.Domain.Abstractions;

namespace SkyTower.Domain.MesoscaleDiscussions.Events;

public record MesoscaleDiscussionIssuedDomainEvent(Id<MesoscaleDiscussion> Id, LinearRing Boundary, ValidityPeriod ValidityPeriod) : IDomainEvent;