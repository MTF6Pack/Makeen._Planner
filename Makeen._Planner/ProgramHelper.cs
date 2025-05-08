using Application.Contracts.Groups;
using Application.DataSeeder;
using Application.Notification_Service.BackGroundServices;
using Application.Task_Service;
using Domain;
using Infrastructure;
using Infrastructure.Date_and_Time;
using Infrastructure.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Persistence;
using Persistence.Repository;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text.Json.Serialization;
using static Persistence.Dapper;

namespace Makeen._Planner
{
    public static class ProgramHelper
    {
        public static void ConfigureAppSettings(this WebApplicationBuilder builder)
        {
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        }
        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.ConfigureCors();
            builder.StartUp();
            builder.ConfigureSwagger();
            builder.Services.AddControllers(options => { options.ModelBinderProviders.Insert(0, new PersianDateModelBinderProvider()); });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                if (!c.SwaggerGeneratorOptions.SecuritySchemes.ContainsKey("Bearer"))
                {
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Enter 'Bearer {your_token_here}'",
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "Bearer"
                    });
                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });
                }
                c.OrderActionsBy(apiDesc => apiDesc.GroupName);
            });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSignalR().AddHubOptions<NotificationHub>(options => { options.EnableDetailedErrors = true; }).AddJsonProtocol();
            builder.Services.AddHostedService<TaskReminderBackgroundService>();
            builder.Services.AddHostedService<NotificationRetryService>();
        }
        public static void ConfigureCulture()
        {
            var persianCulture = new CultureInfo("fa-IR")
            {
                DateTimeFormat = { Calendar = new PersianCalendar() }
            };
            CultureInfo.DefaultThreadCurrentCulture = persianCulture;
            CultureInfo.DefaultThreadCurrentUICulture = persianCulture;
        }

        public static void StartUp(this WebApplicationBuilder builder)
        {
            builder.ConfigureLogging();
            builder.InjectDependencies();
            builder.ConfigureAuthenticationAndIdentity();
            builder.ConfigureJsonOptions();
            builder.LogStartupBanner();
        }
        public static void InjectDependencies(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<DataBaseContext>((services, options) =>
     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
             //.AddInterceptors(new NotificationInterceptor(builder.Services.BuildServiceProvider().GetRequiredService<IMediator>()))
             );

            builder.Services.AddScoped<GroupMapper>();
            builder.Services.AddScoped<JwtTokenService>();
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(NotificationSender).Assembly));
            builder.Services.AddScoped<NotificationQueryService>();
            builder.Services.AddScoped<NotificationSenderHandler>();
            builder.Services.AddMemoryCache();

            builder.Services.Scan(scan => scan
    .FromAssemblies(
        typeof(TaskService).Assembly,
        typeof(FileStorageService).Assembly,
        typeof(TaskRepository).Assembly,
        typeof(GroupMapper).Assembly
    )
    .AddClasses(classes =>
    classes.Where(type =>
        !type.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase) &&
        !type.Name.Contains("Middleware") &&
        !typeof(ControllerBase).IsAssignableFrom(type) &&
        !typeof(IHostedService).IsAssignableFrom(type) &&
        !typeof(Exception).IsAssignableFrom(type) &&
        !typeof(Attribute).IsAssignableFrom(type) &&
        !typeof(IMiddleware).IsAssignableFrom(type)))
    .AsImplementedInterfaces()
    .WithScopedLifetime());

        }
        public static void ConfigureJsonOptions(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new FlexibleDateTimeConverter());
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });
        }
        public static void ConfigureCors(this WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", policy =>
                {
                    policy.SetIsOriginAllowed(_ => true)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });
        }
        public static void ConfigureSwagger(this WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Makeen Planner API",
                    Version = "v1"
                });
                options.DocumentFilter<TitleFilter>();
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Description = "Enter 'Bearer' followed by a space and your token"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }
        private class TitleFilter : IDocumentFilter
        {
            public void Apply(OpenApiDocument doc, DocumentFilterContext context)
            {
                doc.Info.Title = "For those who seek Success ...";
                doc.Info.Version = "Phase 1";
            }
        }
        public static void LogStartupBanner(this WebApplicationBuilder builder)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("MTF6Pack God of C#");
            Console.ForegroundColor = ConsoleColor.Green;

            var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = hostEntry.AddressList
                .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork)?.ToString() ?? "localhost";
            Console.WriteLine($"https://{ipAddress}:{builder.Configuration["Port"]}/swagger");

            Console.ResetColor();
        }
        public static void ConfigureLogging(this WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            builder.Logging.SetMinimumLevel(LogLevel.Warning);
            builder.Logging.AddConsole();
            builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
            builder.Logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Warning);
        }
        public static void ConfigureMiddleware(this WebApplication app)
        {
            app.UseMiddleware<GlobalExceptionMiddleware>();
            //app.UseMiddleware<WebSocketAuthMiddleware>();
            app.UseWebSockets(new WebSocketOptions { AllowedOrigins = { "*" } });
            app.Use(async (context, next) =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    Console.WriteLine($"WebSocket request to: {context.Request.Path}");
                }
                await next();
            });

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DataBaseContext>();
                if (dbContext.Database.GetPendingMigrations().Any()) dbContext.Database.Migrate();
            }

            app.Urls.Add($"https://*:{app.Configuration["Port"]}");

            if (app.Environment.IsDevelopment())
            {
                var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
                var ipAddress = hostEntry.AddressList
                    .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork)?.ToString() ?? "localhost";
                Process.Start("cmd", $"/c start https://{ipAddress}:{app.Configuration["Port"]}/swagger");
                Process.Start("cmd", $"/c start https://{ipAddress}:{app.Configuration["Port"]}/swagger");
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors("AllowAllOrigins");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<NotificationHub>("/notificationhub");

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.EnableTryItOutByDefault(); // ✅ Valid here
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                options.OAuthClientId("your-client-id"); // Optional, only if using OAuth
            });

            app.MapControllers();
        }

        public static void ConfigureAuthenticationAndIdentity(this WebApplicationBuilder builder)
        {
            builder.Services.AddCustomIdentity(builder.Configuration);
            builder.Services.AddJwtAuthentication(builder.Configuration);

            builder.Services.AddAuthorizationBuilder()
                .SetDefaultPolicy(new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build());
        }
    }
}