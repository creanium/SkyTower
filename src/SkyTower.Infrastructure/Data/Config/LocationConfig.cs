using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkyTower.Core.Entities.LocationAggregate;
using SkyTower.Infrastructure.Data.Config.Abstractions;

namespace SkyTower.Infrastructure.Data.Config;

public sealed class LocationConfig : EntityConfigBase<Location>
{
	protected override void ApplyAdditionalConfiguration(EntityTypeBuilder<Location> builder)
	{
		builder.Property(d => d.Name)
			.IsRequired();
		
		builder.Property(d => d.LocalName);

		builder.Property(d => d.State)
			.HasMaxLength(2);
		
		builder.Property(d => d.CountyWarningArea)
			.HasMaxLength(3);
		
		builder.Property(d => d.Position)
			.IsRequired();

		builder.Property(d => d.ConvectiveRisk);

		builder.Property(d => d.TimeZone);
	}
}