using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using ToyStore.EventBus.Abstractions;
using ToyStore.EventBus.RabbitMQ;
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
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("Redis")
        ?? "localhost:6379";
    return ConnectionMultiplexer.Connect(connectionString);
});
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

// EventBus Configuration
builder.Services.AddSingleton<IEventBus, RabbitMQEventBus>();

// MediatR Configuration
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(ProductMappingProfile).Assembly));

// AutoMapper Configuration
builder.Services.AddAutoMapper(typeof(ProductMappingProfile));

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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "ToyStore Product Service API",
        Version = "v1",
        Description = "A comprehensive API for managing toy products, categories, and reviews in the ToyStore microservices architecture.",
        Contact = new()
        {
            Name = "ToyStore Development Team",
            Email = "dev@toystore.com",
            Url = new Uri("https://toystore.com")
        },
        License = new()
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Include XML comments for better documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Add JWT Bearer authentication support
    c.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\""
    });

    c.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new()
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Enable annotations for better documentation
    c.EnableAnnotations();

    // Add operation filters for better examples
    c.ExampleFilters();
});

builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "swagger/{documentName}/swagger.json";
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToyStore Product Service API v1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "ToyStore Product Service API Documentation";
        c.DefaultModelsExpandDepth(-1);
        c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
        c.DisplayRequestDuration();
        c.EnableDeepLinking();
        c.EnableFilter();
        c.EnableValidator();
        c.SupportedSubmitMethods(Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Get,
                                Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Post,
                                Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Put,
                                Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Delete);
    });
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
