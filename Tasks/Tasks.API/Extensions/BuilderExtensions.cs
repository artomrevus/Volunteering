using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using Tasks.API.Filters;
using Tasks.Application.Commands;
using Tasks.Application.Interfaces.Clients;
using Tasks.Application.Interfaces.Queues;
using Tasks.Application.Interfaces.Repositories;
using Tasks.Infrastructure.Clients;
using Tasks.Infrastructure.Configuration;
using Tasks.Infrastructure.Persistence;
using Tasks.Infrastructure.Queues;
using Tasks.Infrastructure.Repositories;

namespace Tasks.API.Extensions;

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
            cfg => cfg.RegisterServicesFromAssembly(typeof(CreateTaskCommand).Assembly));
        
        return builder;
    }
    
    public static WebApplicationBuilder AddDbServices(this WebApplicationBuilder builder)
    {
        var mongoConnectionString = builder.Configuration.GetSection("MongoDbSettings:ConnectionString").Value;
        var mongoDatabaseName = builder.Configuration.GetSection("MongoDbSettings:DatabaseName").Value;

        builder.Services.AddDbContext<MongoDbContext>(options =>
            options.UseMongoDB(mongoConnectionString!, mongoDatabaseName!));

        builder.Services.AddScoped<ITasksRepository, TasksRepository>();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddQueueServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<RabbitSettings>(
            builder.Configuration.GetSection("RabbitSettings"));

        builder.Services.AddScoped<ITasksQueueSender, TasksQueueSender>();
        
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
            .Enrich.WithProperty("Microservice", "Tasks")
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
    
    public static WebApplicationBuilder AddAuth(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                };
            });

        builder.Services.AddAuthorization();
        
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
    
    public static WebApplicationBuilder AddHttpClients(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<NotificationsMicroserviceSettings>(
            builder.Configuration.GetSection("Microservices:Notifications"));

        builder.Services.AddHttpClient("NotificationsMicroservice", httpClient =>
            {
                httpClient.BaseAddress = new Uri(builder.Configuration["Microservices:Notifications:BaseUrl"]!);
                httpClient.Timeout = TimeSpan.FromSeconds(30);
            });

        builder.Services.AddTransient<INotificationsMicroserviceClient, NotificationsMicroserviceClient>();
        
        return builder;
    }

}