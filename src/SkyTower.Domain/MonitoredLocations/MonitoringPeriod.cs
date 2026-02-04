namespace SkyTower.Domain.MonitoredLocations;

public record MonitoringPeriod(DateOnly? NotBefore, DateOnly? NotAfter);