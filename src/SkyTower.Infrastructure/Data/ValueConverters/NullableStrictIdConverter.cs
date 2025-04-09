using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SkyTower.Infrastructure.Data.ValueConverters;

public class NullableStrictIdConverter<T>() : ValueConverter<Id<T>?, string?>(
	id => id.HasValue ? id.Value.ToString() : null,
	value => string.IsNullOrEmpty(value) ? null : new Id<T>(value)
);