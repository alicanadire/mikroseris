# ToyStore - Microservices E-commerce Platform

A modern, production-ready toy store application built with React and designed to work with a .NET microservices backend architecture.

## 🚀 Features

### Frontend Features

- **Modern React Application** - Built with React 18, TypeScript, and Vite
- **Responsive Design** - Mobile-first approach using TailwindCSS
- **Complete E-commerce Flow** - Product browsing, cart management, checkout process
- **Admin Dashboard** - Product management and order tracking
- **Authentication Ready** - Integration points for IdentityServer4
- **Real-time Features** - Prepared for WebSocket integration

### E-commerce Functionality

- ✅ Product catalog with categories and filtering
- ✅ Shopping cart with persistent storage
- ✅ Checkout process with multiple payment options
- ✅ User authentication and registration
- ✅ Admin panel for product management
- ✅ Order confirmation and tracking
- ✅ Product reviews and ratings
- ✅ Wishlist functionality
- ✅ Responsive mobile design

### Microservices Integration Points

- **Product Service** - Product catalog and inventory management
- **User Service** - Customer and admin user management
- **Order Service** - Order processing and fulfillment
- **Payment Service** - Payment processing integration
- **Notification Service** - Email and SMS notifications
- **Inventory Service** - Stock management
- **API Gateway** - Centralized API routing

## 🏗️ Architecture

### Frontend Architecture

```
src/
├── components/          # Reusable UI components
│   ├── ui/             # Base UI library (Radix + TailwindCSS)
│   ├── Header.tsx      # Navigation and cart
│   ├── Footer.tsx      # Site footer
│   └── ProductCard.tsx # Product display component
├── pages/              # Application pages
│   ├── Index.tsx       # Homepage
│   ├── Products.tsx    # Product listing
│   ├── ProductDetail.tsx # Product details
│   ├── Cart.tsx        # Shopping cart
│   ├── Checkout.tsx    # Checkout process
│   ├── Login.tsx       # Authentication
���   └── Admin.tsx       # Admin dashboard
├── lib/                # Utilities and integrations
│   ├── api.ts          # API client for microservices
│   ├── auth.ts         # Authentication helpers
│   └── utils.ts        # Common utilities
├── types/              # TypeScript type definitions
└── hooks/              # Custom React hooks
```

### Backend Microservices (Ready for Integration)

#### 1. **Product Service** (.NET 8 Web API)

- Product catalog management
- Category and inventory tracking
- Search and filtering capabilities
- **Database**: SQL Server
- **Cache**: Redis for product search

#### 2. **User Service** (.NET 8 Web API)

- User registration and profile management
- Admin user management
- **Database**: SQL Server
- **Identity**: IdentityServer4 integration

#### 3. **Order Service** (.NET 8 Web API)

- Order processing and management
- Order history and tracking
- **Database**: SQL Server
- **Messaging**: RabbitMQ for order events

#### 4. **Payment Service** (.NET 8 Web API)

- Payment processing
- Multiple payment provider support
- **Database**: SQL Server (for transaction logs)
- **External APIs**: Stripe, PayPal integration

#### 5. **Notification Service** (.NET 8 Web API)

- Email and SMS notifications
- Order confirmations and updates
- **Database**: MongoDB (for notification logs)
- **Messaging**: RabbitMQ for async notifications

#### 6. **Inventory Service** (.NET 8 Web API)

- Stock level management
- Real-time inventory updates
- **Database**: SQL Server
- **Cache**: Redis for fast stock checks

#### 7. **API Gateway** (Ocelot/.NET 8)

- Request routing and aggregation
- Authentication and authorization
- Rate limiting and caching
- **Load Balancing**: Nginx

## 🛠️ Technology Stack

### Frontend

- **React 18** with TypeScript
- **Vite** for development and building
- **TailwindCSS** for styling
- **Radix UI** for accessible components
- **React Router** for client-side routing
- **React Query** for data fetching
- **Zustand** for state management (ready)

### Backend (Microservices)

- **.NET 8** Web APIs
- **IdentityServer4** for authentication
- **Entity Framework Core** for data access
- **MediatR** for CQRS implementation
- **AutoMapper** for object mapping
- **FluentValidation** for input validation

### Databases

- **SQL Server** - Primary relational database
- **MongoDB** - NoSQL for notifications and logs
- **Redis** - Caching and session storage

### Infrastructure

- **Docker** & **Docker Compose** for containerization
- **RabbitMQ** for message queuing
- **Nginx** for reverse proxy and load balancing
- **Azure** or **AWS** ready for cloud deployment

### Monitoring & Logging

- **Serilog** for structured logging
- **Health Checks** for service monitoring
- **Swagger/OpenAPI** for API documentation

## 🚀 Getting Started

### Prerequisites

- Node.js 18+ and npm
- Docker and Docker Compose
- .NET 8 SDK (for backend development)

### Frontend Setup

```bash
# Clone the repository
git clone [repository-url]
cd toystore-frontend

# Install dependencies
npm install

# Start development server
npm run dev

# Build for production
npm run build
```

### Environment Variables

Create a `.env` file with the following:

```env
# API Endpoints
VITE_API_GATEWAY_URL=http://localhost:5000/api
VITE_PRODUCT_SERVICE_URL=http://localhost:5001/api
VITE_USER_SERVICE_URL=http://localhost:5002/api
VITE_ORDER_SERVICE_URL=http://localhost:5003/api

# IdentityServer4
VITE_IDENTITY_SERVER_URL=http://localhost:5004
VITE_CLIENT_ID=toy-store-spa

# Features
VITE_ENABLE_ANALYTICS=true
VITE_ENABLE_CHAT=true
```

### Docker Deployment

```bash
# Full microservices stack
docker-compose up -d

# Frontend only
docker-compose up frontend

# Scale services
docker-compose up --scale product-service=3
```

## 📊 Microservices Architecture Features

### CQRS Implementation

- **Command Query Responsibility Segregation** in Order Service
- Separate read and write models for optimal performance
- Event sourcing for audit trails

### Message Queue Integration

- **RabbitMQ** for asynchronous communication
- Order events trigger inventory updates
- Email notifications sent via message queues

### Caching Strategy

- **Redis** for product catalog caching
- Session management and cart persistence
- Real-time inventory checks

### Authentication & Authorization

- **IdentityServer4** for centralized authentication
- JWT tokens for service-to-service communication
- Role-based access control (Admin/Customer)

### Data Management

- **SQL Server** for transactional data
- **MongoDB** for document storage (notifications, logs)
- **Redis** for caching and temporary data

## 🎯 API Integration Examples

### Product Service Integration

```typescript
// Get products with filtering
const products = await ApiClient.getProducts({
  category: "educational-toys",
  minPrice: 10,
  maxPrice: 100,
  inStock: true,
});

// Get product details
const product = await ApiClient.getProduct("product-id");
```

### Cart Management

```typescript
// Add item to cart
await ApiClient.addToCart("product-id", 2);

// Update cart item
await ApiClient.updateCartItem("item-id", 3);

// Get cart contents
const cart = await ApiClient.getCart();
```

### Authentication Flow

```typescript
// Login with IdentityServer4
const user = await ApiClient.login("user@example.com", "password");

// OAuth/OpenID Connect flow
const authUrl = getAuthCodeFlowUrl();
window.location.href = authUrl;
```

## 🔧 Development Features

### Code Quality

- **TypeScript** for type safety
- **ESLint** and **Prettier** for code formatting
- **Husky** for pre-commit hooks
- **Jest** and **React Testing Library** for testing

### Development Tools

- **Vite** for fast development builds
- **Hot Module Replacement** for instant updates
- **Dev server proxy** for API development
- **Source maps** for debugging

### Production Optimizations

- **Code splitting** for smaller bundles
- **Tree shaking** for unused code elimination
- **Asset optimization** and compression
- **CDN ready** for static asset delivery

## 📈 Performance & Scalability

### Frontend Performance

- Lazy loading for route-based code splitting
- Image optimization and lazy loading
- Virtual scrolling for large product lists
- Service worker for offline functionality

### Backend Scalability

- Horizontal scaling with Docker containers
- Database read replicas for improved performance
- Redis clustering for cache scaling
- Load balancing with Nginx

### Monitoring

- Health check endpoints for all services
- Structured logging with correlation IDs
- Performance metrics collection
- Error tracking and alerting

## 🚀 Deployment

### Docker Compose (Development)

```bash
docker-compose -f docker-compose.dev.yml up
```

### Production Deployment

```bash
# Build production images
docker-compose -f docker-compose.prod.yml build

# Deploy to production
docker-compose -f docker-compose.prod.yml up -d
```

### Cloud Deployment (Azure/AWS)

- Container registry integration
- Kubernetes manifests included
- Auto-scaling configuration
- Production-ready security settings

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Support

For support and questions:

- Email: support@toystore.com
- Documentation: [docs.toystore.com](https://docs.toystore.com)
- Issues: [GitHub Issues](https://github.com/toystore/issues)

---

Built with ❤️ for modern e-commerce and microservices architecture.
