using System.ComponentModel.DataAnnotations.Schema;

namespace SkyTower.Core.Abstractions;

public abstract class EntityWithDomainEvents
{
	private readonly List<DomainEvent> _domainEvents = [];
	
	[NotMapped]
	public IEnumerable<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

	protected void RegisterDomainEvent(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);
	internal void ClearDomainEvents() => _domainEvents.Clear();
}