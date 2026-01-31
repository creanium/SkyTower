using SkyTower.Core.Entities.LocationAggregate;
using SkyTower.Core.Interfaces;

namespace SkyTower.UseCases.Locations.AddLocations;

public class AddLocationCommandHandler(IRepository<Location> repository, IGeospatialDataProvider geospatialDataProvider) : ICommandHandler<AddLocationCommand, Id<Location>>
{
	public async Task<Result<Id<Location>>> Handle(AddLocationCommand request, CancellationToken cancellationToken)
	{
		Guard.Against.Null(request);
		var timeZone = geospatialDataProvider.GetTimeZoneInfo(request.Latitude, request.Longitude, cancellationToken);
		
		if (!timeZone.IsSuccess)
		{
			return Result.Error("Failed to determine time zone for the provided coordinates: " + string.Join(", ", timeZone.Errors));
		}
		
		var location = new Location(request.Name, request.Latitude, request.Longitude, timeZone.Value);
		var savedLocation = await repository.AddAsync(location, cancellationToken).ConfigureAwait(false);

		return Result.Created(savedLocation.Id);
	}
}