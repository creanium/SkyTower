using System.Text.Json;
using NetTopologySuite.Geometries;
using SkyTower.Infrastructure.MesoscaleDiscussions;

namespace SkyTower.Infrastructure.Tests.MesoscaleDiscussions;

[TestFixture]
internal sealed class MesoscaleDiscussionParserTests : InfrastructureTestBase
{
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
	
	[TestCase("2026-MD0087.txt", "2026-MD0087-Discussion.json")]
	[TestCase("2026-MD0089.txt", "2026-MD0089-Discussion.json")]
	[TestCase("2026-MD0090.txt", "2026-MD0090-Discussion.json")]
	public void DiscussionIsParsed(string testFile, string expectedResultFile)
	{
		var discussionText = GetTestFileContent<MesoscaleDiscussionParserTests>(testFile);
		var expectedDiscussionJson = GetTestFileContent<MesoscaleDiscussionParserTests>(expectedResultFile);
		var expectedDiscussion = JsonSerializer.Deserialize<List<string>>(expectedDiscussionJson)!.AsReadOnly();
		
		var parser = new MesoscaleDiscussionParser(discussionText);
		var discussion = parser.ParseDiscussion();

		Assert.That(discussion, Is.Not.Null);
		Assert.That(expectedDiscussion, Is.Not.Null);
		Assert.That(discussion, Has.Count.EqualTo(expectedDiscussion.Count));
		CollectionAssert.AreEqual(expectedDiscussion, discussion);
	}
	
	[TestCase("2026-MD0006.txt", 40)]
	[TestCase("2026-MD0087.txt", null)]
	[TestCase("2026-MD0089.txt", 5)]
	[TestCase("2026-MD0090.txt", 5)]
	public void CanParseWatchProbability(string testFile, int? expectedProbability)
	{
		var discussionText = GetTestFileContent<MesoscaleDiscussionParserTests>(testFile);
		
		var parser = new MesoscaleDiscussionParser(discussionText);
		var probability = parser.ParseWatchProbability();

		Assert.That(probability, Is.EqualTo(expectedProbability));
	}
	
	[TestCase("2026-MD0089.txt")]
	public void BoundaryIsParsed(string testFile)
	{
		var discussionText = GetTestFileContent<MesoscaleDiscussionParserTests>(testFile);
		
		var expectedRing = new LinearRing([
			new Coordinate(-120.47, 34.41),
			new Coordinate(-120.66, 34.54),
			new Coordinate(-120.67, 34.64),
			new Coordinate(-120.64, 34.86),
			new Coordinate(-120.66, 35.06),
			new Coordinate(-120.85, 35.18),
			new Coordinate(-120.97, 35.29),
			new Coordinate(-121.04, 35.44),
			new Coordinate(-121.03, 35.62),
			new Coordinate(-120.89, 35.69),
			new Coordinate(-120.74, 35.62),
			new Coordinate(-120.42, 35.21),
			new Coordinate(-120.25, 34.86),
			new Coordinate(-120.10, 34.66),
			new Coordinate(-120.05, 34.47),
			new Coordinate(-120.16, 34.42),
			new Coordinate(-120.47, 34.41)
		]);
		
		var parser = new MesoscaleDiscussionParser(discussionText);
		var boundary = parser.ParseBoundary();

		Assert.That(boundary, Is.Not.Null);
		Assert.Multiple(() =>
		{
			Assert.That(boundary.IsClosed, Is.True);
			Assert.That(boundary.Coordinates, Has.Length.EqualTo(17));
			Assert.That(boundary, Is.EqualTo(expectedRing));
		});
	}
}