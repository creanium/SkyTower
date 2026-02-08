using NetTopologySuite.Geometries;
using SkyTower.Domain.Abstractions;
using SkyTower.Domain.MesoscaleDiscussions.Events;

namespace SkyTower.Domain.MesoscaleDiscussions;

public sealed class MesoscaleDiscussion : Entity<MesoscaleDiscussion>
{
	public DateTimeOffset Issued { get; private set; }

	public DateTimeOffset? Updated { get; private set; }

	public int Year { get; private set; }

	public int Number { get; private set; }

	public Polygon Boundary { get; private set; } = null!;

	public ValidityPeriod ValidityPeriod { get; private set; } = null!;

	public Uri GraphicProductUri { get; private set; } = null!;

	public Uri ImageUri { get; private set; } = null!;

	public string RawContent { get; private set; } = null!;

	public string? AreasAffected { get; private set; }

	public string? Concerning { get; private set; }

	public string? Summary { get; private set; }

	public string? Discussion { get; private set; }

	public static MesoscaleDiscussion Create(
		DateTimeOffset issued,
		int number,
		Polygon boundary,
		ValidityPeriod validityPeriod,
		Uri graphicProductUri,
		Uri imageUri,
		string rawContent
	)
	{
		var discussion = new MesoscaleDiscussion
		{
			Issued = issued,
			Year = issued.Year,
			Number = number,
			Boundary = boundary,
			ValidityPeriod = validityPeriod,
			GraphicProductUri = graphicProductUri,
			ImageUri = imageUri,
			RawContent = rawContent
		};
		
		discussion.RaiseDomainEvent(new MesoscaleDiscussionIssuedDomainEvent(discussion.Id));
		
		return discussion;
	}
}