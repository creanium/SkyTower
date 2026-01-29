namespace SkyTower.Core.Exceptions;

/// <summary>
/// Exception thrown when an entity is not found.
/// </summary>
public class EntityNotFoundException<TEntity> : Exception
{
    public EntityNotFoundException(Id<TEntity> id) : base($"{typeof(TEntity).Name} with ID '{id}' was not found.")
    {
    }

    public EntityNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public EntityNotFoundException()
    {
    }

    public EntityNotFoundException(string message) : base(message)
    {
    }
}