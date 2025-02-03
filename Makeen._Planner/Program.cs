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
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers();

            TitleFilter filter = new();

            var app = builder.Build();

            app.UseStaticFiles();
            //app.Urls.Add("https://*:" + builder.Configuration["Port"]);

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
            //Console.WriteLine("Swagger Url : " + $"https://{Dns.GetHostEntry(Dns.GetHostName()).AddressList
            //    .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork)}:{builder.Configuration["Port"]}/swagger");
            app.Run();
        }
    }
}