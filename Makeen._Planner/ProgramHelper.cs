using Application.DataSeeder;
using Domain;
using Infrastructure;
using Makeen._Planner.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Persistence;
using Persistence.Repository.Interface;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Sockets;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

namespace Makeen._Planner
{
    public static class ProgramHelper
    {
        public static void StartUp(this WebApplicationBuilder builder)
        {
            builder.ConfigureJWT();
            builder.RegisterServices();
            builder.ConfigureJsonOptions();
        }

        public static void RegisterServices(this WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole(options =>
                options.LogToStandardErrorThreshold = LogLevel.Warning);
            builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
            builder.Logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Warning);

            builder.Services.AddDbContext<DataBaseContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<JwtTokenService>();
            builder.Services.AddMemoryCache();

            builder.Services.Scan(scan => scan
                .FromAssemblyOf<IUserRepository>()
                .AddClasses(classes => classes.Where(type =>
                    !typeof(Microsoft.Extensions.Hosting.IHostedService).IsAssignableFrom(type)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            builder.Services.Scan(scan => scan
                .FromAssemblyOf<IUserService>()
                .AddClasses(classes => classes.Where(type =>
                    !typeof(Microsoft.Extensions.Hosting.IHostedService).IsAssignableFrom(type)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            builder.Services.AddIdentity<User, UserRole>()
                .AddEntityFrameworkStores<DataBaseContext>()
                .AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserIdClaimType = "id";
            });

            builder.Services.AddAuthorizationBuilder()
                .SetDefaultPolicy(new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build());

            // Authentication scheme is configured in ConfigureJWT.
        }

        public static void ConfigureJWT(this WebApplicationBuilder builder)
        {
            // Clear default inbound claim mapping.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            // Bind and register JWT settings.
            var jwtSettings = new JwtSettings();
            builder.Configuration.GetSection("JWT").Bind(jwtSettings);

            if (string.IsNullOrEmpty(jwtSettings?.Key))
                throw new UnauthorizedAccessException("JWT Key is missing in the configuration.");

            builder.Services.AddSingleton(jwtSettings);

            // Add JWT bearer authentication.
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context => Task.CompletedTask,
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"❌ Authentication Failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        if (context.Principal?.Identity is not ClaimsIdentity)
                        {
                            Console.WriteLine("⚠️ Token validated, but no claims found!");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        }

        public static void ConfigureJsonOptions(this WebApplicationBuilder builder)
        {
            // Add controllers and configure JSON serializer settings.
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });
        }

        public static void ConfigureSwagger(this WebApplicationBuilder builder)
        {
            // Configure Swagger/OpenAPI.
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Makeen Planner API", Version = "v1" });
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

        public static void ConfigureCors(this WebApplicationBuilder builder)
        {
            // Configure a permissive CORS policy.
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

        public static void LogStartupBanner(this WebApplicationBuilder builder)
        {
            // Log a startup banner with color formatting.
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("MTF6Pack God of C#");
            Console.ForegroundColor = ConsoleColor.Green;

            var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = hostEntry.AddressList
                .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork)?.ToString() ?? "localhost";
            Console.WriteLine($"https://{ipAddress}:{builder.Configuration["Port"]}/swagger");

            Console.ResetColor();
        }

        // Custom Swagger document filter.
        public class TitleFilter : IDocumentFilter
        {
            public void Apply(OpenApiDocument doc, DocumentFilterContext context)
            {
                doc.Info.Title = "ّFor those who seek Success ...";
                doc.Info.Version = "Phase 1";
            }
        }
    }
}