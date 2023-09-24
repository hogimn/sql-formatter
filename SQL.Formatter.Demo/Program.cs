using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Options;
using System.Net;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 5073);
});

builder.Services.AddMvc();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin();
    });
});

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
});

builder.Host.UseSerilog();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.UseRouting();

app.MapControllerRoute(
    name: "SqlFormatter",
    pattern: "{controller=SqlFormatter}/{action=FormatSql}");

app.UseCors();

app.UseHttpLogging();

app.Run();
