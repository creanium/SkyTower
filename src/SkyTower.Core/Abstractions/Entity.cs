using JetBrains.Annotations;
using SkyTower.Core.Interfaces;
using SkyTower.Core.ValueObjects;

namespace SkyTower.Core.Abstractions;

public abstract class Entity<TImplementation> : IEntity<TImplementation>, IAuditableEntity, IEqualityComparer<TImplementation> 
	where TImplementation : Entity<TImplementation>
{
	public Id<TImplementation> Id { get; [UsedImplicitly] private set; }
	public AuditRegister Created { get; private set; } = new(null!, default);
	public AuditRegister LastModified { get; private set; } = new(null!, default);

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

	public virtual bool Equals(TImplementation? x, TImplementation? y)
	{
		if (x is null || y is null) return false;
		if (ReferenceEquals(x, y)) return true;
		if (x.GetType() != GetType()) return false;
		if (y.GetType() != GetType()) return false;
		
		return x.Id == y.Id;
	}

	public override bool Equals(object? obj)
	{
		if (obj is null) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != GetType()) return false;

		return Id == ((TImplementation)obj).Id;
	}

	public int GetHashCode(TImplementation? obj)
	{
		return obj?.Id.GetHashCode() ?? throw new ArgumentNullException(nameof(obj));
	}

	public override int GetHashCode()
	{
		// ReSharper disable once NonReadonlyMemberInGetHashCode
		return Id.GetHashCode();
	}
}