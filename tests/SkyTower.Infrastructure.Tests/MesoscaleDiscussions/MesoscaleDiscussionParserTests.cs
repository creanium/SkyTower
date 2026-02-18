using NetTopologySuite.Geometries;
using SkyTower.Infrastructure.MesoscaleDiscussions;

namespace SkyTower.Infrastructure.Tests.MesoscaleDiscussions;

[TestFixture]
internal sealed class MesoscaleDiscussionParserTests : InfrastructureTestBase
{
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