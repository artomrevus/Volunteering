using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Notifications.API.Background;
using Notifications.API.Filters;
using Notifications.Application.Commands;
using Notifications.Application.Interfaces.Notifications;
using Notifications.Application.Interfaces.Queues;
using Notifications.Application.Interfaces.Repositories;
using Notifications.Infrastructure.Configuration;
using Notifications.Infrastructure.Notifications;
using Notifications.Infrastructure.Persistence;
using Notifications.Infrastructure.Queues;
using Notifications.Infrastructure.Repositories;
using SendGrid.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace Notifications.API.Extensions;

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
            cfg => cfg.RegisterServicesFromAssembly(typeof(CreateBindingCommand).Assembly));
        
        return builder;
    }
    
    public static WebApplicationBuilder AddDbServices(this WebApplicationBuilder builder)
    {
        var mongoConnectionString = builder.Configuration.GetSection("MongoDbSettings:ConnectionString").Value;
        var mongoDatabaseName = builder.Configuration.GetSection("MongoDbSettings:DatabaseName").Value;

        builder.Services.AddDbContext<MongoDbContext>(options =>
            options.UseMongoDB(mongoConnectionString!, mongoDatabaseName!));
        
        builder.Services.AddScoped<IBindingsRepository, BindingsRepository>();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddEmailServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<SendGridEmailSettings>(
            builder.Configuration.GetSection("SendGridEmailSettings"));

        builder.Services.AddSendGrid(options =>
        {
            options.ApiKey = builder.Configuration["SendGridEmailSettings:SendGridApiKey"];
        });

        builder.Services.AddScoped<IEmailSender, SendGridEmailSender>();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddQueueServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<RabbitSettings>(
            builder.Configuration.GetSection("RabbitSettings"));

        builder.Services.AddScoped<ITasksQueueConsumer, TasksQueueConsumer>();
        
        builder.Services.AddHostedService<TasksQueueConsumerHostedService>();
        
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
            .Enrich.WithProperty("Microservice", "Notifications")
            .WriteTo.Console()
            .WriteTo.MongoDB(
                databaseUrl: $"{mongoConnectionString}/{mongoDatabaseName}",
                collectionName: "Logs",
                restrictedToMinimumLevel: LogEventLevel.Information
            )
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
}