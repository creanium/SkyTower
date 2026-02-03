using SkyTower.Application.Messaging;
using SkyTower.Domain.Entities.MonitoredLocationAggregate;

namespace SkyTower.Application.MonitoredLocations.AddMonitoredLocation;

public class AddMonitoredLocationCommandHandler(IRepository<MonitoredLocation> repository) : ICommandHandler<AddMonitoredLocationCommand, Id<MonitoredLocation>>
{
	public async Task<Result<Id<MonitoredLocation>>> Handle(AddMonitoredLocationCommand request, CancellationToken cancellationToken)
	{
		Guard.Against.Null(request);
		var monitoredLocation = new MonitoredLocation(request.LocationId, request.UserId);
		monitoredLocation.SetConvectiveOutlookMonitoringEnabled(request.ShouldMonitorConvectiveOutlooks);
		monitoredLocation.SetMesoscaleDiscussionMonitoringEnabled(request.ShouldMonitorMesoscaleDiscussions);
		monitoredLocation.SetMonitoringDates(request.MonitoringStartDate, request.MonitoringEndDate);

		var entity = await repository.AddAsync(monitoredLocation, cancellationToken).ConfigureAwait(false);
		return Result.Created(entity.Id);
	}
}