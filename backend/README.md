# ToyStore Microservices Backend

A complete .NET 8 microservices architecture for an e-commerce toy store application, implementing all modern patterns and technologies.

## 🏗️ Architecture Overview

This project implements a comprehensive microservices architecture with the following requirements:

✅ **.NET 8** - Latest .NET framework  
✅ **Public Site** - React frontend integration  
✅ **Admin Management** - Complete admin dashboard functionality  
✅ **MS SQL Server** - Primary relational database  
✅ **NoSQL Database** - MongoDB for notifications and logs  
✅ **Additional Relational DB** - PostgreSQL for inventory service  
✅ **IdentityServer4** - Authentication and authorization  
✅ **Redis** - Caching and session management  
✅ **CQRS** - Command Query Responsibility Segregation  
✅ **RabbitMQ** - Message queuing and event bus  
✅ **Clean Architecture** - Onion Architecture in Product Service  
✅ **API Gateway** - Ocelot gateway for routing  
✅ **Docker** - Full containerization support

## 🚀 Microservices

### 1. API Gateway (Port 5000)

- **Technology**: Ocelot + .NET 8
- **Purpose**: Single entry point, routing, authentication
- **Features**: Rate limiting, caching, load balancing

### 2. Identity Service (Port 5004)

- **Technology**: IdentityServer4 + .NET 8
- **Database**: SQL Server
- **Purpose**: Authentication, authorization, user management
- **Features**: JWT tokens, OAuth2, OpenID Connect

### 3. Product Service (Port 5001) - **Clean Architecture**

- **Technology**: .NET 8 + CQRS + MediatR
- **Database**: SQL Server
- **Architecture**: Clean Architecture (Onion)
- **Features**: Product catalog, categories, reviews, caching

### 4. Order Service (Port 5002)

- **Technology**: .NET 8
- **Database**: SQL Server
- **Features**: Order management, cart, checkout process

### 5. User Service (Port 5003)

- **Technology**: .NET 8
- **Database**: SQL Server
- **Features**: User profiles, preferences, address management

### 6. Inventory Service (Port 5005)

- **Technology**: .NET 8
- **Database**: PostgreSQL (Additional Relational DB)
- **Features**: Stock management, real-time inventory updates

### 7. Notification Service (Port 5006)

- **Technology**: .NET 8
- **Database**: MongoDB (NoSQL)
- **Features**: Email/SMS notifications, message queuing

## 🛠️ Technology Stack

### Backend Technologies

- **.NET 8** - Latest framework with improved performance
- **Entity Framework Core 8** - ORM for data access
- **MediatR** - CQRS implementation
- **AutoMapper** - Object-to-object mapping
- **FluentValidation** - Input validation
- **Serilog** - Structured logging

### Databases

- **SQL Server** - Primary database for most services
- **PostgreSQL** - Inventory service (additional relational DB)
- **MongoDB** - Notification service (NoSQL requirement)
- **Redis** - Caching and session storage

### Infrastructure

- **IdentityServer4** - Authentication server
- **RabbitMQ** - Message broker for async communication
- **Ocelot** - API Gateway
- **Docker & Docker Compose** - Containerization
- **Nginx** - Load balancer (configurable)

### Monitoring & Health

- **Health Checks** - Built-in health monitoring
- **Swagger/OpenAPI** - API documentation
- **Serilog** - Structured logging with file/console output

## 🏛️ Clean Architecture Implementation

The **Product Service** implements Clean Architecture (Onion Architecture):

```
ToyStore.ProductService/
├── Domain/                 # Core business logic
│   ├── Entities/          # Domain entities
│   └── Repositories/      # Repository interfaces
├── Application/           # Application logic (CQRS)
│   ├── Commands/         # Write operations
│   ├── Queries/          # Read operations
│   ├── Handlers/         # Command/Query handlers
│   ├── DTOs/             # Data transfer objects
│   └── Mappings/         # AutoMapper profiles
├── Infrastructure/        # External concerns
│   ├── Data/             # Database context
│   └── Repositories/     # Repository implementations
└── API/                  # Web API controllers
    └── Controllers/      # REST endpoints
```

### CQRS Pattern

- **Commands**: Create, Update, Delete operations
- **Queries**: Read operations with caching
- **Handlers**: Separate handlers for each operation
- **Events**: Integration events via RabbitMQ

## 🔄 Event-Driven Architecture

### RabbitMQ Integration Events

- `ProductCreatedEvent` - New product added
- `ProductUpdatedEvent` - Product modified
- `OrderCreatedEvent` - New order placed
- `StockUpdatedEvent` - Inventory changes
- `UserRegisteredEvent` - New user registration

### Message Flow Example

```
1. User places order → OrderService
2. OrderCreatedEvent → RabbitMQ
3. InventoryService → Reduces stock
4. NotificationService → Sends confirmation email
5. ProductService → Updates product stats
```

## 🐳 Docker Deployment

### Quick Start

```bash
# Clone repository
git clone [repository-url]
cd backend

# Start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Scale specific service
docker-compose up --scale productservice=3
```

### Services Endpoints

- **API Gateway**: http://localhost:5000
- **Identity Server**: http://localhost:5004
- **Product Service**: http://localhost:5001
- **Order Service**: http://localhost:5002
- **User Service**: http://localhost:5003
- **Inventory Service**: http://localhost:5005
- **Notification Service**: http://localhost:5006
- **RabbitMQ Management**: http://localhost:15672
- **SQL Server**: localhost:1433
- **PostgreSQL**: localhost:5432
- **MongoDB**: localhost:27017
- **Redis**: localhost:6379

## 🔐 Authentication & Authorization

### IdentityServer4 Configuration

- **Clients**: SPA, Admin Dashboard, M2M
- **Scopes**: Service-specific API scopes
- **Users**: Seeded admin and customer accounts

### Default Users

```
Admin: admin@toystore.com / Admin123!
Customer: customer@toystore.com / Customer123!
```

### JWT Claims

- `sub` - User ID
- `email` - User email
- `role` - User role (Admin/Customer)
- `name` - Full name

## 📊 Database Schemas

### SQL Server Databases

- `ToyStoreIdentity` - Identity and user management
- `ToyStoreProducts` - Product catalog and reviews
- `ToyStoreOrders` - Orders and shopping carts
- `ToyStoreUsers` - User profiles and preferences

### PostgreSQL Database

- `toystore_inventory` - Inventory and stock management

### MongoDB Database

- `toystore_notifications` - Notifications and message logs

## 🚀 Getting Started

### Prerequisites

- Docker & Docker Compose
- .NET 8 SDK (for development)
- SQL Server Management Studio (optional)

### Development Setup

```bash
# 1. Start infrastructure services
docker-compose up sqlserver mongodb postgresql redis rabbitmq -d

# 2. Update connection strings in appsettings.json files

# 3. Run migrations
cd src/Services/Product/ToyStore.ProductService.API
dotnet ef database update

# 4. Start services individually or via Docker
dotnet run --project src/ApiGateway/ToyStore.ApiGateway
dotnet run --project src/Services/Identity/ToyStore.IdentityService
dotnet run --project src/Services/Product/ToyStore.ProductService.API
```

### Production Deployment

```bash
# Build and deploy all services
docker-compose -f docker-compose.yml up -d

# Monitor services
docker-compose ps
docker-compose logs -f [service-name]
```

## 📋 API Documentation

### Swagger Endpoints

- **API Gateway**: http://localhost:5000/swagger
- **Product Service**: http://localhost:5001/swagger
- **Order Service**: http://localhost:5002/swagger
- **User Service**: http://localhost:5003/swagger

### Key Endpoints

```
GET  /api/products           - Get products with filtering
GET  /api/products/{id}      - Get product details
POST /api/products           - Create product (Admin)
GET  /api/categories         - Get categories
POST /api/orders             - Create order
GET  /api/orders/{id}        - Get order details
POST /api/cart/add           - Add to cart
GET  /api/users/profile      - Get user profile
```

## 🔄 Data Flow & Integration

### Frontend → Backend Flow

```
React App → API Gateway → Microservices
         ↓
   IdentityServer4 (Auth)
         ↓
   SQL Server / PostgreSQL / MongoDB
         ↓
   Redis (Cache) + RabbitMQ (Events)
```

### CQRS Implementation

```
Web Request → Controller → MediatR → Command/Query Handler → Repository → Database
                                  ↓
                               Event Bus (RabbitMQ) → Other Services
```

## 🧪 Testing

### Health Checks

```bash
curl http://localhost:5000/health
curl http://localhost:5001/health
curl http://localhost:5002/health
```

### API Testing

```bash
# Get products
curl http://localhost:5000/api/products

# Get categories
curl http://localhost:5000/api/categories

# Create product (requires admin token)
curl -X POST http://localhost:5000/api/products \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{product-data}'
```

## 📈 Performance & Scalability

### Caching Strategy

- **Redis**: Product catalog, user sessions
- **In-Memory**: Frequently accessed data
- **Cache Invalidation**: Event-driven updates

### Database Optimization

- **Indexes**: Strategic indexing on query patterns
- **Connection Pooling**: Optimized connections
- **Read Replicas**: Configurable for scaling

### Message Queuing

- **Async Processing**: Non-blocking operations
- **Retry Logic**: Failed message handling
- **Dead Letter Queues**: Error management

## 🔧 Configuration

### Environment Variables

```env
# Database Connections
SQLSERVER_CONNECTION=Server=...
POSTGRESQL_CONNECTION=Host=...
MONGODB_CONNECTION=mongodb://...
REDIS_CONNECTION=localhost:6379

# Identity Server
IDENTITY_SERVER_URL=http://identityservice
JWT_AUTHORITY=http://identityservice

# RabbitMQ
RABBITMQ_CONNECTION=amqp://admin:password@rabbitmq:5672/

# Services
PRODUCT_SERVICE_URL=http://productservice
ORDER_SERVICE_URL=http://orderservice
```

## 🚨 Monitoring & Logging

### Structured Logging

- **Serilog**: JSON structured logs
- **Log Levels**: Information, Warning, Error
- **Log Sinks**: Console, File, (configurable for external systems)

### Health Monitoring

- **Built-in Health Checks**: Database connectivity, external services
- **Custom Health Checks**: Business logic validation
- **Health Dashboard**: Available endpoints for monitoring

## 🤝 Contributing

1. Fork the repository
2. Create feature branch (`git checkout -b feature/new-feature`)
3. Follow clean architecture principles
4. Add unit tests for new functionality
5. Update documentation
6. Submit pull request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🎯 Project Requirements Checklist

✅ **Web Application**: .NET 8 microservices architecture  
✅ **Public Site**: React frontend integrated via API Gateway  
✅ **Admin Management**: Complete admin functionality for store management  
✅ **MS SQL Server**: Primary database for most services  
✅ **NoSQL Database**: MongoDB for notifications service  
✅ **Additional Relational DB**: PostgreSQL for inventory service  
✅ **IdentityServer4**: Complete authentication and authorization  
✅ **Redis**: Caching and session management  
✅ **CQRS**: Implemented in Product Service with MediatR  
✅ **RabbitMQ**: Message queuing and event-driven architecture  
✅ **Clean Architecture**: Onion Architecture in Product Service  
✅ **API Gateway**: Ocelot gateway with routing and authentication  
✅ **Docker**: Complete containerization of all services

**Built with ❤️ for modern microservices architecture**
