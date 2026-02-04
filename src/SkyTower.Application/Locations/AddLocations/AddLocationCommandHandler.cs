using SkyTower.Domain.Locations;

namespace SkyTower.Application.Locations.AddLocations;

public class AddLocationCommandHandler(IRepository<Location> repository, ITimeZoneDataProvider timeZoneDataProvider) : ICommandHandler<AddLocationCommand, Id<Location>>
{
	public async Task<Result<Id<Location>>> Handle(AddLocationCommand request, CancellationToken cancellationToken)
	{
		Guard.Against.Null(request);

		if (!GeographicCoordinate.TryCreate(request.Latitude, request.Longitude, out var position) || position is null)
		{
			return Result.Error("Provided coordinates are invalid.");
		}
		
		var timeZone = timeZoneDataProvider.GetTimeZoneInfo(position.Latitude, position.Longitude);
		
		if (!timeZone.IsSuccess)
		{
			return Result.Error("Failed to determine time zone for the provided coordinates: " + string.Join(", ", timeZone.Errors));
		}
		
		var location = Location.Create(request.Name, position, timeZone.Value);
		var savedLocation = await repository.AddAsync(location, cancellationToken).ConfigureAwait(false);

		return Result.Created(savedLocation.Id);
	}
}