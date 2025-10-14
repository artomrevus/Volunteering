using Auth.API.Extensions;
using Auth.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

var app = builder
    .AddLogging()
    .AddControllers()
    .AddSwagger()
    .AddMediator()
    .AddDbServices()
    .AddTokenServices()
    .AddHashingServices()
    .AddOpenTelemetry()
    .Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandlingMiddleware();

app.UseAuthorization();

app.MapControllers();

app.Run();