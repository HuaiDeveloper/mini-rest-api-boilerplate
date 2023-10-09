using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MiniApi.Application.Auth;
using MiniApi.Application.Products;
using MiniApi.Persistence.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using MiniApi.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = ".MiniApi.Cookies";
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.HttpOnly = true;
        options.EventsType = typeof(CustomCookieAuthenticationEvents);
    });

builder.Services.AddAuthorization(options =>
{
    string[] authRoles = AuthRole.GetAuthRoles();
    foreach (var role in authRoles)
    {
        options.AddPolicy(role, (policy) => policy.RequireRole(role));
    }
});

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

// Service
builder.Services
    .AddScoped<CustomCookieAuthenticationEvents>()
    .AddScoped<AuthService>()
    .AddScoped<StaffService>()
    .AddScoped<ProductService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Endpoint
app.MapProductEndpoint()
    .MapAuthEndpoint();

app.Run();
