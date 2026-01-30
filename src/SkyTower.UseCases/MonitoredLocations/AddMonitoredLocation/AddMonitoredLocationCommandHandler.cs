using SkyTower.Core.Entities.MonitoredLocationAggregate;

namespace SkyTower.UseCases.MonitoredLocations.AddMonitoredLocation;

public class AddMonitoredLocationCommandHandler(IRepository<MonitoredLocation> repository) : ICommandHandler<AddMonitoredLocationCommand, Result<Id<MonitoredLocation>>>
{
	public async Task<Result<Result<Id<MonitoredLocation>>>> Handle(AddMonitoredLocationCommand request, CancellationToken cancellationToken)
	{
		Guard.Against.Null(request);
		var monitoredLocation = new MonitoredLocation(request.LocationId, request.UserId);

		var entity = await repository.AddAsync(monitoredLocation, cancellationToken).ConfigureAwait(false);
		return Result.Created(entity.Id);
	}
}