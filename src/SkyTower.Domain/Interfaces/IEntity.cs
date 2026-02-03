namespace SkyTower.Domain.Interfaces;

public interface IEntity<TEntity>
{
	public Id<TEntity> Id { get; }
}