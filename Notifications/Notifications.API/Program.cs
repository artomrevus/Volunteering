using Notifications.API.Extensions;
using Notifications.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

var app = builder
    .AddLogging()
    .AddOpenTelemetry()
    .AddAuth()
    .AddControllers()
    .AddSwagger()
    .AddMediator()
    .AddDbServices()
    .AddEmailServices()
    .AddQueueServices()
    .Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandlingMiddleware();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();