using Gateway.API.Extensions;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

var app = builder
    .AddOcelot()
    .AddCors()
    .AddLogging()
    .AddOpenTelemetry()
    .Build();

// ------------------------------------
// Configure the HTTP request pipeline:
// ------------------------------------

app.UseCors("AllowAll");

await app.UseOcelot();

app.Run();