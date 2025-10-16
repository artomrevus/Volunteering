using Microsoft.EntityFrameworkCore;
using Tasks.API.Extensions;
using Tasks.API.Filters;
using Tasks.API.Middleware;
using Tasks.Application.Commands;
using Tasks.Application.Interfaces;
using Tasks.Infrastructure.Persistence;
using Tasks.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var app = builder
    .AddLogging()
    .AddOpenTelemetry()
    .AddAuth()
    .AddControllers()
    .AddSwagger()
    .AddMediator()
    .AddDbServices()
    .AddQueueServices()
    .AddHttpClients()
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