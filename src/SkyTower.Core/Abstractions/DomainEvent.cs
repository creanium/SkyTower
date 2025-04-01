using MediatR;

namespace SkyTower.Core.Abstractions;

/// <summary>
/// A base type for domain events. Depends on MediatR INotification.
/// Includes DateOccurred which is set on creation.
/// </summary>
public abstract class DomainEvent : INotification
{
	public DateTimeOffset DateOccurred { get; protected set; } = DateTimeOffset.UtcNow;
}