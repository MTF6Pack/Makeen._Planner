using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Net.Sockets;
using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using static Makeen._Planner.ProgramHelper;

namespace Makeen._Planner
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.StartUp();
            ConfigureServices(builder.Services);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers();

            //builder.Services.AddAuthentication().AddGoogle(googleOptions =>
            //{
            //    googleOptions.ClientId = "629210191188-fhqpu3am5i7satd9q1h5gbb333jcbgg7.apps.googleusercontent.com";
            //    googleOptions.ClientSecret = "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";
            //});

            TitleFilter filter = new();

            var app = builder.Build();

            app.UseStaticFiles();
            app.Urls.Add("https://*:" + builder.Configuration["Port"]);

            var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataBaseContext>();
            if (context.Database.GetPendingMigrations().Any()) context.Database.Migrate();

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
            Console.WriteLine("Swagger Url : " + $"https://{Dns.GetHostEntry(Dns.GetHostName()).AddressList
                .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork)}:{builder.Configuration["Port"]}/swagger");
            app.Run();
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGoogle(options =>
            {
                options.ClientId = "629210191188-fhqpu3am5i7satd9q1h5gbb333jcbgg7.apps.googleusercontent.com";
                options.ClientSecret = "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";
            });

            services.AddRazorPages();
        }
    }
}