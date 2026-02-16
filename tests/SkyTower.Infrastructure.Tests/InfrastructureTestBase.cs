using Microsoft.Extensions.DependencyInjection;
using SkyTower.Application.MesoscaleDiscussions;
using SkyTower.Infrastructure.MesoscaleDiscussions;

namespace SkyTower.Infrastructure.Tests;

[SetUpFixture]
internal abstract class InfrastructureTestBase 
{
	protected IServiceCollection Services { get; private set; } = new ServiceCollection();
	protected ServiceProvider ServiceProvider { get; private set; }
	
	[OneTimeSetUp]
	protected void SetUpFixture()
	{
		Services.AddTransient<IMesoscaleDiscussionService, MesoscaleDiscussionService>();
		
		ServiceProvider = Services.BuildServiceProvider();
	}

	[OneTimeTearDown]
	protected void TearDownFixture()
	{
		ServiceProvider.Dispose();
	}
}