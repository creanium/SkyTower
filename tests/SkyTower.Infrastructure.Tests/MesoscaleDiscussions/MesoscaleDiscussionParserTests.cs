using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Converters;
using SkyTower.Infrastructure.MesoscaleDiscussions;

namespace SkyTower.Infrastructure.Tests.MesoscaleDiscussions;

[TestFixture]
internal sealed class MesoscaleDiscussionParserTests : InfrastructureTestBase
{
	[TestCase("2025-MD2288.txt", "2025-12-31T17:42:00-06:00")] //0542 PM CST Wed Dec 31 2025
	[TestCase("2026-MD0006.txt", "2026-01-08T05:41:00-06:00")] //0541 AM CST Thu Jan 08 2026
	[TestCase("2026-MD0087.txt", "2026-02-15T15:11:00-06:00")] //0311 PM CST Sun Feb 15 2026
	[TestCase("2026-MD0089.txt", "2026-02-16T07:29:00-06:00")] //0729 AM CST Mon Feb 16 2026
	[TestCase("2026-MD0090.txt", "2026-02-17T17:22:00-06:00")] //0522 PM CST Tue Feb 17 2026
	public void IssuedDateIsParsed(string testFile, string expectedIssuedDate)
	{
		var discussionText = GetTestFileContent<MesoscaleDiscussionParserTests>(testFile);

		var parser = new MesoscaleDiscussionParser(discussionText);
		var probability = parser.ParseIssued();
		var expectedDate = DateTimeOffset.Parse(expectedIssuedDate, DateTimeFormatInfo.InvariantInfo);

		Assert.That(probability, Is.EqualTo(expectedDate));
	}

	[TestCase("2025-MD2288.txt", null)]
	[TestCase("2026-MD0006.txt", null)]
	[TestCase("2026-MD0087.txt", "The severe weather threat for Tornado Watch 8, 9 continues.")]
	[TestCase("2026-MD0089.txt", null)]
	[TestCase("2026-MD0090.txt", null)]
	public void HeadlineIsParsed(string testFile, string? expectedHeadline)
	{
		var discussionText = GetTestFileContent<MesoscaleDiscussionParserTests>(testFile);

		var parser = new MesoscaleDiscussionParser(discussionText);
		var headline = parser.ParseHeadline();

		Assert.That(headline, Is.EqualTo(expectedHeadline));
	}
	
	[TestCase("2025-MD2288.txt")]
	[TestCase("2026-MD0006.txt")]
	[TestCase("2026-MD0087.txt")]
	[TestCase("2026-MD0089.txt")]
	[TestCase("2026-MD0090.txt")]
	public void SummaryIsParsed(string testFile)
	{
		var discussionText = GetTestFileContent<MesoscaleDiscussionParserTests>(testFile);
		var expectedSummariesJson = GetTestFileContent<MesoscaleDiscussionParserTests>("ExpectedSummaries.json");
		var expectedSummaries = JsonSerializer.Deserialize<Dictionary<string, string>>(expectedSummariesJson)!.AsReadOnly();
		Assert.That(expectedSummaries, Contains.Key(testFile), $"Expected discussions JSON must contain an entry for {testFile}");

		var parser = new MesoscaleDiscussionParser(discussionText);
		var discussion = parser.ParseSummary();

		var expectedDiscussion = expectedSummaries[testFile];
		Assert.That(discussion, Is.Not.Null);
		Assert.That(expectedDiscussion, Is.Not.Null);
		Assert.That(discussion, Is.EqualTo(expectedDiscussion));
		CollectionAssert.AreEqual(expectedDiscussion, discussion);
	}

	[TestCase("2025-MD2288.txt")]
	[TestCase("2026-MD0006.txt")]
	[TestCase("2026-MD0087.txt")]
	[TestCase("2026-MD0089.txt")]
	[TestCase("2026-MD0090.txt")]
	public void DiscussionIsParsed(string testFile)
	{
		var discussionText = GetTestFileContent<MesoscaleDiscussionParserTests>(testFile);
		var expectedDiscussionJson = GetTestFileContent<MesoscaleDiscussionParserTests>("ExpectedDiscussions.json");
		var expectedDiscussions = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(expectedDiscussionJson)!.AsReadOnly();
		Assert.That(expectedDiscussions, Contains.Key(testFile), $"Expected discussions JSON must contain an entry for {testFile}");

		var parser = new MesoscaleDiscussionParser(discussionText);
		var discussion = parser.ParseDiscussion();

		var expectedDiscussion = expectedDiscussions[testFile];
		Assert.That(discussion, Is.Not.Null);
		Assert.That(expectedDiscussion, Is.Not.Null);
		Assert.That(discussion, Has.Count.EqualTo(expectedDiscussion.Count));
		CollectionAssert.AreEqual(expectedDiscussion, discussion);
	}

	[TestCase("2025-MD2288.txt", null)]
	[TestCase("2026-MD0006.txt", 40)]
	[TestCase("2026-MD0087.txt", null)]
	[TestCase("2026-MD0089.txt", 5)]
	[TestCase("2026-MD0090.txt", 5)]
	public void WatchProbabilityPercentageIsParsed(string testFile, int? expectedProbability)
	{
		var discussionText = GetTestFileContent<MesoscaleDiscussionParserTests>(testFile);

		var parser = new MesoscaleDiscussionParser(discussionText);
		var probability = parser.ParseWatchProbability();

		Assert.That(probability, Is.EqualTo(expectedProbability));
	}

	[TestCase("2025-MD2288.txt", "portions of northern Ohio...northwestern Pennsylvania and southwestern New York")]
	[TestCase("2026-MD0006.txt", "Northwest Texas to north-central Oklahoma")]
	[TestCase("2026-MD0087.txt", "Parts of southeast GA into north FL")]
	[TestCase("2026-MD0089.txt", "Portions of the southern California Coast")]
	[TestCase("2026-MD0090.txt", "portions of eastern Nebraska into central Iowa")]
	public void AreasAffectedIsParsed(string testFile, string? expectedAreasAffected)
	{
		var discussionText = GetTestFileContent<MesoscaleDiscussionParserTests>(testFile);

		var parser = new MesoscaleDiscussionParser(discussionText);
		var areasAffected = parser.ParseAreasAffected();

		Assert.That(areasAffected, Is.EqualTo(expectedAreasAffected));
	}

	[TestCase("2025-MD2288.txt", "Snow Squall")]
	[TestCase("2026-MD0006.txt", "Severe potential...Watch possible")]
	[TestCase("2026-MD0087.txt", "Tornado Watch 8...9...")]
	[TestCase("2026-MD0089.txt", "Severe potential...Watch unlikely")]
	[TestCase("2026-MD0090.txt", "Severe potential...Watch unlikely")]
	public void ConcerningIsParsed(string testFile, string? expectedConcerning)
	{
		var discussionText = GetTestFileContent<MesoscaleDiscussionParserTests>(testFile);

		var parser = new MesoscaleDiscussionParser(discussionText);
		var areasAffected = parser.ParseConcerning();

		Assert.That(areasAffected, Is.EqualTo(expectedConcerning));
	}

	[TestCase("2025-MD2288.txt")]
	[TestCase("2026-MD0006.txt")]
	[TestCase("2026-MD0087.txt")]
	[TestCase("2026-MD0089.txt")]
	[TestCase("2026-MD0090.txt")]
	public void BoundaryIsParsed(string testFile)
	{
		var discussionText = GetTestFileContent<MesoscaleDiscussionParserTests>(testFile);

		var expectedBoundariesJson = GetTestFileContent<MesoscaleDiscussionParserTests>("ExpectedBoundaries.json");
		var expectedBoundaries = JsonSerializer.Deserialize<Dictionary<string, LineString>>(expectedBoundariesJson, _geoJsonSerializerOpts.Value)!.AsReadOnly();
		Assert.That(expectedBoundaries, Contains.Key(testFile), message: $"Expected boundaries JSON must contain an entry for {testFile}");
		var expectedRing = new LinearRing(expectedBoundaries[testFile].Coordinates);

		var parser = new MesoscaleDiscussionParser(discussionText);
		var boundary = parser.ParseBoundary();

		Assert.That(boundary, Is.Not.Null);
		Assert.Multiple(() =>
		{
			Assert.That(boundary.IsClosed, Is.True);
			Assert.That(boundary.Coordinates, Has.Length.EqualTo(expectedRing.Coordinates.Length));
			Assert.That(boundary, Is.EqualTo(expectedRing));
		});
	}

	private readonly Lazy<JsonSerializerOptions> _geoJsonSerializerOpts = new(() =>
	{
		var serializerOpts = new JsonSerializerOptions(JsonSerializerDefaults.Web);
		serializerOpts.Converters.Add(new GeoJsonConverterFactory());
		return serializerOpts;
	});
}