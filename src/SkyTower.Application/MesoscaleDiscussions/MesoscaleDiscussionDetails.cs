using NetTopologySuite.Geometries;
using SkyTower.Domain.MesoscaleDiscussions;

namespace SkyTower.Application.MesoscaleDiscussions;

public class MesoscaleDiscussionDetails
{
	public DateTimeOffset Issued { get; init; }
	
	public string? AreasAffected { get; init; }
	
	public string? Concerning { get; init; }
	
	public ValidityPeriod? ValidityPeriod { get; init; }
	
	public double? ProbabilityOfWatchIssuance { get; init; }
	
	public string? Summary { get; init; }
	
	public string? Discussion { get; init; }

	public Polygon? Boundary { get; init; }
	
	public Dictionary<string, string> MostProbablePeakIntensities { get; } = [];
}