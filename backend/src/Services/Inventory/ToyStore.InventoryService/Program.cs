using Microsoft.EntityFrameworkCore;
using ToyStore.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure PostgreSQL
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection")));

// Add cache service
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ToyStore Inventory Service", Version = "v1" });
});

// Add health checks
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("PostgreSQLConnection")!);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToyStore Inventory Service v1"));
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
    context.Database.EnsureCreated();
}

app.Run();

// DbContext for Inventory
public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }

    public DbSet<InventoryItem> InventoryItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<InventoryItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Location).HasMaxLength(100);
            entity.HasIndex(e => e.ProductId);
        });

        // Seed data
        modelBuilder.Entity<InventoryItem>().HasData(
            new InventoryItem { Id = 1, ProductId = "1", Quantity = 50, Location = "Warehouse A", LastUpdated = DateTime.UtcNow },
            new InventoryItem { Id = 2, ProductId = "2", Quantity = 30, Location = "Warehouse A", LastUpdated = DateTime.UtcNow },
            new InventoryItem { Id = 3, ProductId = "3", Quantity = 25, Location = "Warehouse B", LastUpdated = DateTime.UtcNow },
            new InventoryItem { Id = 4, ProductId = "4", Quantity = 40, Location = "Warehouse B", LastUpdated = DateTime.UtcNow }
        );
    }
}

public class InventoryItem
{
    public int Id { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Location { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
}
