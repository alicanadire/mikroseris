using System.Reflection;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StackExchange.Redis;
using ToyStore.EventBus.Abstractions;
using ToyStore.EventBus.RabbitMQ;
using ToyStore.IdentityService;
using ToyStore.IdentityService.Configuration;
using ToyStore.IdentityService.Data;
using ToyStore.IdentityService.Models;
using ToyStore.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

// Serilog Configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

// Add services to the container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Redis Configuration
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("Redis")
        ?? "localhost:6379";
    return ConnectionMultiplexer.Connect(connectionString);
});
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

// RabbitMQ EventBus Configuration
builder.Services.Configure<RabbitMQSettings>(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("RabbitMQ")
        ?? "amqp://guest:guest@localhost:5672/";
    options.ExchangeName = "toystore_eventbus";
    options.QueueName = "identity_service_queue";
});
builder.Services.AddSingleton<IEventBus, RabbitMQEventBus>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // Password settings
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = false;

        // User settings
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var identityServerBuilder = builder.Services.AddIdentityServer(options =>
    {
        options.Events.RaiseErrorEvents = true;
        options.Events.RaiseInformationEvents = true;
        options.Events.RaiseFailureEvents = true;
        options.Events.RaiseSuccessEvents = true;
        options.EmitStaticAudienceClaim = true;
    })
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
            sql => sql.MigrationsAssembly(migrationsAssembly));
    })
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
            sql => sql.MigrationsAssembly(migrationsAssembly));
    })
    .AddAspNetIdentity<ApplicationUser>();

// Add signing credential
if (builder.Environment.IsDevelopment())
{
    identityServerBuilder.AddDeveloperSigningCredential();
}

// CORS configuration
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

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "ToyStore Identity Service API",
        Version = "v1",
        Description = "Identity and authentication service for the ToyStore platform. Handles user registration, login, and role management.",
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

    // Enable annotations for better documentation
    c.EnableAnnotations();

    // Add operation filters for better examples
    c.ExampleFilters();
});

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger(c =>
    {
        c.RouteTemplate = "swagger/{documentName}/swagger.json";
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToyStore Identity Service API v1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "ToyStore Identity Service API Documentation";
        c.DefaultModelsExpandDepth(-1);
        c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
        c.DisplayRequestDuration();
        c.EnableDeepLinking();
        c.EnableFilter();
        c.EnableValidator();
    });
}

app.UseStaticFiles();

app.UseCors("CorsPolicy");

app.UseRouting();

app.UseIdentityServer();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// Seed data
if (args.Contains("/seed"))
{
    await SeedData.EnsureSeedData(app);
    return;
}

app.Run();
