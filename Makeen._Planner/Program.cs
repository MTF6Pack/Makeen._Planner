using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Net;
using System.Net.Sockets;
using static Makeen._Planner.ProgramHelper;

namespace Makeen._Planner
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options => options.AddPolicy("AllowAllOrigins", builder =>
            {
                builder.SetIsOriginAllowed(_ => true)
                     .AllowAnyMethod()
                     .AllowAnyHeader()
                     .AllowCredentials();
            }));

            builder.StartUp();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            TitleFilter filter = new();

            var app = builder.Build();

            var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataBaseContext>();
            if (context.Database.GetPendingMigrations().Any()) context.Database.Migrate();

            app.Urls.Add("https://*:" + builder.Configuration["Port"]);

            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseSwagger();
            app.UseSwaggerUI(options => options.EnableTryItOutByDefault());

            app.MapControllers();

            app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseCors("AllowAllOrigins");
            app.UseAuthentication();
            app.UseAuthorization();
            Console.WriteLine("Swagger Url : " + $"https://{Dns.GetHostEntry(Dns.GetHostName()).AddressList
                .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork)}:{builder.Configuration["Port"]}/swagger");
            app.Run();
        }
    }
}