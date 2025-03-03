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

            //////////       // Authentication Setup
            //////////       builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //////////.AddJwtBearer(options =>
            //////////{
            //////////    options.RequireHttpsMetadata = false;
            //////////    options.SaveToken = true;
            //////////    options.TokenValidationParameters = new TokenValidationParameters
            //////////    {
            //////////        ValidateIssuer = false,
            //////////        ValidateAudience = false,
            //////////        ValidateIssuerSigningKey = true,
            //////////        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings.Key))
            //////////    };
            //////////    // 🔥 Fix the redirect issue
            //////////    options.Events = new JwtBearerEvents
            //////////    {
            //////////        OnChallenge = context =>
            //////////        {
            //////////            context.HandleResponse();
            //////////            context.Response.StatusCode = 401;
            //////////            context.Response.ContentType = "application/json";
            //////////            return context.Response.WriteAsync("{\"error\": \"Unauthorized. Please provide a valid token.\"}");
            //////////        }
            //////////    };
            //////////})
            //////////       .AddCookie()
            //////////       .AddGoogle(options =>
            //////////       {
            //////////           options.ClientId = builder.Configuration["Google:ClientId"]!;
            //////////           options.ClientSecret = builder.Configuration["Google:ClientSecret"]!;
            //////////       });

            // Database Context
            builder.Services.AddDbContext<DataBaseContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // Register JwtTokenService as Scoped before dependent services
            builder.Services.AddScoped<JwtTokenService>();

            // Register Application Services
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

            builder.Services.AddTransient<IEmailSender, EmailSender>();
            //builder.Services.AddTransient<IBaseEmailOTP, OTPSender>();

            builder.Services.AddMemoryCache();

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
            Console.WriteLine($"🔑 Loaded JWT Secret Key: {jwtSettings.Key}");

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
                        Console.WriteLine($"🔍 Token Received: {context.Token}");
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"❌ Authentication Failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        if (context.Principal?.Identity is ClaimsIdentity claimsIdentity)
                        {
                            Console.WriteLine("✅ Token successfully validated!");
                            Console.WriteLine($"📌 User ID: {claimsIdentity.FindFirst("id")?.Value}");
                            Console.WriteLine($"📌 Email: {claimsIdentity.FindFirst("email")?.Value}");
                        }
                        else
                        {
                            Console.WriteLine("⚠️ Token validated, but no claims found!");
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            Console.WriteLine("🚀 Authentication Middleware Configured!");
        }


        public static void ConvertEnumToString(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });
        }

        public static void Consoleshits(this WebApplicationBuilder builder)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Kirrrrr bokhoooooooooooooorrrrrrrrrrrrrrrrrrrrr Amo yossssssssseeeeeeeeeeeeeefffffffff");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("-------------------------------------------------------------------------------------------");

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
