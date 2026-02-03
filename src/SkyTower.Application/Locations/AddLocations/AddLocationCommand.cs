using SkyTower.Application.Messaging;
using SkyTower.Domain.Entities.LocationAggregate;

namespace SkyTower.Application.Locations.AddLocations;

public sealed record AddLocationCommand(string Name, double Latitude, double Longitude) : ICommand<Id<Location>>;