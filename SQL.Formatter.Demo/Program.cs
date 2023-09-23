using Microsoft.Extensions.Options;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.UseRouting();

app.MapControllerRoute(
    name: "SqlFormatter",
    pattern: "{controller=SqlFormatter}/{action=FormatSql}");

app.UseCors();

app.Run();
