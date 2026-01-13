using System.Diagnostics.CodeAnalysis;

namespace SkyTower.Worker;

[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Class is instantiated via dependency injection")]
internal sealed partial class ServiceWorker(ILogger<ServiceWorker> logger) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			LogWorkerRunningAtTime(DateTimeOffset.Now);
			await Task.Delay(1000, stoppingToken).ConfigureAwait(false);
		}
	}

	[LoggerMessage(LogLevel.Information, "Worker running at: {time}")]
	partial void LogWorkerRunningAtTime(DateTimeOffset time);
}