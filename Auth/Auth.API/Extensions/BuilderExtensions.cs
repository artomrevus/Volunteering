using Auth.API.Filters;
using Auth.Application.Commands;
using Auth.Application.Interfaces.Hashers;
using Auth.Application.Interfaces.Repositories;
using Auth.Application.Interfaces.Tokens;
using Auth.Infrastructure.Configuration;
using Auth.Infrastructure.Hashers;
using Auth.Infrastructure.Persistence;
using Auth.Infrastructure.Repositories;
using Auth.Infrastructure.Tokens;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;

namespace Auth.API.Extensions;

public static class BuilderExtensions
{
    public static WebApplicationBuilder AddControllers(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<ValidateModelStateFilter>();
        });
        
        return builder;
    }
    
    public static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddMediator(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(
            cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));
        
        return builder;
    }
    
    public static WebApplicationBuilder AddDbServices(this WebApplicationBuilder builder)
    {
        var mongoConnectionString = builder.Configuration.GetSection("MongoDbSettings:ConnectionString").Value;
        var mongoDatabaseName = builder.Configuration.GetSection("MongoDbSettings:DatabaseName").Value;

        builder.Services.AddDbContext<MongoDbContext>(options =>
            options.UseMongoDB(mongoConnectionString!, mongoDatabaseName!));
        
        builder.Services.AddScoped<IUsersRepository, UsersRepository>();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddTokenServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<JwtSettings>(
            builder.Configuration.GetSection("Jwt"));
        
        builder.Services.AddScoped<ITokensService, TokensService>();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddHashingServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
        
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
            .Enrich.WithProperty("Microservice", "Auth")
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
                .AddEntityFrameworkCoreInstrumentation()
                .AddOtlpExporter()
            );

        builder.Logging.AddOpenTelemetry(logging => logging
            .AddOtlpExporter()
        );
        
        return builder;
    }
}