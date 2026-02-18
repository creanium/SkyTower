using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using SkyTower.Application.MesoscaleDiscussions;
using SkyTower.Domain.MesoscaleDiscussions;

namespace SkyTower.Infrastructure.Tests.MesoscaleDiscussions;

[TestFixture]
internal sealed class MesoscaleDiscussionServiceTests : InfrastructureTestBase
{
	private IMesoscaleDiscussionService _service;

	[OneTimeSetUp]
	public void Setup()
	{
		_service = ServiceProvider.GetRequiredService<IMesoscaleDiscussionService>();
	}

	[Test]
	public void DiscussionTextIsParsedIntoParts()
	{
		const string discussionText =
			"""
						
			Mesoscale Discussion 0089
			NWS Storm Prediction Center Norman OK
			0729 AM CST Mon Feb 16 2026

			Areas affected...Portions of the southern California Coast

			Concerning...Severe potential...Watch unlikely 

			Valid 161329Z - 161530Z

			Probability of Watch Issuance...5 percent

			SUMMARY...Weak convective showers may pose a threat for waterspouts
			and damaging winds along portions of the southern California coast
			through mid-morning.

			DISCUSSION...Latest radar imagery from KVBX shows shallow convective
			cells moving northward ahead of an eastward migrating convective
			band within a narrow plume of warm air advection. Several of these
			cells show weak rotation per velocity imagery, and while too shallow
			for substantial lightning production, may be capable of brief/weak
			waterspouts given nearly 450 m2/s2 0-1 km SRH sampled by the nearby
			KVBX VWP. These cells will gradually approach the coastline of
			western Santa Barbara and San Luis Obispo counties in the next few
			hours and may pose a risk of waterspouts and damaging winds along
			the shore. This threat is expected to remain fairly spatially
			limited to coastal areas given very limited buoyancy further inland,
			at least for the next few hours before cold temperatures aloft
			spread east. Regardless, the spatial/temporal threat will likely
			remain sufficiently limited to preclude watch issuance.

			..Moore/Smith.. 02/16/2026

			...Please see www.spc.noaa.gov for graphic product...

			ATTN...WFO...LOX...

			LAT...LON   34412047 34542066 34642067 34862064 35062066 35182085
			            35292097 35442104 35622103 35692089 35622074 35212042
			            34862025 34662010 34472005 34422016 34412047 

			MOST PROBABLE PEAK TORNADO INTENSITY...UP TO 95 MPH
			MOST PROBABLE PEAK WIND GUST...UP TO 60 MPH
						
						
			""";
		var details = _service.ParseDiscussionText(discussionText);

		var expectedValidDate = new DateOnly(2026, 02, 16);
		var expectedValidFrom = new DateTimeOffset(expectedValidDate, new TimeOnly(13, 29), TimeSpan.Zero);
		var expectedValidTo = new DateTimeOffset(expectedValidDate, new TimeOnly(15, 30), TimeSpan.Zero);
		var expectedValidityPeriod = new ValidityPeriod(expectedValidFrom, expectedValidTo);

		Assert.That(details, Is.Not.Null);
		Assert.That(details.Issued, Is.EqualTo(new DateTimeOffset(2026, 02, 16, 7, 29, 0, TimeSpan.FromHours(-6))));
		Assert.That(details.ValidityPeriod, Is.EqualTo(expectedValidityPeriod));
	}

	[TestCase("2025-12-31T23:29:00-06:00",
		"2026-01-01T05:29:00Z",
		"2026-01-01T15:30:00Z",
		"""
		Mesoscale Discussion 0089
		NWS Storm Prediction Center Norman OK
		1129 PM CST Wed Dec 31 2025

		Valid 010529Z - 011530Z

		Dummy text
		""")]
	[TestCase("2026-01-31T11:29:00-06:00",
		"2026-01-31T23:29:00Z",
		"2026-02-02T12:15:00Z",
		"""
		Mesoscale Discussion 0089
		NWS Storm Prediction Center Norman OK
		1129 AM CST Sat Jan 31 2026

		Valid 312329Z - 021215Z

		Dummy text
		""")]
	[TestCase("2025-12-31T14:14:00-06:00",
		"2025-12-31T20:14:00Z",
		"2026-01-01T08:30:00Z",
		"""
		Mesoscale Discussion 0001
		NWS Storm Prediction Center Norman OK
		0214 PM CST Wed Dec 31 2025

		Valid 312014Z - 010830Z

		Dummy text
		""")]
	public void ValidityPeriodIsParsedCorrectly(string expectedIssued, string expectedStart, string expectedEnd, string discussionText)
	{
		var details = _service.ParseDiscussionText(discussionText);

		var expectedValidityPeriod = new ValidityPeriod(DateTimeOffset.Parse(expectedStart, DateTimeFormatInfo.InvariantInfo), DateTimeOffset.Parse(expectedEnd, DateTimeFormatInfo.InvariantInfo));

		Assert.That(details, Is.Not.Null);
		Assert.That(details.Issued, Is.EqualTo(DateTimeOffset.Parse(expectedIssued, DateTimeFormatInfo.InvariantInfo)));
		Assert.That(details.ValidityPeriod, Is.EqualTo(expectedValidityPeriod));
	}
}