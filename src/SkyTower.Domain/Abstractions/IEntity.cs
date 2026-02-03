namespace SkyTower.Domain.Abstractions;

public interface IEntity<TEntity>
{
	public Id<TEntity> Id { get; }
}