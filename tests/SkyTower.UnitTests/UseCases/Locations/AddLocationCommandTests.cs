using System.Reflection;
using Ardalis.SharedKernel;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using SkyTower.Core.Entities.LocationAggregate;
using SkyTower.Core.Interfaces;
using SkyTower.Infrastructure.Geo;
using SkyTower.UseCases.Locations.AddLocations;
using StrictId;

namespace SkyTower.UnitTests.UseCases.Locations;

[TestFixture]
internal sealed class AddLocationCommandTests
{
	private ServiceCollection _serviceCollection = [];
	private ServiceProvider _serviceProvider = null!;
	private List<Location> Locations = [];
	private IRepository<Location> _locationRepository = Substitute.For<IRepository<Location>>();
	
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

		_serviceProvider = _serviceCollection
			.AddLogging()
			.AddScoped<IRepository<Location>>(_ => _locationRepository)
			.AddTransient<IGeospatialDataProvider, GeospatialDataProvider>()
			.AddMediatR(cfg =>
			{
				cfg.LicenseKey =
					"eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxNzk5OTcxMjAwIiwiaWF0IjoiMTc2ODQ1MTAzNyIsImFjY291bnRfaWQiOiIwMTliYmZlNTFlZTI3N2ViOTJmZGVjOGFmNjhmMjdmZiIsImN1c3RvbWVyX2lkIjoiY3RtXzAxa2V6eWFwYThxMzBwZmd6YnJrYWZmNzE0Iiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.wmNCjmkP9Dsb7Guk8vWaXXueLA_Vy3btDJyZbl5YRVKezTzRWq27_H28uyGJDvRFXDa8Bhuczm6G1dDqAUPu8nq8hxgi7eDCVcpInGnoNY8Rp2W4gvuwIW1YzVRNnAs7t3dW9BQb2YbZH4k3Cc8feZAkUEZJaAtVh93tuea4SyIwPEkhvm1s_0xSK-NELKr_rz7Sl8fgDFQgKQlMWznnNTVCAVkw6u-EHtFe_7Jdk-RHIAha4XQWXHjS7SjvNFwf4TKVYGdskgWfHagSWOi8-g5zVb841MxmniXj62yHRCtLnUgXepA3I7ppfDPzICr3ZpRHp94pUcfTphK0d_RuxA";
				cfg.RegisterServicesFromAssemblies(typeof(AddLocationCommand).Assembly);
			})
			.BuildServiceProvider();
	}

	[OneTimeTearDown]
	public void OneTimeTearDown()
	{
		_serviceProvider.Dispose();
	}
	
	[TestCase(39.7560314, -104.9929286, "America/Denver")]
	[TestCase(32.7071882, -117.1568773, "America/Los_Angeles")]
	[TestCase(33.4454856, -112.0666928, "America/Phoenix")]
	[TestCase(41.9481846, -87.655559, "America/Chicago")]
	[TestCase(38.8727332, -77.0074815, "America/New_York")]
	public async Task AddLocation_DeterminesTimeZone(double latitude, double longitude, string expectedTimeZoneId)
	{
		var command = new AddLocationCommand("Test Location", latitude, longitude);
		
		var mediator = _serviceProvider.GetRequiredService<IMediator>();
		var response = await mediator.Send(command).ConfigureAwait(false);
		
		Assert.That(response.IsSuccess, Is.True);
		
		var location = Locations.FirstOrDefault(l => l.Id == response.Value);
		Assert.That(location, Is.Not.Null);
		Assert.That(location.TimeZone.Id, Is.EqualTo(expectedTimeZoneId));
	}
}