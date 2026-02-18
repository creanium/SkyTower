using NetTopologySuite.Geometries;
using SkyTower.Domain.MesoscaleDiscussions;

namespace SkyTower.Application.MesoscaleDiscussions;

public class MesoscaleDiscussionDetails
{
	public DateTimeOffset Issued { get; init; }
	
	public string? AreasAffected { get; init; }
	
	public string? Concerning { get; init; }
	
	public ValidityPeriod? ValidityPeriod { get; init; }
	
	public double? WatchProbability { get; init; }
	
	public string? Headline { get; init; }
	
	public string? Summary { get; init; }
	
	public IReadOnlyList<string>? Discussion { get; init; }

	public LinearRing? Boundary { get; init; }
	
	public Dictionary<string, string> MostProbablePeakIntensities { get; } = [];
}