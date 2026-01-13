namespace SkyTower.Core.Interfaces;

#pragma warning disable CA1040
public interface IAggregateRoot
{	
	// This interface is a marker interface for aggregate roots in the domain model.
	// It doesn't contain any members, but it indicates that the implementing class is an aggregate root.
	// Aggregate roots are responsible for managing the lifecycle of their child entities and enforcing invariants.
	// They are the entry point for accessing and modifying the state of the aggregate.
}