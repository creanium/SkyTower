namespace SkyTower.Domain.MonitoredLocations;

#pragma warning disable S2342

[Flags]
public enum DaysOfWeek
{
	None = 0,
	Sunday = 1 << 0,
	Monday = 1 << 1,
	Tuesday = 1 << 2,
	Wednesday = 1 << 3,
	Thursday = 1 << 4,
	Friday = 1 << 5,
	Saturday = 1 << 6,
	Daily = Sunday + Monday + Tuesday + Wednesday + Thursday + Friday + Saturday,
	Weekdays = Monday + Tuesday + Wednesday + Thursday + Friday,
	Weekends = Sunday + Saturday
}