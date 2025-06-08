# 🎮 ToyStore - Microservices E-Commerce Platform

A complete, production-ready toy store e-commerce platform built with modern microservices architecture, designed for university microservices course requirements.

![ToyStore Platform](https://img.shields.io/badge/Platform-E--Commerce-blue)
![Architecture](https://img.shields.io/badge/Architecture-Microservices-green)
![Frontend](https://img.shields.io/badge/Frontend-React%20%2B%20TypeScript-blue)
![Backend](https://img.shields.io/badge/Backend-.NET%208-purple)
![Deployment](https://img.shields.io/badge/Deployment-Docker-lightblue)

## 🚀 Quick Start

### Windows (PowerShell - Recommended)

```powershell
.\start-toystore.ps1
```

### Cross-Platform (Docker)

```bash
cd backend
docker-compose up -d
npm install && npm run dev
```

### Access Points

- **🌐 Main Website**: http://localhost:3000
- **⚙️ Admin Panel**: http://localhost:3000/admin
- **🔗 API Gateway**: http://localhost:5000
- **📊 Swagger UI**: http://localhost:5001/swagger

## 📋 University Course Requirements ✅

### Required Technologies

- ✅ **.NET 7/8** Web Application (Using .NET 8)
- ✅ **Public Website** (Complete React SPA)
- ✅ **Admin Management** (Full admin dashboard)
- ✅ **MS SQL Server** (4 separate databases)
- ✅ **NoSQL Database** (MongoDB for notifications)
- ✅ **Additional Relational DB** (PostgreSQL for inventory)
- ✅ **IdentityServer4** (Complete authentication server)
- ✅ **Redis Caching** (Throughout all services)
- ✅ **CQRS Pattern** (Product service with MediatR)
- ✅ **RabbitMQ** (Event-driven messaging)
- ✅ **Clean Architecture** (Product service implementation)
- ✅ **API Gateway** (Ocelot with routing & auth)
- ✅ **Docker Deployment** (Complete containerization)
- ✅ **Seed Data** (Comprehensive test data)

## 🏗️ Architecture Overview

### Microservices

1. **API Gateway** (Ocelot) - Central routing and authentication
2. **Identity Service** - User authentication with IdentityServer4
3. **Product Service** - Product catalog with Clean Architecture + CQRS
4. **Order Service** - Shopping cart and order management
5. **User Service** - User profile management
6. **Inventory Service** - Stock management with PostgreSQL
7. **Notification Service** - Email/SMS with MongoDB

### Databases

- **SQL Server**: Identity, Products, Orders, Users
- **PostgreSQL**: Inventory management
- **MongoDB**: Notifications and logs
- **Redis**: Caching layer

### Frontend

- **React 18** with TypeScript
- **Modern UI** with Tailwind CSS and shadcn/ui
- **Responsive Design** for all devices
- **Real-time Updates** with React Query

## 🎯 Key Features

### Public Website

- 🏠 **Homepage** with hero sections and featured products
- 🛍️ **Product Catalog** with advanced filtering and search
- 🛒 **Shopping Cart** with real-time updates
- 💳 **Checkout Process** with payment integration
- 👤 **User Authentication** with login/register
- 📱 **Responsive Design** for mobile and desktop

### Admin Dashboard

- 📊 **System Overview** with real-time metrics
- 🏷️ **Product Management** (CRUD operations)
- 📦 **Order Management** with status tracking
- 👥 **User Management** with role assignment
- 📈 **Analytics Dashboard** with charts
- 🔧 **Service Health Monitoring**

### Backend Features

- 🔐 **JWT Authentication** with refresh tokens
- 📧 **Event-Driven Architecture** with RabbitMQ
- ⚡ **Redis Caching** for performance
- 🔍 **Advanced Search** with filtering
- 📝 **Comprehensive Logging** with Serilog
- 🏥 **Health Checks** for all services

## 🛠️ Technology Stack

### Frontend

- **React 18** - Modern UI library
- **TypeScript** - Type safety
- **Vite** - Fast build tool
- **Tailwind CSS** - Utility-first CSS
- **shadcn/ui** - Component library
- **React Router** - Client-side routing
- **React Query** - Data fetching and caching

### Backend

- **.NET 8** - Modern backend framework
- **Entity Framework Core** - ORM
- **MediatR** - CQRS implementation
- **AutoMapper** - Object mapping
- **Serilog** - Structured logging
- **FluentValidation** - Input validation

### Infrastructure

- **Docker & Docker Compose** - Containerization
- **Ocelot** - API Gateway
- **IdentityServer4** - Authentication server
- **RabbitMQ** - Message broker
- **Redis** - In-memory cache
- **Nginx** - Reverse proxy

## 📁 Project Structure

```
ToyStore/
├── 🎨 Frontend (React + TypeScript)
│   ├── src/components/          # Reusable UI components
│   ├── src/pages/              # Page components
│   ├── src/lib/                # API client and utilities
│   └── src/types/              # TypeScript definitions
│
├── ⚙️ Backend (.NET 8 Microservices)
│   ├── src/ApiGateway/         # Ocelot API Gateway
│   ├── src/Services/           # Microservices
│   │   ├── Identity/           # IdentityServer4
│   │   ├── Product/            # Clean Architecture + CQRS
│   │   ├── Order/              # Order management
│   │   ├── User/               # User profiles
│   │   ├── Inventory/          # Stock management
│   │   └── Notification/       # Email/SMS service
│   └── src/Shared/             # Common libraries
│
├── 🐳 Infrastructure
│   ├── docker-compose.yml      # Main services
│   ├── docker-compose-full.yml # Extended setup
│   └── scripts/                # Database initialization
│
└── 📚 Documentation
    ├── README.md               # This file
    ├── DEPLOYMENT.md           # Deployment guide
    └── INTEGRATION.md          # Integration instructions
```

## 🚀 Deployment Options

### 1. Quick Start (Windows)

```powershell
# Start everything with one command
.\start-toystore.ps1

# Include frontend in Docker
.\start-toystore.ps1 -IncludeFrontend

# Show real-time logs
.\start-toystore.ps1 -ShowLogs
```

### 2. Manual Docker Setup

```bash
# Start backend services
cd backend
docker-compose -f docker-compose-full.yml up -d

# Start frontend separately
npm install
npm run dev
```

### 3. Development Mode

```bash
# Backend with hot reload
cd backend
dotnet run --project src/ApiGateway/ToyStore.ApiGateway

# Frontend with hot reload
npm run dev
```

## 🔧 Configuration

### Environment Variables

Copy `.env.example` to `.env` and configure:

```env
# API Configuration
VITE_API_GATEWAY_URL=http://localhost:5000/api
VITE_IDENTITY_SERVER_URL=http://localhost:5004

# Feature Flags
VITE_ENABLE_NOTIFICATIONS=true
VITE_ENABLE_ANALYTICS=true
```

### Database Connection Strings

All configured in `appsettings.json` files with Docker networking.

## 📊 Monitoring & Health Checks

### Health Check Endpoints

- **API Gateway**: http://localhost:5000/health
- **All Services**: Available through gateway routing

### Management UIs

- **RabbitMQ Management**: http://localhost:15672
- **Database Admin**: http://localhost:8080
- **Redis Admin**: http://localhost:8081

## 🧪 Testing

### Backend Tests

```bash
cd backend
dotnet test
```

### Frontend Tests

```bash
npm test
```

## 📈 Performance Features

- **Redis Caching** - API responses and session data
- **Database Indexing** - Optimized queries
- **Lazy Loading** - Frontend components
- **Connection Pooling** - Database connections
- **CDN Ready** - Static asset optimization

## 🔐 Security Features

- **JWT Authentication** with refresh tokens
- **Role-Based Authorization** (Admin, Customer)
- **CORS Configuration** for secure cross-origin requests
- **Input Validation** with FluentValidation
- **SQL Injection Protection** with parameterized queries
- **XSS Protection** with proper encoding

## 🚧 Development Setup

### Prerequisites

- .NET 8 SDK
- Node.js 18+
- Docker Desktop
- SQL Server Management Studio (optional)

### Local Development

1. Clone the repository
2. Copy `.env.example` to `.env`
3. Run `.\start-toystore.ps1`
4. Access http://localhost:3000

## 📖 API Documentation

### Swagger UI

- **Gateway Swagger**: http://localhost:5000/swagger
- **Product Service**: http://localhost:5001/swagger
- **Order Service**: http://localhost:5002/swagger

### Key Endpoints

```
GET    /api/products              # Get products with filtering
POST   /api/products              # Create product (admin)
GET    /api/products/{id}         # Get product details
POST   /api/cart/add              # Add to cart
GET    /api/orders                # Get user orders
POST   /api/auth/login            # User login
```

## 🎓 Educational Value

This project demonstrates:

- **Microservices Architecture** patterns
- **Domain-Driven Design** principles
- **CQRS and Event Sourcing** concepts
- **Clean Architecture** implementation
- **API Gateway** patterns
- **Event-Driven Architecture** with messaging
- **Containerization** best practices
- **Modern Frontend** development

## 📞 Support

For questions or issues:

1. Check the [Deployment Guide](backend/DEPLOYMENT.md)
2. Review [Integration Instructions](INTEGRATION.md)
3. Check Docker logs: `docker-compose logs -f`

## 📄 License

This project is for educational purposes as part of a university microservices course.

---

**🎮 Happy coding! Built with ❤️ for modern e-commerce.**
