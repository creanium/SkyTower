using FastEndpoints;

namespace SkyTower.WebApi.Endpoints.Observations;

internal sealed class WundergroundEndpoint : Endpoint<WundergroundRequest>
{
	public override void Configure()
	{
		Get("/api/observations/wunderground");
		AllowAnonymous();
	}
}