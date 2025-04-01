namespace SkyTower.Core.Interfaces;

public interface IEntity<TEntity>
{
	public Id<TEntity> Id { get; }
}