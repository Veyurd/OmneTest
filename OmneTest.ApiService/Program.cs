using OmneTest.ApiService.Extensions;
using Repository.Implementations;
using Repository.Interfaces;
using FastEndpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddFastEndpoints();
// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.AddNpgsqlDataSource(connectionName: "OmneDB");
builder.Services.AddScoped<IProductRepository, ProductRepository>();


var app = builder.Build();

//This Ensures the Products Table will be Created if it does not already exist
app.EnsureDatabaseTable();

app.UseDefaultExceptionHandler()
   .UseFastEndpoints();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapDefaultEndpoints();

app.Run();

//Quick hack to make the Test WebAppFactory target the Api Project without rewriting program.cs to use an actual main method
public partial class Program { }