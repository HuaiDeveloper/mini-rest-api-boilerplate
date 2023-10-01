using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MiniApi.Applicatoin.Products;
using MiniApi.Persistence.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddOptions<DatabaseSetting>()
    .BindConfiguration(nameof(DatabaseSetting))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddDbContext<ApplicationDbContext>((p, b) =>
{
    DatabaseSetting databaseSetting = p.GetRequiredService<IOptions<DatabaseSetting>>().Value;
    b.UseNpgsql(databaseSetting.ConnectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.AddProductEndpoint();

app.Run();
