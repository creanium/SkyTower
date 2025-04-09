using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SkyTower.Infrastructure.Data.ValueConverters;

[UsedImplicitly]
public class TimeZoneInfoConverter() : ValueConverter<TimeZoneInfo, string>(
	v => v.Id,
	v => TimeZoneInfo.FindSystemTimeZoneById(v)
);