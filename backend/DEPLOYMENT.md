# 🚀 ToyStore Microservices - Complete Deployment Guide

This document provides comprehensive deployment instructions for the complete ToyStore microservices backend, implementing all project requirements.

## ✅ Project Requirements Fulfilled

### **Mikroservisler Dersi Proje İsterleri - COMPLETED**

✅ **.NET 7 veya 8 kullanılarak geliştirilen web uygulaması çalışıyor mu?**

- **YES** - Complete .NET 8 microservices architecture with 7 services

✅ **Public site yapılmış mı?**

- **YES** - React frontend integrated via API Gateway (port 5000)

✅ **Sitenin yönetimi admin tarafından yapılabiliyor mu?**

- **YES** - Complete admin dashboard with product/order/user management

✅ **MS Sql Server veri tabanı kullanılmış mı?**

- **YES** - Primary database for Identity, Product, Order, and User services

✅ **Bir No-Sql veri tabanı kullanılmış mı?**

- **YES** - MongoDB for Notification service

✅ **MS Sql Server harici bir ilişkisel veri tabanı kullanılmış mı?**

- **YES** - PostgreSQL for Inventory service

✅ **IdentityServer4 kullanılmış mı?**

- **YES** - Complete IdentityServer4 implementation with JWT tokens

✅ **Redis kullanılmış mı?**

- **YES** - Caching and session management

✅ **CQRS uygulanmış mı?**

- **YES** - Full CQRS implementation in Product Service with MediatR

✅ **RabbitMQ kullanılmış mı?**

- **YES** - Event-driven architecture with message queuing

✅ **En az 1 mikroserviste, Clean Architecture - Onion Architecture mimarisi uygulanmış mı?**

- **YES** - Product Service implements Clean Architecture with Domain/Application/Infrastructure/API layers

✅ **API Gateway kullanılmış mı?**

- **YES** - Ocelot API Gateway with routing, authentication, and caching

✅ **Dockerize bir şekilde tüm projeyi çalıştırabilir misiniz?**

- **YES** - Complete Docker Compose setup with all services

✅ **Seed data da ekle**

- **YES** - Comprehensive seed data for all services

## 🏗️ Complete Architecture

```
Frontend (React)
       ↓
API Gateway (Ocelot) - Port 5000
       ↓
┌─────────────────────────────────────────────────────┐
│                 Microservices                       │
├─────────────────────────────────────────────────────┤
│ Identity Service (IdentityServer4)    - Port 5004  │
│ Product Service (Clean Arch + CQRS)   - Port 5001  │
│ Order Service                          - Port 5002  │
│ User Service                           - Port 5003  │
│ Inventory Service (PostgreSQL)        - Port 5005  │
│ Notification Service (MongoDB)        - Port 5006  │
└─────────────────────────────────────────────────────┘
       ↓
┌─────────────────────────────────────────────────────┐
│                 Infrastructure                      │
├─────────────────────────────────────────────────────┤
│ SQL Server        - Port 1433                      │
│ PostgreSQL        - Port 5432                      │
│ MongoDB           - Port 27017                     │
│ Redis             - Port 6379                      │
│ RabbitMQ          - Port 5672, Management 15672    │
└──────────���──────────────────────────────────────────┘
```

## 🐳 Quick Deployment

### Prerequisites

```bash
# Install Docker and Docker Compose
sudo apt update
sudo apt install docker.io docker-compose

# Or on Windows/Mac - Install Docker Desktop
```

### Instant Deployment

```bash
# Clone repository
git clone [repository-url]
cd backend

# Make deployment script executable
chmod +x scripts/deploy.sh

# Deploy everything with one command
./scripts/deploy.sh

# Or manually with Docker Compose
docker-compose up -d
```

### Verify Deployment

```bash
# Check all services are running
docker-compose ps

# Test API Gateway
curl http://localhost:5000/health

# Test services
curl http://localhost:5001/api/products
curl http://localhost:5002/api/orders
curl http://localhost:5004/.well-known/openid-configuration
```

## 📊 Database Setup & Seed Data

### Automatic Setup

The deployment automatically:

1. Creates all databases
2. Runs migrations
3. Seeds sample data
4. Creates default users

### Manual Seed Data

```bash
# Seed Identity Service with users
docker-compose exec identityservice dotnet run --seed

# Product Service seeds automatically via EF migrations
# Check seed data in ProductDbContext.cs
```

### Default Accounts

```
Admin User:
Email: admin@toystore.com
Password: Admin123!
Role: Admin

Customer User:
Email: customer@toystore.com
Password: Customer123!
Role: Customer
```

## 🔌 Service Endpoints

### API Gateway (Single Entry Point)

```
http://localhost:5000/api/products     → Product Service
http://localhost:5000/api/orders      → Order Service
http://localhost:5000/api/users       → User Service
http://localhost:5000/api/inventory   → Inventory Service
http://localhost:5000/api/notifications → Notification Service
```

### Direct Service Access (Development)

```
Identity Server:     http://localhost:5004
Product Service:     http://localhost:5001
Order Service:       http://localhost:5002
User Service:        http://localhost:5003
Inventory Service:   http://localhost:5005
Notification Service: http://localhost:5006
```

### Management Interfaces

```
RabbitMQ Management: http://localhost:15672
Username: admin
Password: ToyStore123!

Swagger UIs:
http://localhost:5001/swagger (Product Service)
http://localhost:5002/swagger (Order Service)
http://localhost:5003/swagger (User Service)
```

## 🗄️ Database Configurations

### SQL Server Databases

```sql
-- Connection: Server=localhost;User Id=sa;Password=ToyStore123!

ToyStoreIdentity   -- IdentityServer4 & Users
ToyStoreProducts   -- Products, Categories, Reviews
ToyStoreOrders     -- Orders, Cart, OrderItems
ToyStoreUsers      -- User Profiles, Addresses
```

### PostgreSQL Database

```sql
-- Connection: Host=localhost;Database=toystore_inventory;Username=postgres;Password=ToyStore123!

toystore_inventory -- Inventory, Stock Management
```

### MongoDB Database

```javascript
// Connection: mongodb://admin:ToyStore123!@localhost:27017/toystore_notifications?authSource=admin

toystore_notifications; // Notifications, Logs, Messages
```

### Redis

```bash
# Connection: localhost:6379, Password: ToyStore123!
# Used for: Caching, Sessions, Temporary Data
```

## 🔄 Event-Driven Architecture

### RabbitMQ Events

```csharp
ProductCreatedEvent    → Inventory Service, Notification Service
ProductUpdatedEvent    → Inventory Service, Search Service
OrderCreatedEvent      → Inventory Service, Notification Service, User Service
OrderConfirmedEvent    → Notification Service, Inventory Service
StockUpdatedEvent      → Product Service, Notification Service
UserRegisteredEvent    → Notification Service, User Service
```

### Event Flow Example

```
1. User creates order → Order Service
2. OrderCreatedEvent → RabbitMQ
3. Inventory Service → Reduces stock
4. Notification Service → Sends confirmation email
5. User Service → Updates order history
```

## 🏛️ Clean Architecture (Product Service)

### Layer Structure

```
Domain Layer (Core Business Logic)
├── Entities/
│   ├── Product.cs
│   ├── Category.cs
│   └── ProductReview.cs
├── Repositories/
│   └── IProductRepository.cs
└── [No dependencies on external layers]

Application Layer (Business Use Cases)
├── Commands/
│   └── CreateProductCommand.cs
├── Queries/
│   └── GetProductsQuery.cs
├── Handlers/
│   ├── CreateProductHandler.cs
│   └── GetProductsHandler.cs
├── DTOs/
│   └── ProductDto.cs
└── Mappings/
    └── ProductMappingProfile.cs

Infrastructure Layer (External Concerns)
├── Data/
│   └── ProductDbContext.cs
├── Repositories/
│   └── ProductRepository.cs
└── [Implements Domain interfaces]

API Layer (Controllers & Web concerns)
├── Controllers/
│   └── ProductsController.cs
└── Program.cs
```

### CQRS Implementation

```csharp
// Command (Write)
public class CreateProductCommand : IRequest<ApiResponse<ProductDto>>
{
    public ProductCreateDto Product { get; set; }
}

// Query (Read)
public class GetProductsQuery : IRequest<ApiResponse<PaginatedResponse<ProductDto>>>
{
    public int Page { get; set; }
    public string? SearchTerm { get; set; }
}

// Handler
public class CreateProductHandler : IRequestHandler<CreateProductCommand, ApiResponse<ProductDto>>
{
    // Implementation with event publishing
}
```

## 🔐 Authentication & Authorization

### IdentityServer4 Configuration

```csharp
// Clients
- toystore-spa (React Frontend)
- toystore-admin (Admin Dashboard)
- toystore-m2m (Service-to-Service)

// API Scopes
- toystore.products
- toystore.orders
- toystore.users
- toystore.inventory
- toystore.notifications
- toystore.admin

// Identity Resources
- OpenId, Profile, Email, Roles
```

### JWT Token Example

```json
{
  "sub": "user-id",
  "email": "user@toystore.com",
  "name": "John Doe",
  "role": "Customer",
  "scope": ["toystore.products", "toystore.orders"],
  "aud": "toystore.api"
}
```

## 📈 Performance & Scalability

### Caching Strategy

```csharp
// Redis Caching Examples
products_page_1_12        // Product listings
product_{id}              // Individual products
featured_products_8       // Featured products
user_session_{userId}     // User sessions
```

### Database Optimization

```sql
-- Strategic Indexes
CREATE INDEX IX_Products_Category ON Products(CategoryId, IsActive);
CREATE INDEX IX_Products_Search ON Products(Name, Brand, IsActive);
CREATE INDEX IX_Orders_User ON Orders(UserId, CreatedAt);
```

### Horizontal Scaling

```yaml
# Docker Compose Scaling
docker-compose up --scale productservice=3 --scale orderservice=2

# Load Balancer Configuration (Nginx)
upstream productservice {
    server productservice_1:80;
    server productservice_2:80;
    server productservice_3:80;
}
```

## 🧪 Testing & Validation

### Health Checks

```bash
# Overall system health
curl http://localhost:5000/health

# Individual services
curl http://localhost:5001/health  # Product Service
curl http://localhost:5002/health  # Order Service
curl http://localhost:5004/health  # Identity Service
```

### API Testing

```bash
# Get products (public)
curl http://localhost:5000/api/products

# Get categories (public)
curl http://localhost:5000/api/categories

# Create product (admin only)
curl -X POST http://localhost:5000/api/products \
  -H "Authorization: Bearer {admin-jwt-token}" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Test Toy",
    "description": "A test toy",
    "price": 29.99,
    "categoryId": "category-guid"
  }'

# Place order (authenticated)
curl -X POST http://localhost:5000/api/orders \
  -H "Authorization: Bearer {user-jwt-token}" \
  -H "Content-Type: application/json" \
  -d '{
    "paymentMethod": "credit_card",
    "shippingAddress": {...}
  }'
```

### Frontend Integration

```javascript
// React API calls
const products = await fetch("http://localhost:5000/api/products");
const categories = await fetch("http://localhost:5000/api/categories");

// With authentication
const orders = await fetch("http://localhost:5000/api/orders", {
  headers: {
    Authorization: `Bearer ${token}`,
  },
});
```

## 🚨 Production Deployment

### Environment Variables

```env
# Production environment file
ASPNETCORE_ENVIRONMENT=Production
SQLSERVER_CONNECTION=Server=prod-sql;Database=ToyStore;...
IDENTITY_SERVER_URL=https://identity.toystore.com
API_GATEWAY_URL=https://api.toystore.com
RABBITMQ_CONNECTION=amqp://user:pass@prod-rabbit:5672/
REDIS_CONNECTION=prod-redis:6379,password=prodpass
```

### Docker Production

```bash
# Production deployment
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d

# With scaling
docker-compose up -d --scale productservice=3 --scale orderservice=2
```

### Kubernetes Deployment

```yaml
# Example Kubernetes configuration
apiVersion: apps/v1
kind: Deployment
metadata:
  name: toystore-productservice
spec:
  replicas: 3
  selector:
    matchLabels:
      app: productservice
  template:
    spec:
      containers:
        - name: productservice
          image: toystore/productservice:latest
          ports:
            - containerPort: 80
```

## 📊 Monitoring & Logging

### Structured Logging

```csharp
// Serilog configuration in each service
Log.Information("Product {ProductId} created by user {UserId}", productId, userId);
Log.Warning("Low stock for product {ProductId}: {Stock} units", productId, stock);
Log.Error(ex, "Failed to process order {OrderId}", orderId);
```

### Log Files

```
/logs/api-gateway-.log
/logs/identity-service-.log
/logs/product-service-.log
/logs/order-service-.log
```

### Metrics & Health

```bash
# Custom health checks include:
- Database connectivity
- Redis availability
- RabbitMQ connection
- External service dependencies
```

## 🔧 Development

### Local Development

```bash
# Start infrastructure only
docker-compose up -d sqlserver postgresql mongodb redis rabbitmq

# Run services individually in VS Code/Visual Studio
cd src/Services/Product/ToyStore.ProductService.API
dotnet run

cd src/Services/Order/ToyStore.OrderService
dotnet run

# Or run specific service in Docker
docker-compose up identityservice productservice
```

### Database Migrations

```bash
# Product Service
cd src/Services/Product/ToyStore.ProductService.API
dotnet ef migrations add InitialCreate
dotnet ef database update

# Order Service
cd src/Services/Order/ToyStore.OrderService
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## 🤝 Integration Points

### Frontend Integration

```typescript
// Frontend connects to API Gateway only
const API_BASE = "http://localhost:5000/api";

// All requests go through gateway
const products = await fetch(`${API_BASE}/products`);
const orders = await fetch(`${API_BASE}/orders`, {
  headers: { Authorization: `Bearer ${token}` },
});
```

### Service Communication

```csharp
// Services communicate via:
1. HTTP calls through API Gateway
2. RabbitMQ events for async operations
3. Shared Redis cache for performance
```

## 📋 Troubleshooting

### Common Issues

```bash
# Port conflicts
sudo lsof -i :5000  # Check if API Gateway port is busy
docker-compose down && docker-compose up -d

# Database connection issues
docker-compose logs sqlserver
docker-compose restart sqlserver

# RabbitMQ connection issues
docker-compose logs rabbitmq
curl http://localhost:15672  # Check management interface
```

### Service Debugging

```bash
# View logs
docker-compose logs -f [service-name]

# Execute into container
docker-compose exec productservice bash

# Check service health
curl http://localhost:5001/health
```

## 🎯 Complete Feature List

### ✅ Implemented Features

- **Product Management**: CRUD with Clean Architecture
- **Order Processing**: Complete order flow with cart
- **User Management**: Profiles and authentication
- **Inventory Tracking**: PostgreSQL-based stock management
- **Notifications**: MongoDB-based email system
- **Caching**: Redis for performance optimization
- **Message Queuing**: RabbitMQ for async operations
- **API Gateway**: Centralized routing and security
- **Authentication**: IdentityServer4 with JWT
- **Database Support**: SQL Server, PostgreSQL, MongoDB
- **Docker Deployment**: Complete containerization
- **Seed Data**: Sample products, categories, users
- **Health Monitoring**: Built-in health checks
- **Logging**: Structured logging with Serilog

---

## 🎉 Deployment Success

After running `./scripts/deploy.sh` or `docker-compose up -d`, you will have:

1. **Complete microservices architecture** running
2. **All databases** configured and seeded
3. **Frontend integration ready** via API Gateway
4. **Admin functionality** fully operational
5. **All project requirements** satisfied

**Your ToyStore microservices backend is production-ready! 🚀**

For support or questions, check the logs or create an issue in the repository.

**Built with ❤️ for modern microservices architecture**
