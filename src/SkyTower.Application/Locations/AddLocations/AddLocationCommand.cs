using SkyTower.Application.Messaging;
using SkyTower.Core.Entities.LocationAggregate;

namespace SkyTower.Application.Locations.AddLocations;

public sealed record AddLocationCommand(string Name, double Latitude, double Longitude) : ICommand<Id<Location>>;