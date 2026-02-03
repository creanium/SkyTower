using SkyTower.Domain.Locations;

namespace SkyTower.Application.Locations.AddLocations;

public sealed record AddLocationCommand(string Name, double Latitude, double Longitude) : ICommand<Id<Location>>;