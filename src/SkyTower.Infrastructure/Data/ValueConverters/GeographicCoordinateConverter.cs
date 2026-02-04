using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using SkyTower.Domain.Extensions;
using SkyTower.Domain.Locations;

namespace SkyTower.Infrastructure.Data.ValueConverters;

[UsedImplicitly]
public class GeographicCoordinateConverter() : ValueConverter<GeographicCoordinate, Point>(
	c => c.ToPoint(),
	p => GeographicCoordinate.FromPoint(p) 
);