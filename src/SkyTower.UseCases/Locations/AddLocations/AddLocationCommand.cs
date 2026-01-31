using SkyTower.Core.Entities.LocationAggregate;

namespace SkyTower.UseCases.Locations.AddLocations;

public sealed record AddLocationCommand(string Name, double Latitude, double Longitude) : ICommand<Id<Location>>;