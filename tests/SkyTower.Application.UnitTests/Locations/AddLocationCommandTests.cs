using System.Reflection;
using Ardalis.SharedKernel;
using Ardalis.Result;
using NSubstitute;
using SkyTower.Domain.Entities.LocationAggregate;
using SkyTower.Domain.Interfaces;
using SkyTower.Application.Locations.AddLocations;
using StrictId;

namespace SkyTower.Application.UnitTests.Locations;

[TestFixture]
internal sealed class AddLocationCommandTests
{
	private readonly List<Location> Locations = [];
	private readonly IRepository<Location> _locationRepository = Substitute.For<IRepository<Location>>();
	private readonly IGeospatialDataProvider _geospatialDataProvider = Substitute.For<IGeospatialDataProvider>();
	
	[OneTimeSetUp]
	public void OneTimeSetUp()
	{
		_locationRepository.AddAsync(Arg.Any<Location>(), Arg.Any<CancellationToken>())
			.Returns(callInfo =>
			{
				var location = callInfo.Arg<Location>();
				var propertyInfo = location.GetType().GetProperty("Id");
				propertyInfo = propertyInfo!.DeclaringType!.GetProperty("Id");
				propertyInfo!.SetValue(location, Id<Location>.NewId(), BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
				Locations.Add(location);

				return location;
			});
		
		_locationRepository.GetByIdAsync(Arg.Any<Id<Location>>(), Arg.Any<CancellationToken>())
			.Returns(callInfo => Locations.FirstOrDefault(c => c.Id == callInfo.Arg<Id<Location>>()));
	}
	
	[TestCase(39.7560314, -104.9929286, "America/Denver")]
	[TestCase(32.7071882, -117.1568773, "America/Los_Angeles")]
	[TestCase(33.4454856, -112.0666928, "America/Phoenix")]
	[TestCase(41.9481846, -87.655559, "America/Chicago")]
	[TestCase(38.8727332, -77.0074815, "America/New_York")]
	public async Task AddLocation_DeterminesTimeZone(double latitude, double longitude, string expectedTimeZoneId)
	{
		_geospatialDataProvider.GetTimeZoneInfo(latitude, longitude)
			.Returns(Result<TimeZoneInfo>.Success(TimeZoneInfo.FindSystemTimeZoneById(expectedTimeZoneId)));
		
		var command = new AddLocationCommand("Test Location", latitude, longitude);
		
		var response = await new AddLocationCommandHandler(_locationRepository, _geospatialDataProvider)
			.Handle(command, CancellationToken.None)
			.ConfigureAwait(false);
		
		Assert.That(response.IsSuccess, Is.True);
		
		var location = Locations.FirstOrDefault(l => l.Id == response.Value);
		Assert.That(location, Is.Not.Null);
		Assert.That(location.TimeZone.Id, Is.EqualTo(expectedTimeZoneId));
	}
}