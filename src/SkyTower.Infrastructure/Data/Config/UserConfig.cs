using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkyTower.Core.Entities.UserAggregate;
using SkyTower.Infrastructure.Data.Config.Abstractions;

namespace SkyTower.Infrastructure.Data.Config;

public class UserConfig : EntityConfigBase<User>
{
	protected override void ApplyAdditionalConfiguration(EntityTypeBuilder<User> builder)
	{
		builder.Property(d => d.Username)
			.IsRequired()
			.HasMaxLength(300);

		builder.Property(d => d.Email)
			.IsRequired()
			.HasMaxLength(300);
		
		builder.Property(d => d.FirstName)
			.IsRequired()
			.HasMaxLength(100);
		
		builder.Property(d => d.LastName)
			.HasMaxLength(100);
	}
}