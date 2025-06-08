# 🔗 ToyStore Frontend-Backend Integration Guide

This guide explains how the React frontend integrates with the .NET microservices backend.

## 🏗️ Integration Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   React SPA     │───▶│  API Gateway    │───▶│  Microservices  │
│  (Port 3000)    │    │  (Port 5000)    │    │  (Various)      │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## 🔧 Configuration

### Environment Variables

The frontend uses these environment variables (in `.env`):

```env
# Backend API endpoints
VITE_API_GATEWAY_URL=http://localhost:5000/api
VITE_IDENTITY_SERVER_URL=http://localhost:5004

# Feature flags
VITE_ENABLE_NOTIFICATIONS=true
VITE_ENABLE_ANALYTICS=true

# Development settings
VITE_DEBUG_MODE=false
```

### API Client Configuration

The main API client is in `src/lib/api.ts`:

```typescript
const API_CONFIG = {
  GATEWAY: import.meta.env.VITE_API_GATEWAY_URL || "http://localhost:5000/api",
  IDENTITY_SERVER:
    import.meta.env.VITE_IDENTITY_SERVER_URL || "http://localhost:5004",
  TIMEOUT: 30000,
  RETRY_ATTEMPTS: 3,
};
```

## 🔐 Authentication Flow

### 1. Login Process

```typescript
// Frontend sends credentials to Identity Service
const response = await ApiClient.login(email, password);

// Identity Service returns JWT token
localStorage.setItem("access_token", response.access_token);
localStorage.setItem("refresh_token", response.refresh_token);

// Subsequent requests include Authorization header
headers["Authorization"] = `Bearer ${token}`;
```

### 2. Token Management

```typescript
// Auto token refresh when expired
if (response.status === 401) {
  const refreshed = await this.refreshToken();
  if (refreshed) {
    // Retry original request with new token
    return this.request(config);
  }
}
```

## 📡 API Integration Points

### Product Service Integration

```typescript
// Get products with filtering
static async getProducts(filters?: ProductFilters): Promise<PaginatedResponse<Product>> {
  const params = new URLSearchParams();
  if (filters?.category) params.append('category', filters.category);
  if (filters?.search) params.append('search', filters.search);

  return this.get(`/products?${params}`);
}

// Get single product with reviews
static async getProduct(id: string): Promise<Product> {
  return this.get(`/products/${id}`);
}
```

### Order Service Integration

```typescript
// Add item to cart
static async addToCart(productId: string, quantity: number): Promise<Cart> {
  return this.post('/cart/add', { productId, quantity });
}

// Get user's cart
static async getCart(): Promise<Cart> {
  return this.get('/cart');
}

// Place order
static async createOrder(orderData: CreateOrderRequest): Promise<Order> {
  return this.post('/orders', orderData);
}
```

### User Service Integration

```typescript
// Get user profile
static async getUserProfile(): Promise<User> {
  return this.get('/users/profile');
}

// Update profile
static async updateProfile(data: UpdateProfileRequest): Promise<User> {
  return this.put('/users/profile', data);
}
```

## 🎨 Frontend Components Integration

### Header Component

```typescript
// Real-time cart count
const { data: cart } = useQuery({
  queryKey: ['cart'],
  queryFn: () => ApiClient.getCart(),
  enabled: !!user,
});

// Display cart item count
<Badge>{cart?.totalItems || 0}</Badge>
```

### Product Catalog

```typescript
// Products with pagination and filtering
const { data: products, isLoading } = useQuery({
  queryKey: ["products", filters, page],
  queryFn: () => ApiClient.getProducts({ ...filters, page }),
});

// Real-time search
const [searchTerm, setSearchTerm] = useState("");
const debouncedSearch = useDebounce(searchTerm, 300);
```

### Shopping Cart

```typescript
// Add to cart with optimistic updates
const addToCartMutation = useMutation({
  mutationFn: ({ productId, quantity }) =>
    ApiClient.addToCart(productId, quantity),
  onSuccess: () => {
    queryClient.invalidateQueries(["cart"]);
    toast.success("Added to cart!");
  },
});
```

## 🔄 Real-time Updates

### Using React Query for Cache Management

```typescript
// Auto-refresh cart data
useQuery({
  queryKey: ["cart"],
  queryFn: () => ApiClient.getCart(),
  refetchInterval: 30000, // Refresh every 30 seconds
  staleTime: 5000,
});

// Optimistic updates for better UX
const updateQuantityMutation = useMutation({
  mutationFn: ({ itemId, quantity }) =>
    ApiClient.updateCartItem(itemId, quantity),
  onMutate: async ({ itemId, quantity }) => {
    // Optimistically update the UI
    const previousCart = queryClient.getQueryData(["cart"]);
    queryClient.setQueryData(["cart"], (old) => ({
      ...old,
      items: old.items.map((item) =>
        item.id === itemId ? { ...item, quantity } : item,
      ),
    }));
    return { previousCart };
  },
});
```

## 📊 Backend Status Monitoring

### Health Check Integration

```typescript
// Monitor microservices health
const useBackendStatus = () => {
  return useQuery({
    queryKey: ["backend-status"],
    queryFn: async () => {
      const services = [
        { name: "API Gateway", url: "/health" },
        { name: "Product Service", url: "/products/health" },
        { name: "Order Service", url: "/orders/health" },
        { name: "User Service", url: "/users/health" },
      ];

      const results = await Promise.allSettled(
        services.map((service) =>
          fetch(`${API_CONFIG.GATEWAY}${service.url}`).then((res) => ({
            ...service,
            status: res.ok ? "healthy" : "unhealthy",
            responseTime: Date.now() - startTime,
          })),
        ),
      );

      return results.map((result) =>
        result.status === "fulfilled" ? result.value : null,
      );
    },
    refetchInterval: 10000, // Check every 10 seconds
  });
};
```

## 🛡️ Error Handling

### Global Error Boundary

```typescript
// Automatic retry with exponential backoff
const apiRequest = async (config, retryCount = 0) => {
  try {
    return await fetch(config);
  } catch (error) {
    if (retryCount < API_CONFIG.RETRY_ATTEMPTS) {
      const delay = Math.pow(2, retryCount) * 1000;
      await new Promise((resolve) => setTimeout(resolve, delay));
      return apiRequest(config, retryCount + 1);
    }
    throw error;
  }
};

// User-friendly error messages
const getErrorMessage = (error) => {
  if (error.status === 404) return "Item not found";
  if (error.status === 401) return "Please log in again";
  if (error.status >= 500) return "Server error, please try again";
  return error.message || "Something went wrong";
};
```

## 🚀 Performance Optimizations

### Caching Strategy

```typescript
// Aggressive caching for product data
useQuery({
  queryKey: ["product", productId],
  queryFn: () => ApiClient.getProduct(productId),
  staleTime: 5 * 60 * 1000, // 5 minutes
  cacheTime: 30 * 60 * 1000, // 30 minutes
});

// Background updates for fresh data
useQuery({
  queryKey: ["products"],
  queryFn: () => ApiClient.getProducts(),
  refetchOnWindowFocus: false,
  refetchOnMount: false,
  staleTime: 2 * 60 * 1000,
});
```

### Lazy Loading

```typescript
// Code splitting for better performance
const AdminPanel = lazy(() => import('./pages/Admin'));
const ProductDetail = lazy(() => import('./pages/ProductDetail'));

// Image lazy loading
<img
  src={product.imageUrl}
  loading="lazy"
  alt={product.name}
/>
```

## 🔧 Development Tips

### Hot Reload Setup

```typescript
// Vite proxy for backend requests (vite.config.ts)
export default defineConfig({
  server: {
    proxy: {
      "/api": {
        target: "http://localhost:5000",
        changeOrigin: true,
      },
    },
  },
});
```

### Mock Data for Development

```typescript
// Fallback to mock data when backend is unavailable
const ApiClient = {
  async getProducts() {
    try {
      return await this.request("/products");
    } catch (error) {
      if (import.meta.env.DEV) {
        return MOCK_PRODUCTS; // Fallback for development
      }
      throw error;
    }
  },
};
```

## 📱 Mobile Responsiveness

The frontend is fully responsive and works seamlessly with the backend APIs on all devices:

- **Mobile-first design** with Tailwind CSS
- **Touch-friendly** cart and navigation
- **Optimized images** for mobile bandwidth
- **Progressive Web App** features ready

## 🎯 Production Deployment

### Environment Configuration

```bash
# Production environment variables
VITE_API_GATEWAY_URL=https://api.toystore.com/api
VITE_IDENTITY_SERVER_URL=https://auth.toystore.com
VITE_ENABLE_ANALYTICS=true
```

### Docker Integration

```dockerfile
# Frontend Dockerfile
FROM node:18-alpine as builder
WORKDIR /app
COPY package*.json ./
RUN npm ci --only=production

COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=builder /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
```

This integration ensures a seamless, production-ready e-commerce experience with real-time updates, robust error handling, and optimal performance.
