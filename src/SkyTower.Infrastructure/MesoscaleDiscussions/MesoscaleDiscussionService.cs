using System.Globalization;
using System.Text.RegularExpressions;
using Ardalis.GuardClauses;
using SkyTower.Application.MesoscaleDiscussions;
using SkyTower.Domain.MesoscaleDiscussions;

namespace SkyTower.Infrastructure.MesoscaleDiscussions;

public partial class MesoscaleDiscussionService : IMesoscaleDiscussionService
{
	public Task FetchNewDiscussions()
	{
		throw new NotImplementedException();
	}

	public Task<MesoscaleDiscussionSummary> GetDiscussion(int year, int number)
	{
		throw new NotImplementedException();
	}

	public MesoscaleDiscussionDetails ParseDiscussionText(string rawText)
	{
		var toParse = Guard.Against.NullOrWhiteSpace(rawText).Trim();
		var parser = new MesoscaleDiscussionParser(toParse);
		var issuedDate = parser.ParseIssued();

		// validity period: ddhhmmZ - ddhhmmZ
		var details = new MesoscaleDiscussionDetails
		{
			Issued = issuedDate,
			AreasAffected =  parser.ParseAreasAffected(),
			Concerning = parser.ParseConcerning(),
			ValidityPeriod = parser.ParseValidityPeriod(issuedDate),
			WatchProbability =  parser.ParseWatchProbability(),
			Headline = parser.ParseHeadline(),
			Summary = parser.ParseSummary(),
			Discussion = parser.ParseDiscussion(),
			Boundary = parser.ParseBoundary()
		};

		return details;
	}
}