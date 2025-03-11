using Application.DataSeeder;
using Application.EmailConfirmation;
using Domain;
using Infrustucture;
using Makeen._Planner.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Persistence;
using Persistence.Repository.Interface;
using Swashbuckle.AspNetCore.SwaggerGen;
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
            builder.Services.ConfigureJWT(builder.Configuration);
            RegisterServices(builder);
            ConvertEnumToString(builder);
        }

        public static void RegisterServices(this WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Warning);
            builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
            builder.Logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Warning);

            builder.Services.AddDbContext<DataBaseContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<JwtTokenService>();
            builder.Services.AddMemoryCache();

            builder.Services.Scan(scan => scan
                .FromAssemblyOf<IUserRepository>()
                .AddClasses()
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            builder.Services.Scan(scan => scan
                .FromAssemblyOf<IUserService>()
                .AddClasses()
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            builder.Services.AddIdentity<User, UserRole>()

                .AddEntityFrameworkStores<DataBaseContext>()
                .AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Ensure the user id claim type is set to the one we use (if needed)
                options.ClaimsIdentity.UserIdClaimType = "id";
            });
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            });

            //builder.Services.AddTransient<IEmailConfirmService, EmailConfirmService>();

            builder.Services.Configure<JsonOptions>(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new FlexibleDateTimeConverter());
            });

            // Swagger Configuration
            builder.Services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc("v1", new OpenApiInfo { Title = "Makeen Planner API", Version = "v1" });
                o.DocumentFilter<TitleFilter>();
            });
        }

        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });

            var jwtSettings = new JwtSettings();
            configuration.GetSection("JWT").Bind(jwtSettings);

            if (string.IsNullOrEmpty(jwtSettings?.Key)) throw new UnauthorizedException("JWT Key is missing in the configuration.");

            services.AddSingleton(jwtSettings); // Register JwtSettings

            // Explicitly set JWT as the default scheme
            services.AddAuthentication(options =>
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
                    OnMessageReceived = context =>
                    {
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"❌ Authentication Failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        if (context.Principal?.Identity is not ClaimsIdentity claimsIdentity)
                        {
                            Console.WriteLine("⚠️ Token validated, but no claims found!");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        }


        public static void ConvertEnumToString(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                //options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });
        }

        public static void Consoleshits(this WebApplicationBuilder builder)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("MTF6Pack God of C#");

            //Console.ForegroundColor = ConsoleColor.Yellow;
            //Console.WriteLine("-------------------------------------------------------------------------------------------");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"https://{Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork)}:{builder.Configuration["Port"]}/swagger");
            Console.ResetColor();
        }
    }
    public class TitleFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument doc, DocumentFilterContext context)
        {
            doc.Info.Title = "ّFor those who seek Success ...";
            doc.Info.Version = "Phase 1";
        }
    }
}
