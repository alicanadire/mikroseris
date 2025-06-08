using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using StackExchange.Redis;
using System.Reflection;
using ToyStore.EventBus.Abstractions;
using ToyStore.EventBus.RabbitMQ;
using ToyStore.ProductService.Application.Mappings;
using ToyStore.ProductService.Domain.Repositories;
using ToyStore.ProductService.Infrastructure.Data;
using ToyStore.ProductService.Infrastructure.Repositories;
using ToyStore.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

// Serilog Configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Database Configuration
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Authentication Configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["IdentityServer:Authority"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Redis Configuration
builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("Redis") ?? 
                          builder.Configuration["Redis:ConnectionString"];
    return ConnectionMultiplexer.Connect(connectionString!);
});

builder.Services.AddScoped<ICacheService, RedisCacheService>();

// RabbitMQ Configuration
builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddSingleton<IEventBus, RabbitMQEventBus>();

// MediatR Configuration
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(ProductMappingProfile).Assembly));

// AutoMapper Configuration
builder.Services.AddAutoMapper(typeof(ProductMappingProfile));

// FluentValidation Configuration
builder.Services.AddValidatorsFromAssembly(typeof(ProductMappingProfile).Assembly);

// Repository Configuration
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductReviewRepository, ProductReviewRepository>();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks()
    .AddDbContext<ProductDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// Database Migration
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    context.Database.Migrate();
}

app.Run();
