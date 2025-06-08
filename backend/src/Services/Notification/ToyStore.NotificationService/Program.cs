using MongoDB.Driver;
using ToyStore.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure MongoDB
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDBConnection") 
        ?? "mongodb://admin:ToyStore123!@localhost:27017";
    return new MongoClient(connectionString);
});

builder.Services.AddScoped(sp =>
{
    var client = sp.GetService<IMongoClient>();
    return client?.GetDatabase("toystore_notifications");
});

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
    c.SwaggerDoc("v1", new() { Title = "ToyStore Notification Service", Version = "v1" });
});

// Add health checks
builder.Services.AddHealthChecks()
    .AddMongoDb(builder.Configuration.GetConnectionString("MongoDBConnection") 
        ?? "mongodb://admin:ToyStore123!@localhost:27017");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToyStore Notification Service v1"));
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

// Initialize MongoDB collections
using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetService<IMongoDatabase>();
    if (database != null)
    {
        // Ensure collections exist
        var notificationsCollection = database.GetCollection<BsonDocument>("notifications");
        var templatesCollection = database.GetCollection<BsonDocument>("templates");
        
        // Create indexes
        var notificationIndexKeys = Builders<BsonDocument>.IndexKeys
            .Ascending("recipient")
            .Ascending("createdAt");
        notificationsCollection.Indexes.CreateOne(new CreateIndexModel<BsonDocument>(notificationIndexKeys));
    }
}

app.Run();
