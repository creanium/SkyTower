using SkyTower.Domain.MonitoredLocations;
using SkyTower.Domain.MonitoredLocations.Specifications;

namespace SkyTower.Application.MonitoredLocations.RemoveDigestSubscription;

public class UnsubscribeFromDigestCommandHandler(IRepository<MonitoredLocation> monitoredLocationRepository) : ICommandHandler<UnsubscribeFromDigestCommand>
{
	public async Task<Result> Handle(UnsubscribeFromDigestCommand request, CancellationToken cancellationToken)
	{
		Guard.Against.Null(request);
		var monitoredLocation = await monitoredLocationRepository.SingleOrDefaultAsync(new MonitoredLocationWithDigestsSpec(request.LocationId), cancellationToken)
			.ConfigureAwait(false);

		if (monitoredLocation is null)
		{
			return Result.NotFound("Monitored location not found.");
		}
		
		try 
		{
			monitoredLocation.UnsubscribeFromDigest(request.SubscriptionId);
			return Result.Success();
		}
		catch (EntityNotFoundException<DigestSubscription> ex)
		{
			return Result.NotFound(ex.Message);
		}
	}
}