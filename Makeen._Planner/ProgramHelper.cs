using Application.DataSeeder;
using Application.EmailConfirmation;
using Domain;
using Infrustucture;
using Makeen._Planner.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
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
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using Application.UserAndOtp;
using Application.DataSeeder.OTP;

namespace Makeen._Planner
{
    public static class ProgramHelper
    {
        public static void StartUp(this WebApplicationBuilder builder)
        {
            ConfigureJWT(builder);
            RegisterServices(builder);
            ConvertEnumToString(builder);
        }

        public static void RegisterServices(this WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Warning);
            builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
            builder.Logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Warning);

            // Authentication Setup
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGoogle(options =>
            {
                options.ClientId = builder.Configuration["Google:ClientId"]!;
                options.ClientSecret = builder.Configuration["Google:ClientSecret"]!;
            });

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

        public static void ConfigureJWT(this WebApplicationBuilder builder)
        {
            var jwtSettings = builder.Configuration.GetSection("JWT").Get<JwtSettings>();

            if (string.IsNullOrEmpty(jwtSettings?.Key))
            {
                throw new ArgumentNullException("JWT Key is missing in the configuration.");
            }

            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(option =>
            {
                option.RequireHttpsMetadata = false;
                option.SaveToken = true;
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                };
            });
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