using SkyTower.Core.Abstractions;

namespace SkyTower.Core.ValueObjects;

public class AuditRegister(string by, DateTimeOffset on) : ValueObject
{
	public string By { get; init; } = by;
	public DateTimeOffset On { get; init; } = on.ToUniversalTime();
	
	public AuditRegister Clone() => new AuditRegister(By, On);
}