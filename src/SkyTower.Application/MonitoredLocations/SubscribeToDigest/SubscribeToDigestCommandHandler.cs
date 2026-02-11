using SkyTower.Domain.MonitoredLocations;
using SkyTower.Domain.MonitoredLocations.Specifications;

namespace SkyTower.Application.MonitoredLocations.SubscribeToDigest;

public class SubscribeToDigestCommandHandler(IRepository<MonitoredLocation> repository) : ICommandHandler<SubscribeToDigestCommand, MonitoredLocation>
{
	public async Task<Result<MonitoredLocation>> Handle(SubscribeToDigestCommand request, CancellationToken cancellationToken)
	{
		Guard.Against.Null(request);
		var monitoredLocation = await repository
			.SingleOrDefaultAsync(new MonitoredLocationWithDigestsSpec(request.MonitoredLocationId), cancellationToken)
			.ConfigureAwait(false);

		if (monitoredLocation is null)
		{
			return Result.NotFound($"Monitored location with id {request.MonitoredLocationId} not found.");
		}

		monitoredLocation.SubscribeToDigest(request.DaysOfWeek, request.LocalDeliveryTime);

		var updateCount = await repository.UpdateAsync(monitoredLocation, cancellationToken).ConfigureAwait(false);

		return updateCount > 0
			? Result<MonitoredLocation>.Success(monitoredLocation)
			: Result<MonitoredLocation>.Error($"Failed to update monitored location with id {request.MonitoredLocationId}.");
	}
}