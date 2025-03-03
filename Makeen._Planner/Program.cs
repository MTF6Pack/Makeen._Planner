using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Diagnostics;

namespace Makeen._Planner
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // Add CORS policy before services
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

            builder.StartUp();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = PathString.Empty;
                options.AccessDeniedPath = PathString.Empty;
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = 403;
                    return Task.CompletedTask;
                };
            });

            var app = builder.Build();

            // Run migrations automatically if pending
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DataBaseContext>();
                if (context.Database.GetPendingMigrations().Any())
                {
                    context.Database.Migrate();
                }
            }

            app.Urls.Add("https://*:" + builder.Configuration["Port"]);

            app.UseMiddleware<GlobalExceptionMiddleware>();

            // Open Swagger automatically
            Process.Start("cmd", $"/c start https://192.168.164.222:6969/swagger");

            app.UseSwagger();
            app.UseSwaggerUI(options => options.EnableTryItOutByDefault());

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseCors("AllowAllOrigins");
            app.UseRouting();
            app.UseAuthentication();
            app.Use(async (context, next) =>
            {
                Console.WriteLine($"🛠 Middleware Check: {context.Request.Method} {context.Request.Path}");
                if (context.Request.Headers.ContainsKey("Authorization"))
                {
                    Console.WriteLine($"🔍 Token Found: {context.Request.Headers.Authorization}");
                }
                else
                {
                    Console.WriteLine("🚨 No Authorization Header Found!");
                }
                await next();
            });
            app.UseAuthorization();
            app.MapControllers();



            builder.Consoleshits();
            app.Run();
        }
    }
}