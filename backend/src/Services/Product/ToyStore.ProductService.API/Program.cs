using Microsoft.EntityFrameworkCore;
using ToyStore.ProductService.Application.Mappings;
using ToyStore.ProductService.Domain.Repositories;
using ToyStore.ProductService.Infrastructure.Data;
using ToyStore.ProductService.Infrastructure.Repositories;
using ToyStore.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

// Database Configuration
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Redis Configuration
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

// MediatR Configuration
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(ProductMappingProfile).Assembly));

// AutoMapper Configuration
builder.Services.AddAutoMapper(typeof(ProductMappingProfile));

// Repository Configuration
builder.Services.AddScoped<IProductRepository, ProductRepository>();

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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ToyStore Product Service", Version = "v1" });
});

builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToyStore Product Service v1"));
}

app.UseCors("CorsPolicy");
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

// Database Migration
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
