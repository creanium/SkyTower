using Ardalis.GuardClauses;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkyTower.Core.Abstractions;
using StrictId.EFCore;

namespace SkyTower.Infrastructure.Data.Config.Abstractions;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public abstract class EntityConfigBase<TImplementation> : IEntityTypeConfiguration<TImplementation>
	where TImplementation : Entity<TImplementation>
{
	/// <summary>
	/// Base configuration for entities
	/// </summary>
	/// <param name="builder"></param>
	public void Configure(EntityTypeBuilder<TImplementation> builder)
	{
		Guard.Against.Null(builder);
		
		builder.HasKey(d => d.Id);
		builder.Property(d => d.Id)
			.ValueGeneratedOnAdd()
			.HasStrictIdValueGenerator()
			.HasColumnOrder(1);

		builder.OwnsOne(n => n.Created, b =>
		{
			b.Property(c => c.On)
				.IsRequired()
				.HasColumnName("Created")
				.HasColumnOrder(2);

			b.Property(c => c.By)
				.IsRequired()
				.HasMaxLength(100)
				.HasColumnName("CreatedBy")
				.HasColumnOrder(3);
		});

		builder.OwnsOne(n => n.LastModified, b =>
		{
			b.Property(c => c.On)
				.IsRequired()
				.HasColumnName("LastModified")
				.HasColumnOrder(4);

			b.Property(c => c.By)
				.IsRequired()
				.HasMaxLength(100)
				.HasColumnName("LastModifiedBy")
				.HasColumnOrder(5);
		});

		ApplyAdditionalConfiguration(builder);
	}

	/// <summary>
	/// Called by the base configuration to allow for additional fields to be configured in the derived class
	/// </summary>
	/// <param name="builder"></param>
	protected abstract void ApplyAdditionalConfiguration(EntityTypeBuilder<TImplementation> builder);
}