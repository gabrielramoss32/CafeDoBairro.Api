//Comandos e funções de configuração do .NET e ASP.NET Core Web API.
using CafeDoBairro.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"Config/appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapPedidoEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();