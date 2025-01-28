using Makeen._Planner.Task_Service;
using Makeen._Planner.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;
using System.Text.Json.Serialization;
using Domain;
using Persistence;
using Persistence.Repository.Base;
using Persistence.Repository;
using Persistence.Repository.Interface;
using Domain.Task;
using Application.Group_Service;
using Application.DataSeeder.OTP;
using Application.UserAndOtp.Services;
using Scrutor;
using System;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Makeen._Planner
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.ConfigureJWT();

            //builder.Services.AddAuthentication().AddGoogle(options =>
            //{
            //    IConfigurationSection googleAuthNSection =
            //   config.GetSection("Authentication:Google");
            //    options.ClientId = googleAuthNSection["ClientId"];
            //    options.ClientSecret = googleAuthNSection["ClientSecret"];
            //});

            // AddUser services to the container.

            TitleFilter yosef = new();
            builder.Services.AddSwaggerGen(o => o.DocumentFilter<TitleFilter>());
            //builder.Services.AddControllers();
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.DefaultIgnoreCondition =
                    JsonIgnoreCondition.WhenWritingNull;
            });

            builder.Services.AddDbContext<DataBaseContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddIdentity<Domain.User, UserRole>()
            .AddEntityFrameworkStores<DataBaseContext>()
            .AddDefaultTokenProviders();

            builder.Services.Scan(scan => scan
                .FromAssemblyOf<IUserRepository>() // Scans the assembly containing IUserRepository
                .AddClasses() // Finds all classes that implement IUserRepository
                .AsImplementedInterfaces() // Registers them as their implemented interfaces
                .WithScopedLifetime()); // You can also use WithScopedLifetime() or WithSingletonLifetime()


            builder.Services.Scan(scan => scan
                .FromAssemblyOf<IUserService>() // Scans the assembly containing IUserRepository
                .AddClasses() // Finds all classes that implement IUserRepository
                .AsImplementedInterfaces() // Registers them as their implemented interfaces
                .WithScopedLifetime()); // You can also use WithScopedLifetime() or WithSingletonLifetime()


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataBaseContext>();
            if (context.Database.GetPendingMigrations().Any()) context.Database.Migrate();

            app.UseStaticFiles();

            //Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.MapControllers();

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.Run();
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
        public class TitleFilter : IDocumentFilter
        {
            public void Apply(OpenApiDocument doc, DocumentFilterContext context)
            {
                doc.Info.Title = "ّFor those who seek Success ...";
                doc.Info.Version = "Phase 1";
            }
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            // Register services using Scrutor
            services.Scan(scan => scan
                .FromAssemblyOf<IUserRepository>() // Scans the assembly containing IUserRepository
                .AddClasses(classes => classes.AssignableTo<IConvention>()) // Finds all classes that implement IUserRepository
                .AsImplementedInterfaces() // Registers them as their implemented interfaces
                .WithScopedLifetime()); // You can also use WithScopedLifetime() or WithSingletonLifetime()

            // Other service registrations
            services.AddControllers();
        }
    }
}



//builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//builder.Services.AddScoped<IEmailOTPService, BaseEmailOTP>();
//builder.Services.AddScoped<IOTPService, OTPService,>();
//builder.Services.AddScoped<IUserService, UserService>();
//builder.Services.AddScoped<ITaskService, TaskService>();
//builder.Services.AddScoped<IGroupService, GroupService>();

//builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<ITaskRepository, TaskRepository>();
//builder.Services.AddScoped<IGroupRepository, GroupRepository>();

//builder.Services.AddScoped<IRepository<Domain.UserAndOtp>, Repository<Domain.UserAndOtp>>();
//builder.Services.AddScoped<IRepository<Domain.Task.Task>, Repository<Domain.Task.Task>>();
//builder.Services.AddScoped<IRepository<Group>, Repository<Group>>();