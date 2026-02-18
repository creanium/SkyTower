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
	
	protected static string GetTestFileContent<TTestClass>(string relativePath)
	{
		var basePath = TestContext.CurrentContext.TestDirectory;
		var namespaceParts = typeof(TTestClass).Namespace!.Split('.');
		var namespacePath = namespaceParts[^1];
		var fullPath = Path.Combine(basePath, namespacePath, "TestFiles", relativePath);
		
		return File.ReadAllText(fullPath);
	}
}