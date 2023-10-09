using System.Text.Json;
using System.Text.Json.Serialization;
using MiniApi.Application;
using MiniApi.Middleware;
using MiniApi.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

builder.Services.AddPersistence();
builder.Services.AddMiddleware();
builder.Services.AddApplication();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseApplication();
app.UseCustomMiddleware();

// Endpoint
app.MapEndpoint();

app.Run();
