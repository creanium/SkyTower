using SkyTower.Domain.MonitoredLocations;

namespace SkyTower.Application.MonitoredLocations.AddMonitoredLocation;

public class AddMonitoredLocationCommandHandler(IRepository<MonitoredLocation> repository) : ICommandHandler<AddMonitoredLocationCommand, Id<MonitoredLocation>>
{
	public async Task<Result<Id<MonitoredLocation>>> Handle(AddMonitoredLocationCommand request, CancellationToken cancellationToken)
	{
		Guard.Against.Null(request);
		var monitoredLocation = new MonitoredLocation(request.LocationId, request.UserId)
			.SetConvectiveOutlookMonitoringEnabled(request.ShouldMonitorConvectiveOutlooks)
			.SetMesoscaleDiscussionMonitoringEnabled(request.ShouldMonitorMesoscaleDiscussions)
			.SetMonitoringDates(new MonitoringPeriod(request.MonitoringStartDate, request.MonitoringEndDate));

		var entity = await repository.AddAsync(monitoredLocation, cancellationToken).ConfigureAwait(false);
		return Result.Created(entity.Id);
	}
}