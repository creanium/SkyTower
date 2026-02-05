namespace SkyTower.Domain.MonitoredLocations;

public record MonitoringPeriod
{
	public DateOnly? NotBefore { get; init; }
	public DateOnly? NotAfter { get; init; }

	public MonitoringPeriod(DateOnly? NotBefore, DateOnly? NotAfter)
	{
		if (NotAfter <= NotBefore)
		{
			throw new ArgumentOutOfRangeException(nameof(NotAfter), "NotAfter must be after NotBefore.");
		}

		this.NotBefore = NotBefore;
		this.NotAfter = NotAfter;
	}

	public void Deconstruct(out DateOnly? notBefore, out DateOnly? notAfter)
	{
		notBefore = NotBefore;
		notAfter = NotAfter;
	}
}