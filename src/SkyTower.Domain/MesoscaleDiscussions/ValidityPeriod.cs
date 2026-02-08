using SkyTower.Domain.Abstractions;

namespace SkyTower.Domain.MesoscaleDiscussions;

public class ValidityPeriod : ValueObject
{
	public DateTimeOffset ValidFrom { get; init; }
	public DateTimeOffset ValidTo { get; init; }

	public ValidityPeriod(DateTimeOffset ValidFrom, DateTimeOffset ValidTo)
	{
		if (ValidTo <= ValidFrom)
		{
			throw new ArgumentOutOfRangeException(nameof(ValidTo), "ValidTo must be after ValidFrom.");
		}

		this.ValidFrom = ValidFrom;
		this.ValidTo = ValidTo;
	}

	public void Deconstruct(out DateTimeOffset validFrom, out DateTimeOffset validTo)
	{
		validFrom = ValidFrom;
		validTo = ValidTo;
	}
}