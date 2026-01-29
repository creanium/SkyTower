using Ardalis.Specification;

namespace SkyTower.Core.Entities.MonitoredLocationAggregate.Specifications;

public class MonitoredLocationWithDigestsSpec : SingleResultSpecification<MonitoredLocation>
{
	public MonitoredLocationWithDigestsSpec(Id<MonitoredLocation> locationId)
	{
		Query.Where(l => l.Id == locationId)
			.Include(l => l.DigestSubscriptions);
	}
}