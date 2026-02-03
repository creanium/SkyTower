namespace SkyTower.Domain.Abstractions;

public class AuditRegister(string by, DateTimeOffset on) : ValueObject
{
	public string By { get; init; } = by;
	public DateTimeOffset On { get; init; } = on.ToUniversalTime();
	
	public AuditRegister Clone() => new AuditRegister(By, On);
}