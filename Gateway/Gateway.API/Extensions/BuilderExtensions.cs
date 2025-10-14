using Ocelot.DependencyInjection;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;

namespace Gateway.API.Extensions;

public static class BuilderExtensions
{
    public static WebApplicationBuilder AddOcelot(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
        builder.Services.AddOcelot();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policyBuilder => policyBuilder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
            );
        });
        
        return builder;
    }
    
    public static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
            
        var mongoConnectionString = builder.Configuration.GetSection("MongoDbSettings:ConnectionString").Value;
        var mongoDatabaseName = builder.Configuration.GetSection("MongoDbSettings:DatabaseName").Value;
        
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.WithProperty("Microservice", "Gateway")
            .WriteTo.Console()
            .WriteTo.MongoDB(
                databaseUrl: $"{mongoConnectionString}/{mongoDatabaseName}",
                collectionName: "Logs",
                restrictedToMinimumLevel: LogEventLevel.Information
            )
            .WriteTo.OpenTelemetry()
            .CreateLogger();
        
        builder.Host.UseSerilog();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(builder.Configuration["OTEL_SERVICE_NAME"]!))
            .WithMetrics(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter()
            )
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter()
            );

        builder.Logging.AddOpenTelemetry(logging => logging
            .AddOtlpExporter()
        );
        
        return builder;
    }
}