using SkyTower.Domain.Abstractions;

namespace SkyTower.Domain.Exceptions;

/// <summary>
/// Exception thrown when an entity is not found.
/// </summary>
public class EntityNotFoundException<TEntity> : Exception where TEntity : Entity<TEntity>
{
    public EntityNotFoundException(Id<TEntity> id) : base($"{typeof(TEntity).Name} with ID '{id}' was not found.")
    {
    }

    public EntityNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    private EntityNotFoundException()
    {
    }

    public EntityNotFoundException(string message) : base(message)
    {
    }
}