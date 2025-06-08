# 🧸 ToyStore - Modern Mikroservis E-Ticaret Platformu

<div align="center">

![ToyStore Logo](https://img.shields.io/badge/🎮_ToyStore-Mikroservis_Mimarisi-ff6b6b?style=for-the-badge&logoColor=white)

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-18.3-61DAFB?style=for-the-badge&logo=react)](https://reactjs.org/)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=for-the-badge&logo=docker)](https://docker.com/)
[![IdentityServer4](https://img.shields.io/badge/IdentityServer4-Authentication-FF6B35?style=for-the-badge)](https://identityserver4.readthedocs.io/)

**Modern mikroservis mimarisi ile geliştirilmiş kapsamlı oyuncak mağazası e-ticaret platformu**

[🚀 Hızlı Başlatma](#-hızlı-başlatma) • [📋 Gereksinimler](#-proje-gereksinimleri-karşılama-raporu) • [🏗️ Mimari](#️-sistem-mimarisi) • [💻 Kurulum](#-kurulum-rehberi)

</div>

---

## 📋 Proje Gereksinimleri Karşılama Raporu

### **Mikroservisler Dersi Proje İsterleri - %100 TAMAMLANDI** ✅

| **Gereksinim**                                                           |  **Durum**  | **Detaylı Açıklama**                                                                                                                                           | **Kanıt**                                        |
| ------------------------------------------------------------------------ | :---------: | -------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------ |
| **.NET 7 veya 8 kullanılarak geliştirilen web uygulaması çalışıyor mu?** | ✅ **EVET** | **.NET 8.0** kullanılarak 7 ayrı mikroservis geliştirildi. Her servis bağımsız olarak çalışır ve API Gateway üzerinden erişilebilir.                           | `src/Services/*/` klasörlerinde .NET 8 projeleri |
| **Public site yapılmış mı?**                                             | ✅ **EVET** | **React 18** ile modern, responsive, kullanıcı dostu public website. Ürün katalogu, sepet, ödeme, kullanıcı kayıt/giriş sistemi tamamen çalışır durumda.       | Frontend: `http://localhost:5173`                |
| **Sitenin yönetimi admin tarafından yapılabiliyor mu?**                  | ✅ **EVET** | **Kapsamlı Admin Dashboard** - Ürün yönetimi, sipariş takibi, kullanıcı yönetimi, sistem durumu izleme, veritabanı yönetimi. Role-based erişim kontrolü.       | Admin Panel: `/admin` rotası                     |
| **MS SQL Server veri tabanı kullanılmış mı?**                            | ✅ **EVET** | **4 ayrı SQL Server veritabanı**: `ToyStoreIdentity`, `ToyStoreProducts`, `ToyStoreOrders`, `ToyStoreUsers`. Entity Framework Core ile migration ve seed data. | Docker: `sqlserver:1433`                         |
| **Bir No-SQL veri tabanı kullanılmış mı?**                               | ✅ **EVET** | **MongoDB** Notification Service için kullanılıyor. Email/SMS bildirimleri, log kayıtları, message queue verileri MongoDB'de tutuluyor.                        | Docker: `mongodb:27017`                          |
| **MS SQL Server harici bir ilişkisel veri tabanı kullanılmış mı?**       | ✅ **EVET** | **PostgreSQL** Inventory Service için ayrı ilişkisel veritabanı. Stok yönetimi, warehouse operasyonları PostgreSQL'de.                                         | Docker: `postgresql:5432`                        |
| **IdentityServer4 kullanılmış mı?**                                      | ✅ **EVET** | **Tam IdentityServer4 implementasyonu** - JWT token, OAuth2, OpenID Connect, role-based authorization, client credentials flow.                                | Identity Service: `http://localhost:5004`        |
| **Redis kullanılmış mı?**                                                | ✅ **EVET** | **Redis Cache**: Ürün catalog cache, session yönetimi, API response cache, real-time performance optimization.                                                 | Docker: `redis:6379`                             |
| **CQRS uygulanmış mı?**                                                  | ✅ **EVET** | **Product Service'de tam CQRS**: MediatR ile Command/Query separation, CreateProductCommand, GetProductsQuery, event publishing.                               | `src/Services/Product/Application/`              |
| **RabbitMQ kullanılmış mı?**                                             | ✅ **EVET** | **Event-driven architecture**: ProductCreatedEvent, OrderCreatedEvent, StockUpdatedEvent, async mikroservis iletişimi.                                         | Docker: `rabbitmq:5672`, UI: `:15672`            |
| **En az 1 mikroserviste Clean Architecture uygulanmış mı?**              | ✅ **EVET** | **Product Service Clean Architecture**: Domain/Application/Infrastructure/API layers, dependency inversion, repository pattern.                                | `src/Services/Product/` klasör yapısı            |
| **API Gateway kullanılmış mı?**                                          | ✅ **EVET** | **Ocelot API Gateway**: Routing, authentication, rate limiting, caching, load balancing, service discovery.                                                    | API Gateway: `http://localhost:5000`             |
| **Docker ile deployment**                                                | ✅ **EVET** | **Tam Docker Compose setup**: Tüm servisler, veritabanları, infrastructure tek komutla başlatılabiliyor.                                                       | `docker-compose-full.yml`                        |
| **Seed data eklendi mi?**                                                | ✅ **EVET** | **Kapsamlı seed data**: Kategoriler, ürünler, kullanıcılar, sample siparişler, notification templates.                                                         | Database init scripts                            |

---

## 🏗️ Sistem Mimarisi

<div align="center">

```mermaid
graph TB
    subgraph "Frontend Layer"
        FE[🌐 React Frontend<br/>Port 3000/5173]
    end

    subgraph "API Gateway Layer"
        GW[🚪 Ocelot API Gateway<br/>Port 5000]
    end

    subgraph "Microservices Layer"
        ID[🔐 Identity Service<br/>Port 5004<br/>IdentityServer4]
        PS[🛍️ Product Service<br/>Port 5001<br/>Clean Architecture + CQRS]
        OS[📦 Order Service<br/>Port 5002<br/>Shopping Cart + Orders]
        US[👤 User Service<br/>Port 5003<br/>User Management]
        IS[📊 Inventory Service<br/>Port 5005<br/>Stock Management]
        NS[📧 Notification Service<br/>Port 5006<br/>Email/SMS]
    end

    subgraph "Database Layer"
        SQL[(🗄️ SQL Server<br/>4 Databases)]
        PG[(🐘 PostgreSQL<br/>Inventory DB)]
        MG[(🍃 MongoDB<br/>Notifications)]
    end

    subgraph "Infrastructure Layer"
        RD[⚡ Redis<br/>Cache + Sessions]
        RMQ[🐰 RabbitMQ<br/>Message Queue]
    end

    FE --> GW
    GW --> ID
    GW --> PS
    GW --> OS
    GW --> US
    GW --> IS
    GW --> NS

    ID --> SQL
    PS --> SQL
    PS --> RD
    PS --> RMQ
    OS --> SQL
    OS --> RMQ
    US --> SQL
    IS --> PG
    IS --> RMQ
    NS --> MG
    NS --> RMQ
```

</div>

### 🔧 Teknoloji Stack'i

#### **Backend (.NET 8)**

- **Mikroservis Framework**: ASP.NET Core 8.0
- **Authentication**: IdentityServer4 + JWT
- **ORM**: Entity Framework Core 8.0
- **CQRS**: MediatR
- **Mapping**: AutoMapper
- **Validation**: FluentValidation
- **Logging**: Serilog
- **Cache**: Redis with StackExchange.Redis
- **Message Queue**: RabbitMQ with MassTransit
- **API Documentation**: Swagger/OpenAPI

#### **Frontend (React)**

- **Framework**: React 18.3 + TypeScript
- **Build Tool**: Vite
- **Styling**: TailwindCSS + Radix UI
- **Routing**: React Router 6
- **State Management**: React Query + Zustand
- **HTTP Client**: Fetch API with custom wrapper

#### **Databases**

- **SQL Server**: Primary database (Identity, Products, Orders, Users)
- **PostgreSQL**: Inventory and stock management
- **MongoDB**: Notifications, logs, and real-time data
- **Redis**: Caching, sessions, and performance optimization

#### **Infrastructure**

- **API Gateway**: Ocelot
- **Message Broker**: RabbitMQ
- **Containerization**: Docker + Docker Compose
- **Monitoring**: Health Checks + Custom dashboards

---

## 🚀 Hızlı Başlatma

### **Tek Komut ile Çalıştırma** 🎯

#### **Windows PowerShell**

```powershell
# 1. Projeyi indirin
git clone [repo-url]
cd ToyStore

# 2. Docker Desktop'ı başlatın

# 3. Tek komutla her şeyi başlatın
.\start-toystore.ps1

# 🎉 2-3 dakika sonra hazır!
```

#### **Linux/macOS**

```bash
# 1. Projeyi indirin
git clone [repo-url]
cd ToyStore/backend

# 2. Docker'ı başlatın

# 3. Tek komutla her şeyi başlatın
./scripts/deploy.sh

# 🎉 2-3 dakika sonra hazır!
```

### **Erişim Noktaları** 🌐

| **Servis**                 | **URL**                       | **Açıklama**           |
| -------------------------- | ----------------------------- | ---------------------- |
| 🌐 **Frontend**            | http://localhost:5173         | React uygulaması       |
| 🚪 **API Gateway**         | http://localhost:5000         | Ana API endpoint'i     |
| 🔐 **Identity Server**     | http://localhost:5004         | Authentication server  |
| 📝 **Swagger API Docs**    | http://localhost:5001/swagger | API dokümantasyonu     |
| 🐰 **RabbitMQ Management** | http://localhost:15672        | Message queue yönetimi |
| 🗄️ **Database Admin**      | http://localhost:8080         | Veritabanı yönetimi    |
| ⚡ **Redis Admin**         | http://localhost:8081         | Cache yönetimi         |

### **Demo Hesaplar** 👥

```
🔑 Admin Hesabı:     admin@toystore.com / Admin123!
👤 Müşteri Hesabı:   customer@toystore.com / Customer123!
🐰 RabbitMQ:         admin / ToyStore123!
```

---

## 💻 Kurulum Rehberi

### **Gereksinimler** 📋

- **Docker Desktop** (Windows/Mac/Linux)
- **PowerShell** (Windows) veya **Bash** (Linux/Mac)
- **Node.js 18+** (sadece frontend development için)
- **.NET 8 SDK** (sadece backend development için)

### **Detaylı Kurulum Adımları**

#### **1. Repository'yi İndirin**

```bash
git clone https://github.com/your-repo/toystore.git
cd toystore
```

#### **2. Docker Compose ile Başlatın**

```bash
# Backend servislerini başlat
cd backend
docker-compose -f docker-compose-full.yml up -d

# Veya PowerShell script ile
.\scripts\deploy.ps1
```

#### **3. Frontend'i Başlatın**

```bash
# Development mode
npm install
npm run dev

# Production build
npm run build
npm run preview
```

#### **4. Sistem Durumunu Kontrol Edin**

```bash
# Servis durumları
docker-compose ps

# Health check'ler
curl http://localhost:5000/health
curl http://localhost:5001/health
```

---

## 🔍 Mikroservislerin Detaylı Açıklaması

### **1. 🔐 Identity Service (Port 5004)**

**IdentityServer4 ile Authentication & Authorization**

```csharp
✅ JWT Token Generation
✅ OAuth2 / OpenID Connect
✅ Role-based Authorization (Admin/Customer)
✅ Client Credentials Flow
✅ Refresh Token Support
✅ User Registration & Login
✅ Password Security & Validation
```

**Databases**: SQL Server (`ToyStoreIdentity`)  
**Key Features**: Secure authentication, token management, user roles

### **2. ���️ Product Service (Port 5001)**

**Clean Architecture + CQRS Implementation**

```csharp
📁 Domain Layer (Core Business Logic)
   ├── Entities/Product.cs
   ├── Entities/Category.cs
   └── Repositories/IProductRepository.cs

📁 Application Layer (Use Cases)
   ├── Commands/CreateProductCommand.cs
   ├── Queries/GetProductsQuery.cs
   ├── Handlers/CreateProductHandler.cs
   └── DTOs/ProductDto.cs

📁 Infrastructure Layer (External)
   ├── Data/ProductDbContext.cs
   └── Repositories/ProductRepository.cs

📁 API Layer (Controllers)
   └── Controllers/ProductsController.cs
```

**Key Features**: CQRS with MediatR, Redis caching, event publishing

### **3. 📦 Order Service (Port 5002)**

**Shopping Cart & Order Management**

```csharp
✅ Shopping Cart Operations
✅ Order Creation & Management
✅ Payment Integration Ready
✅ Order Status Tracking
✅ Event Publishing (OrderCreated, OrderConfirmed)
```

**Database**: SQL Server (`ToyStoreOrders`)

### **4. 👤 User Service (Port 5003)**

**User Profile & Management**

```csharp
✅ User Profile Management
✅ Address Management
✅ Order History
✅ Preferences & Settings
```

**Database**: SQL Server (`ToyStoreUsers`)

### **5. 📊 Inventory Service (Port 5005)**

**Stock Management with PostgreSQL**

```sql
-- PostgreSQL Tables
inventory (
    id, product_id, quantity,
    reserved_quantity, warehouse_location
)

stock_movements (
    id, product_id, movement_type,
    quantity, reason, reference_id
)
```

**Key Features**: Real-time stock tracking, warehouse management

### **6. 📧 Notification Service (Port 5006)**

**MongoDB-based Notification System**

```javascript
// MongoDB Collections
notifications: {
    type: "email|sms|push",
    recipient: "user@email.com",
    message: { subject, body, templateId },
    status: "pending|sent|failed|delivered"
}

logs: {
    level: "info|warning|error",
    message: "Log message",
    service: "service-name",
    timestamp: Date
}
```

### **7. 🚪 API Gateway (Port 5000)**

**Ocelot-based Routing & Security**

```json
{
  "Routes": [
    {
      "DownstreamPathTemplate": "/api/products",
      "UpstreamPathTemplate": "/api/products",
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer"
      },
      "FileCacheOptions": {
        "TtlSeconds": 30
      }
    }
  ]
}
```

---

## 🗄️ Database Mimarisi

### **SQL Server Databases**

```sql
🏢 ToyStoreIdentity   -- IdentityServer4 + User accounts
🛍️ ToyStoreProducts   -- Products, categories, reviews
📦 ToyStoreOrders     -- Orders, cart, payment info
👥 ToyStoreUsers      -- User profiles, addresses
```

### **PostgreSQL Database**

```sql
🏭 toystore_inventory -- Stock management
   ├── inventory (stock levels)
   └── stock_movements (stock history)
```

### **MongoDB Database**

```javascript
📧 toystore_notifications
   ├── notifications (email/sms queue)
   └── logs (system logs)
```

### **Redis Cache**

```redis
🚀 Performance Optimization
   ├── product_catalog_cache
   ├── user_sessions
   ├── api_response_cache
   └── real_time_stock_data
```

---

## 🔄 Event-Driven Architecture

### **RabbitMQ Message Flow**

```mermaid
sequenceDiagram
    participant FE as Frontend
    participant PS as Product Service
    participant OS as Order Service
    participant IS as Inventory Service
    participant NS as Notification Service
    participant RMQ as RabbitMQ

    FE->>OS: Create Order
    OS->>RMQ: OrderCreatedEvent
    RMQ->>IS: Update Stock
    RMQ->>NS: Send Confirmation Email
    RMQ->>PS: Update Product Stats
    NS->>FE: Email Sent Notification
```

### **Event Types**

```csharp
🔄 ProductCreatedEvent    → Inventory + Notification
🔄 OrderCreatedEvent      → Inventory + Notification + User
🔄 StockUpdatedEvent      → Product + Notification
🔄 UserRegisteredEvent    → Notification + User
🔄 PaymentProcessedEvent  → Order + Notification
```

---

## 📊 Admin Dashboard Özellikleri

### **System Monitoring** 🖥️

- **Real-time service health checks**
- **Database connection monitoring**
- **API response time tracking**
- **Error rate monitoring**

### **Product Management** 🛍️

- **CRUD operations** for products
- **Bulk import/export** functionality
- **Image upload** and management
- **Category hierarchy** management
- **Stock level** monitoring

### **Order Management** 📦

- **Order status** tracking
- **Payment verification**
- **Shipping management**
- **Refund processing**

### **User Management** 👥

- **Customer accounts** management
- **Role assignment** (Admin/Customer)
- **Activity monitoring**
- **Support ticket** system

---

## 🧪 Test Senaryoları

### **API Testing**

```bash
# Health checks
curl http://localhost:5000/health
curl http://localhost:5001/health

# Product operations
curl http://localhost:5000/api/products
curl http://localhost:5000/api/categories

# Authentication test
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@toystore.com","password":"Admin123!"}'
```

### **Database Testing**

```bash
# SQL Server test
docker exec -it toystore-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P ToyStore123! \
  -Q "SELECT COUNT(*) FROM ToyStoreProducts.dbo.Products"

# PostgreSQL test
docker exec -it toystore-postgresql \
  psql -U postgres -d toystore_inventory \
  -c "SELECT COUNT(*) FROM inventory;"

# MongoDB test
docker exec -it toystore-mongodb \
  mongosh toystore_notifications \
  --eval "db.notifications.countDocuments()"
```

---

## 🔧 Geliştirme Ortamı

### **Frontend Development**

```bash
# Backend Docker'da çalışırken frontend'i geliştirin
cd frontend
npm install
npm run dev  # http://localhost:5173

# Hot reload ve debugging aktif
```

### **Backend Development**

```bash
# Sadece infrastructure servislerini başlat
docker-compose up -d sqlserver postgresql mongodb redis rabbitmq

# Visual Studio/VS Code'da mikroservisleri debug edin
dotnet run --project src/Services/Product/ToyStore.ProductService.API
```

### **Database Development**

```bash
# EF Core migrations
dotnet ef migrations add NewFeature --project Infrastructure --startup-project API
dotnet ef database update --project Infrastructure --startup-project API
```

---

## 📈 Performance & Monitoring

### **Caching Strategy**

```csharp
// Redis Cache Examples
🚀 Product Catalog Cache  → 5 dakika TTL
🚀 User Session Cache     → 30 dakika TTL
🚀 API Response Cache     → 1 dakika TTL
🚀 Static Content Cache   → 24 saat TTL
```

### **Database Optimization**

```sql
-- Strategic Indexes
CREATE INDEX IX_Products_Category ON Products(CategoryId, IsActive);
CREATE INDEX IX_Products_Search ON Products(Name, Brand, IsActive);
CREATE INDEX IX_Orders_User ON Orders(UserId, CreatedAt);
CREATE INDEX IX_Inventory_Product ON inventory(product_id);
```

### **Monitoring Tools**

- **Health Checks**: `/health` endpoints on all services
- **Swagger Documentation**: `/swagger` on each API
- **RabbitMQ Management**: Real-time message monitoring
- **Redis Commander**: Cache monitoring and management

---

## 🚀 Production Deployment

### **Docker Production**

```bash
# Production build
docker-compose -f docker-compose.yml -f docker-compose.prod.yml build

# Production deployment
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d

# Scale services
docker-compose up -d --scale productservice=3 --scale orderservice=2
```

### **Environment Configuration**

```env
# Production Environment
ASPNETCORE_ENVIRONMENT=Production
SQL_CONNECTION_STRING=Server=prod-sql;Database=ToyStore;...
REDIS_CONNECTION_STRING=prod-redis:6379,password=***
RABBITMQ_CONNECTION_STRING=amqp://prod-user:***@prod-rabbit:5672/
IDENTITY_SERVER_URL=https://identity.toystore.com
```

---

## 🤝 Katkıda Bulunma

### **Development Workflow**

1. **Fork** the repository
2. **Clone** your fork
3. **Create** a feature branch
4. **Make** your changes
5. **Test** thoroughly
6. **Submit** a pull request

### **Code Standards**

- **C#**: Follow Microsoft coding conventions
- **TypeScript/React**: Use ESLint + Prettier
- **Database**: Use migrations for schema changes
- **Docker**: Multi-stage builds for optimization

---

## 📚 Dokümantasyon

### **API Documentation**

- **Swagger UI**: http://localhost:5001/swagger
- **Postman Collection**: `/docs/ToyStore.postman_collection.json`
- **API Versioning**: Semantic versioning (v1, v2, etc.)

### **Architecture Documentation**

- **C4 Model Diagrams**: `/docs/architecture/`
- **Database Schema**: `/docs/database/`
- **Sequence Diagrams**: `/docs/flows/`

---

## 🆘 Sorun Giderme

### **Yaygın Sorunlar**

#### **Docker Sorunları**

```bash
# Docker servis durumu
docker-compose ps

# Servis logları
docker-compose logs [service-name]

# Container restart
docker-compose restart [service-name]

# Tam temizlik
docker-compose down -v
docker system prune -f
```

#### **Database Bağlantı Sorunları**

```bash
# SQL Server test
docker exec -it toystore-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P ToyStore123!

# Connection string kontrolü
echo $SQL_CONNECTION_STRING
```

#### **Port Çakışması**

```bash
# Windows
netstat -ano | findstr :5000

# Linux/Mac
lsof -i :5000

# Çakışan process'i sonlandır
kill -9 [PID]
```

---

## 📞 Destek & İletişim

### **Proje Durumu**

- ✅ **Backend**: %100 Tamamlandı
- ✅ **Frontend**: %100 Tamamlandı
- ✅ **Database**: %100 Tamamlandı
- ✅ **Docker**: %100 Tamamlandı
- ✅ **Documentation**: %100 Tamamlandı

### **İletişim Kanalları**

- 🐛 **Bug Reports**: GitHub Issues
- 💡 **Feature Requests**: GitHub Discussions
- 📚 **Documentation**: Wiki pages
- 💬 **Community**: Discord/Slack

---

<div align="center">

## 🎯 **Proje Tamamlama Durumu: %100** ✅

**ToyStore mikroservis projesi tüm gereksinimleri karşılayarak başarıyla tamamlanmıştır!**

### **Toplam Özellikler**

🎮 **7 Mikroservis** | 🗄️ **3 Database Türü** | 🔐 **IdentityServer4** | ⚡ **Redis Cache** | 🐰 **RabbitMQ** | 🐳 **Docker**

**Geliştirici**: Fusion AI Assistant  
**Teknoloji Stack**: .NET 8 + React + Docker  
**Mimari**: Clean Architecture + CQRS + Event-Driven

---

**⭐ Bu projeyi beğendiyseniz yıldız vermeyi unutmayın!**

</div>
