using Makeen._Planner;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureAppSettings();
builder.ConfigureServices();
ProgramHelper.ConfigureCulture();

var app = builder.Build();
app.ConfigureMiddleware();
app.Run();