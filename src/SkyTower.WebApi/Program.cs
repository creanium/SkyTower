using FastEndpoints;
using SkyTower.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddFastEndpoints();

var appDbConnectionString = builder.Configuration.GetConnectionString("Application") ??
                            throw new InvalidOperationException("Application connection string is not configured.");
builder.Services.AddApplicationDbContext(appDbConnectionString);

var app = builder.Build();

app.UseFastEndpoints();

await app.RunAsync().ConfigureAwait(false);