using Application.DataSeeder;
using Domain;
using Google.Protobuf.WellKnownTypes;
using Makeen._Planner.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Persistence;
using Persistence.Repository.Interface;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;
using System.Text.Json.Serialization;

namespace Makeen._Planner
{
    public static class ProgramHelper
    {
        public static void StartUp(this WebApplicationBuilder builder)
        {
            Tools(builder);
            ConfigureJWT(builder);
            ConvertEnumToString(builder);

        }
        public static void Tools(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<DataBaseContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddSwaggerGen(o => o.DocumentFilter<TitleFilter>());


            builder.Services.Scan(scan => scan
                .FromAssemblyOf<IUserRepository>() // Scans the assembly containing IUserRepository
                .AddClasses() // Finds all classes that implement IUserRepository
                .AsImplementedInterfaces() // Registers them as their implemented interfaces
                .WithScopedLifetime()); // You can also use WithScopedLifetime() or WithSingletonLifetime()


            builder.Services.Scan(scan => scan
                .FromAssemblyOf<IUserService>()
                .AddClasses()
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            builder.Services.AddIdentity<User, UserRole>()
            .AddEntityFrameworkStores<DataBaseContext>()
            .AddDefaultTokenProviders();
        
            builder.Services.AddScoped<JwtToken>();



        }
        public static void ConfigureJWT(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1234567890qwerty1234567890qwerty"))
                };
            });
        }
        public static void ConvertEnumToString(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.DefaultIgnoreCondition =
                    JsonIgnoreCondition.WhenWritingNull;
            });
        }
    }
    public class TitleFilter() : IDocumentFilter
    {
        public void Apply(OpenApiDocument doc, DocumentFilterContext context)
        {
            doc.Info.Title = "ّFor those who seek Success ...";
            doc.Info.Version = "Phase 1";
        }
    }
}
