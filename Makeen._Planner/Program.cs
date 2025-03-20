using Application.Notification_Service.BackGroundService;
using Infrastructure;
using Makeen._Planner;
using Microsoft.OpenApi.Models;
using Persistence;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.ConfigureCors();
builder.StartUp();
builder.ConfigureSwagger();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    if (!c.SwaggerGeneratorOptions.SecuritySchemes.ContainsKey("Bearer"))
    {
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Enter 'Bearer {your_token_here}'",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
    }

    c.OrderActionsBy(apiDesc => apiDesc.GroupName);
});

// JWT handling configuration
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

builder.Services.AddHostedService<TaskNotificationService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();
// DateTime Middleware (should be early in the request pipeline)
app.UseMiddleware<GlobalDateTimeMiddleware>();

// Database Migration (before running the app)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DataBaseContext>();
    if (dbContext.Database.GetPendingMigrations().Any())
    {
        dbContext.Database.Migrate();
    }
}

// Set application URLs
app.Urls.Add($"https://*:{builder.Configuration["Port"]}");


// Open Swagger in Browser
var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
var ipAddress = hostEntry.AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork)?.ToString() ?? "localhost";
Process.Start("cmd", $"/c start https://{ipAddress}:{builder.Configuration["Port"]}/swagger");

// Ensure HTTPS redirection is enforced early
app.UseHttpsRedirection();

// Static files should be before authentication
app.UseStaticFiles();

app.UseRouting();

// CORS should be before authentication
app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.EnableTryItOutByDefault();
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    options.OAuthClientId("your-client-id"); // Optional if using OAuth.
});

app.MapControllers();

// Log startup banner last
builder.LogStartupBanner();

app.Run();