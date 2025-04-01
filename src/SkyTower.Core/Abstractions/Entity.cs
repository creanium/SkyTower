using JetBrains.Annotations;
using SkyTower.Core.Interfaces;
using SkyTower.Core.ValueObjects;

namespace SkyTower.Core.Abstractions;

public abstract class Entity<TImplementation> : EntityWithDomainEvents, IEntity<TImplementation>, IAuditableEntity
{
	public Id<TImplementation> Id { get; [UsedImplicitly] private set; }
	public AuditRegister Created { get; private set; } = new(null!, default);
	public AuditRegister LastModified { get; private set; } = new(null!, default);
	public bool ShouldSkipAutomatedAudit { get; private set; } = false;
	public int? LegacyId { get; private set; } 

	public void SetCreated(IApplicationUser? by)
	{
		if (Created.On != default)
		{
			throw new InvalidOperationException("The 'Created' audit has already been set.");
		}
		
		SetLastModified(by);
		Created = LastModified;
	}

	public void SetLastModified(IApplicationUser? by)
	{
		LastModified = new AuditRegister(by?.Email ?? string.Empty, DateTimeOffset.UtcNow);
	}

	public void SkipAutomatedAudit()
	{
		ShouldSkipAutomatedAudit = true;
	}

	public void SetLegacyId(int legacyId, AuditRegister created, AuditRegister modified)
	{
		SkipAutomatedAudit();
		
		Created = created;
		LastModified = modified;
		LegacyId = legacyId;
	}
}