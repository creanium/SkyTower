using SkyTower.Core.ValueObjects;

namespace SkyTower.Core.Interfaces;

public interface IAuditableEntity
{
	/// <summary>
	/// Capture the user who created the entity and when it was created
	/// </summary>
	public AuditRegister Created { get; }
	
	/// <summary>
	/// Capture the user who last modified the entity and when it was last modified
	/// </summary>
	public AuditRegister LastModified { get; }
	
	/// <summary>
	/// A flag to set if the entity should skip automated audit. This is useful when the entity is being created or modified by migration.
	/// </summary>
	public bool ShouldSkipAutomatedAudit { get; }
	
	/// <summary>
	/// Mark the entity as created by the specified user
	/// </summary>
	/// <param name="by">The user who created the entity</param>
	public void SetCreated(IApplicationUser? by);
	
	/// <summary>
	/// Mark the entity as modified by the specific user. This will also set the LastModified.On property to the current date and time
	/// </summary>
	/// <param name="by">The user who modified the entity</param>
	public void SetLastModified(IApplicationUser? by);
	
	/// <summary>
	/// Flags this entity as not needing automated auditing.
	/// </summary>
	public void SkipAutomatedAudit();
}