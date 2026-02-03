using SkyTower.Domain.Abstractions;

namespace SkyTower.Domain.ValueObjects;

public class AuditRegister(string by, DateTimeOffset on) : ValueObject
{
	public string By { get; init; } = by;
	public DateTimeOffset On { get; init; } = on.ToUniversalTime();
	
	public AuditRegister Clone() => new AuditRegister(By, On);
}