import {
  Product,
  Category,
  User,
  Cart,
  Order,
  Review,
  ApiResponse,
  PaginatedResponse,
  ProductFilters,
  Address,
} from "@/types";

// API configuration for microservices backend
const API_CONFIG = {
  GATEWAY: process.env.VITE_API_GATEWAY_URL || "http://localhost:5000/api",
  IDENTITY_SERVER:
    process.env.VITE_IDENTITY_SERVER_URL || "http://localhost:5004",
};

// Mock data for fallback (when backend is not available)
const MOCK_CATEGORIES: Category[] = [
  {
    id: "1",
    name: "Action Figures",
    description: "Superhero and character action figures",
    imageUrl: "/images/categories/action-figures.jpg",
    slug: "action-figures",
    sortOrder: 1,
    isActive: true,
    createdAt: "2024-01-01T00:00:00Z",
    updatedAt: "2024-01-01T00:00:00Z",
  },
  {
    id: "2",
    name: "Building Blocks",
    description: "LEGO and other building sets",
    imageUrl: "/images/categories/building-blocks.jpg",
    slug: "building-blocks",
    sortOrder: 2,
    isActive: true,
    createdAt: "2024-01-01T00:00:00Z",
    updatedAt: "2024-01-01T00:00:00Z",
  },
  {
    id: "3",
    name: "Educational Toys",
    description: "Learning and STEM toys",
    imageUrl: "/images/categories/educational.jpg",
    slug: "educational-toys",
    sortOrder: 3,
    isActive: true,
    createdAt: "2024-01-01T00:00:00Z",
    updatedAt: "2024-01-01T00:00:00Z",
  },
];

const MOCK_PRODUCTS: Product[] = [
  {
    id: "1",
    name: "Super Hero Action Figure Set",
    description:
      "Complete set of 6 superhero action figures with accessories and display stand.",
    shortDescription: "Set of 6 superhero action figures with accessories",
    price: 89.99,
    originalPrice: 119.99,
    imageUrls: ["/images/products/superhero-set-1.jpg"],
    category: MOCK_CATEGORIES[0],
    ageRange: "6-12 years",
    brand: "Hero Toys",
    inStock: true,
    stockQuantity: 25,
    rating: 4.8,
    reviewCount: 142,
    tags: ["superhero", "action", "collectible"],
    createdAt: "2024-01-15T10:00:00Z",
    updatedAt: "2024-01-20T15:30:00Z",
  },
];

// API functions for real backend integration
class ApiClient {
  private static baseUrl = API_CONFIG.GATEWAY;
  private static token: string | null = localStorage.getItem("access_token");

  private static async request<T>(
    endpoint: string,
    options: RequestInit = {},
  ): Promise<T> {
    const url = `${this.baseUrl}${endpoint}`;

    const headers = {
      "Content-Type": "application/json",
      ...(this.token && { Authorization: `Bearer ${this.token}` }),
      ...options.headers,
    };

    try {
      const response = await fetch(url, {
        ...options,
        headers,
      });

      if (!response.ok) {
        if (response.status === 401) {
          // Token expired, redirect to login
          this.logout();
          window.location.href = "/login";
          throw new Error("Authentication required");
        }
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const data = await response.json();

      // Backend returns ApiResponse<T> format
      if (data.success !== undefined) {
        if (!data.success) {
          throw new Error(data.message || "API request failed");
        }
        return data.data;
      }

      return data;
    } catch (error) {
      console.error("API request failed:", error);
      throw error;
    }
  }

  // Products API - Real backend integration
  static async getProducts(
    filters?: ProductFilters,
  ): Promise<PaginatedResponse<Product>> {
    try {
      const params = new URLSearchParams();

      if (filters?.page) params.append("page", filters.page.toString());
      if (filters?.pageSize)
        params.append("pageSize", filters.pageSize.toString());
      if (filters?.searchTerm) params.append("searchTerm", filters.searchTerm);
      if (filters?.category) {
        // Convert category slug to categoryId
        const categories = await this.getCategories();
        const category = categories.find((c) => c.slug === filters.category);
        if (category) params.append("categoryId", category.id);
      }
      if (filters?.minPrice !== undefined)
        params.append("minPrice", filters.minPrice.toString());
      if (filters?.maxPrice !== undefined)
        params.append("maxPrice", filters.maxPrice.toString());
      if (filters?.brand) params.append("brand", filters.brand);
      if (filters?.inStock !== undefined)
        params.append("inStock", filters.inStock.toString());
      if (filters?.sortBy) params.append("sortBy", filters.sortBy);
      if (filters?.sortOrder) params.append("sortOrder", filters.sortOrder);

      const queryString = params.toString();
      const endpoint = `/products${queryString ? `?${queryString}` : ""}`;

      return await this.request<PaginatedResponse<Product>>(endpoint);
    } catch (error) {
      console.warn("Backend not available, using mock data");
      // Fallback to mock data
      return {
        data: MOCK_PRODUCTS,
        totalCount: MOCK_PRODUCTS.length,
        pageSize: 12,
        currentPage: 1,
        totalPages: 1,
      };
    }
  }

  static async getProduct(id: string): Promise<Product | null> {
    try {
      return await this.request<Product>(`/products/${id}`);
    } catch (error) {
      console.warn("Backend not available, using mock data");
      return MOCK_PRODUCTS.find((p) => p.id === id) || null;
    }
  }

  static async getCategories(): Promise<Category[]> {
    try {
      return await this.request<Category[]>("/categories");
    } catch (error) {
      console.warn("Backend not available, using mock data");
      return MOCK_CATEGORIES;
    }
  }

  static async getFeaturedProducts(): Promise<Product[]> {
    try {
      return await this.request<Product[]>("/products/featured?count=8");
    } catch (error) {
      console.warn("Backend not available, using mock data");
      return MOCK_PRODUCTS.slice(0, 4);
    }
  }

  // Cart API - Real backend integration
  static async getCart(): Promise<Cart> {
    if (!this.token) {
      // Return empty cart for unauthenticated users
      return {
        id: "guest_cart",
        items: [],
        totalItems: 0,
        totalPrice: 0,
      };
    }

    try {
      const cartData = await this.request<any>("/cart");
      return {
        id: cartData.id,
        items: cartData.items.map((item: any) => ({
          id: item.id,
          product: {
            id: item.productId,
            name: item.productName,
            imageUrls: [item.productImageUrl],
            price: item.unitPrice,
            category: {
              name: "",
              slug: "",
              id: "",
              description: "",
              imageUrl: "",
              sortOrder: 0,
              isActive: true,
              createdAt: "",
              updatedAt: "",
            },
            brand: "",
            ageRange: "",
            inStock: true,
            stockQuantity: 1,
            rating: 0,
            reviewCount: 0,
            tags: [],
            shortDescription: "",
            description: "",
            createdAt: "",
            updatedAt: "",
          },
          quantity: item.quantity,
          price: item.unitPrice,
        })),
        totalItems: cartData.totalItems,
        totalPrice: cartData.totalAmount,
      };
    } catch (error) {
      console.error("Failed to get cart:", error);
      return {
        id: "error_cart",
        items: [],
        totalItems: 0,
        totalPrice: 0,
      };
    }
  }

  static async addToCart(
    productId: string,
    quantity: number = 1,
  ): Promise<Cart> {
    if (!this.token) {
      throw new Error("Authentication required");
    }

    try {
      const product = await this.getProduct(productId);
      if (!product) throw new Error("Product not found");

      const addToCartData = {
        productId: productId,
        productName: product.name,
        productImageUrl: product.imageUrls[0] || "",
        quantity: quantity,
        unitPrice: product.price,
      };

      await this.request("/cart/add", {
        method: "POST",
        body: JSON.stringify(addToCartData),
      });

      return await this.getCart();
    } catch (error) {
      console.error("Failed to add to cart:", error);
      throw error;
    }
  }

  static async updateCartItem(itemId: string, quantity: number): Promise<Cart> {
    if (!this.token) {
      throw new Error("Authentication required");
    }

    await this.request(`/cart/update/${itemId}`, {
      method: "PUT",
      body: JSON.stringify(quantity),
    });

    return await this.getCart();
  }

  static async removeFromCart(itemId: string): Promise<Cart> {
    if (!this.token) {
      throw new Error("Authentication required");
    }

    await this.request(`/cart/remove/${itemId}`, {
      method: "DELETE",
    });

    return await this.getCart();
  }

  static async clearCart(): Promise<Cart> {
    if (!this.token) {
      throw new Error("Authentication required");
    }

    await this.request("/cart/clear", {
      method: "DELETE",
    });

    return await this.getCart();
  }

  // Order API
  static async createOrder(orderData: {
    paymentMethod: string;
    shippingAddress: Address;
  }): Promise<Order> {
    if (!this.token) {
      throw new Error("Authentication required");
    }

    const result = await this.request<Order>("/orders", {
      method: "POST",
      body: JSON.stringify(orderData),
    });

    return result;
  }

  static async getOrders(
    page: number = 1,
    pageSize: number = 10,
  ): Promise<PaginatedResponse<Order>> {
    if (!this.token) {
      throw new Error("Authentication required");
    }

    return await this.request<PaginatedResponse<Order>>(
      `/orders?page=${page}&pageSize=${pageSize}`,
    );
  }

  static async getOrder(orderId: string): Promise<Order> {
    if (!this.token) {
      throw new Error("Authentication required");
    }

    return await this.request<Order>(`/orders/${orderId}`);
  }

  // Auth API - IdentityServer4 Integration
  static async login(email: string, password: string): Promise<User> {
    // For demo purposes, simulate login with IdentityServer4
    // In production, this would use proper OAuth2/OIDC flow
    const mockUser: User = {
      id: "user_1",
      email,
      firstName: email.includes("admin") ? "Admin" : "John",
      lastName: email.includes("admin") ? "User" : "Doe",
      role: email.includes("admin") ? "admin" : "customer",
      createdAt: new Date().toISOString(),
    };

    // Simulate JWT token from IdentityServer4
    const mockToken = `eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.${btoa(
      JSON.stringify({
        sub: mockUser.id,
        email: mockUser.email,
        name: `${mockUser.firstName} ${mockUser.lastName}`,
        role: mockUser.role,
        aud: "toystore.api",
        iss: API_CONFIG.IDENTITY_SERVER,
        exp: Math.floor(Date.now() / 1000) + 3600,
      }),
    )}.signature`;

    localStorage.setItem("access_token", mockToken);
    localStorage.setItem("user_data", JSON.stringify(mockUser));
    this.token = mockToken;

    return mockUser;
  }

  static async logout(): Promise<void> {
    localStorage.removeItem("access_token");
    localStorage.removeItem("refresh_token");
    localStorage.removeItem("user_data");
    this.token = null;

    // In production, also call IdentityServer4 logout endpoint
    // window.location.href = `${API_CONFIG.IDENTITY_SERVER}/connect/endsession`;
  }

  static async getCurrentUser(): Promise<User | null> {
    const userData = localStorage.getItem("user_data");
    const token = localStorage.getItem("access_token");

    if (!userData || !token) {
      return null;
    }

    // Verify token hasn't expired
    try {
      const payload = JSON.parse(atob(token.split(".")[1]));
      if (payload.exp < Date.now() / 1000) {
        this.logout();
        return null;
      }
    } catch {
      this.logout();
      return null;
    }

    return JSON.parse(userData);
  }

  // Admin API
  static async createProduct(
    product: Omit<Product, "id" | "createdAt" | "updatedAt">,
  ): Promise<Product> {
    if (!this.token) {
      throw new Error("Authentication required");
    }

    const productData = {
      name: product.name,
      description: product.description,
      shortDescription: product.shortDescription,
      price: product.price,
      originalPrice: product.originalPrice,
      brand: product.brand,
      ageRange: product.ageRange,
      stockQuantity: product.stockQuantity,
      imageUrls: product.imageUrls,
      tags: product.tags,
      isFeatured: false,
      categoryId: product.category.id,
    };

    return await this.request<Product>("/products", {
      method: "POST",
      body: JSON.stringify(productData),
    });
  }

  static async updateProduct(
    id: string,
    updates: Partial<Product>,
  ): Promise<Product> {
    if (!this.token) {
      throw new Error("Authentication required");
    }

    return await this.request<Product>(`/products/${id}`, {
      method: "PUT",
      body: JSON.stringify(updates),
    });
  }

  static async deleteProduct(id: string): Promise<void> {
    if (!this.token) {
      throw new Error("Authentication required");
    }

    await this.request(`/products/${id}`, {
      method: "DELETE",
    });
  }

  // Health check for backend services
  static async checkHealth(): Promise<{ [service: string]: boolean }> {
    const services = [
      { name: "apigateway", url: "http://localhost:5000/health" },
      { name: "identity", url: "http://localhost:5004/health" },
      { name: "product", url: "http://localhost:5001/health" },
      { name: "order", url: "http://localhost:5002/health" },
      { name: "user", url: "http://localhost:5003/health" },
      { name: "inventory", url: "http://localhost:5005/health" },
      { name: "notification", url: "http://localhost:5006/health" },
    ];

    const results: { [service: string]: boolean } = {};

    await Promise.allSettled(
      services.map(async (service) => {
        try {
          const response = await fetch(service.url, {
            method: "GET",
            signal: AbortSignal.timeout(5000),
          });
          results[service.name] = response.ok;
        } catch {
          results[service.name] = false;
        }
      }),
    );

    return results;
  }
}

export default ApiClient;
