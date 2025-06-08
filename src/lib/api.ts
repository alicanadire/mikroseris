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
} from "@/types";

// API configuration for microservices backend
const API_CONFIG = {
  GATEWAY: process.env.VITE_API_GATEWAY_URL || 'http://localhost:5000/api',
  IDENTITY_SERVER: process.env.VITE_IDENTITY_SERVER_URL || 'http://localhost:5004',
};

// Mock data for development (replace with actual API calls)
const MOCK_CATEGORIES: Category[] = [
  {
    id: "1",
    name: "Action Figures",
    description: "Superhero and character action figures",
    imageUrl: "/images/categories/action-figures.jpg",
    slug: "action-figures",
  },
  {
    id: "2",
    name: "Building Blocks",
    description: "LEGO and other building sets",
    imageUrl: "/images/categories/building-blocks.jpg",
    slug: "building-blocks",
  },
  {
    id: "3",
    name: "Dolls",
    description: "Barbie, baby dolls, and fashion dolls",
    imageUrl: "/images/categories/dolls.jpg",
    slug: "dolls",
  },
  {
    id: "4",
    name: "Educational Toys",
    description: "Learning and STEM toys",
    imageUrl: "/images/categories/educational.jpg",
    slug: "educational-toys",
  },
  {
    id: "5",
    name: "Remote Control",
    description: "RC cars, drones, and robots",
    imageUrl: "/images/categories/rc-toys.jpg",
    slug: "remote-control",
  },
  {
    id: "6",
    name: "Board Games",
    description: "Family games and puzzles",
    imageUrl: "/images/categories/board-games.jpg",
    slug: "board-games",
  },
];

const MOCK_PRODUCTS: Product[] = [
  {
    id: "1",
    name: "Super Hero Action Figure Set",
    description:
      "Complete set of 6 superhero action figures with accessories and display stand. Perfect for imaginative play and collecting.",
    shortDescription: "Set of 6 superhero action figures with accessories",
    price: 89.99,
    originalPrice: 119.99,
    imageUrls: [
      "/images/products/superhero-set-1.jpg",
      "/images/products/superhero-set-2.jpg",
      "/images/products/superhero-set-3.jpg",
    ],
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
  {
    id: "2",
    name: "Ultimate Building Castle Set",
    description:
      "Massive 2000-piece castle building set with knights, dragons, and medieval accessories. Includes detailed instruction manual.",
    shortDescription: "2000-piece medieval castle building set",
    price: 149.99,
    imageUrls: [
      "/images/products/castle-set-1.jpg",
      "/images/products/castle-set-2.jpg",
    ],
    category: MOCK_CATEGORIES[1],
    ageRange: "8+ years",
    brand: "BlockMaster",
    inStock: true,
    stockQuantity: 12,
    rating: 4.9,
    reviewCount: 89,
    tags: ["building", "castle", "medieval"],
    createdAt: "2024-01-10T08:00:00Z",
    updatedAt: "2024-01-18T12:00:00Z",
  },
  {
    id: "3",
    name: "Fashion Doll Dream House",
    description:
      "Three-story dollhouse with furniture, elevator, and over 50 accessories. Lights up and plays music.",
    shortDescription: "Three-story dollhouse with lights and sounds",
    price: 199.99,
    originalPrice: 249.99,
    imageUrls: [
      "/images/products/dollhouse-1.jpg",
      "/images/products/dollhouse-2.jpg",
      "/images/products/dollhouse-3.jpg",
    ],
    category: MOCK_CATEGORIES[2],
    ageRange: "3-10 years",
    brand: "DreamPlay",
    inStock: true,
    stockQuantity: 8,
    rating: 4.7,
    reviewCount: 156,
    tags: ["dollhouse", "fashion", "interactive"],
    createdAt: "2024-01-05T14:00:00Z",
    updatedAt: "2024-01-22T09:15:00Z",
  },
  {
    id: "4",
    name: "STEM Robotics Kit",
    description:
      "Build and program your own robot! Includes sensors, motors, and visual programming interface. Great for learning coding.",
    shortDescription: "Programmable robot building kit for kids",
    price: 129.99,
    imageUrls: [
      "/images/products/robot-kit-1.jpg",
      "/images/products/robot-kit-2.jpg",
    ],
    category: MOCK_CATEGORIES[3],
    ageRange: "10+ years",
    brand: "TechKids",
    inStock: true,
    stockQuantity: 15,
    rating: 4.9,
    reviewCount: 73,
    tags: ["STEM", "robotics", "programming", "educational"],
    createdAt: "2024-01-12T11:00:00Z",
    updatedAt: "2024-01-19T16:45:00Z",
  },
  {
    id: "5",
    name: "Racing Drone Pro",
    description:
      "High-speed racing drone with HD camera, LED lights, and smartphone app control. Indoor and outdoor use.",
    shortDescription: "HD camera racing drone with app control",
    price: 179.99,
    imageUrls: ["/images/products/drone-1.jpg", "/images/products/drone-2.jpg"],
    category: MOCK_CATEGORIES[4],
    ageRange: "12+ years",
    brand: "SkyTech",
    inStock: true,
    stockQuantity: 20,
    rating: 4.6,
    reviewCount: 95,
    tags: ["drone", "racing", "camera", "remote-control"],
    createdAt: "2024-01-08T13:00:00Z",
    updatedAt: "2024-01-21T11:30:00Z",
  },
  {
    id: "6",
    name: "Family Strategy Game Collection",
    description:
      "Set of 5 classic strategy board games including chess, checkers, backgammon, and more. Premium wooden pieces.",
    shortDescription: "Collection of 5 classic strategy board games",
    price: 79.99,
    originalPrice: 99.99,
    imageUrls: [
      "/images/products/board-games-1.jpg",
      "/images/products/board-games-2.jpg",
    ],
    category: MOCK_CATEGORIES[5],
    ageRange: "8+ years",
    brand: "GameMaster",
    inStock: true,
    stockQuantity: 30,
    rating: 4.5,
    reviewCount: 124,
    tags: ["board-games", "strategy", "family", "classic"],
    createdAt: "2024-01-14T09:00:00Z",
    updatedAt: "2024-01-20T14:00:00Z",
  },
];

// API functions for real backend integration
class ApiClient {
  private static baseUrl = API_CONFIG.GATEWAY;
  private static token: string | null = localStorage.getItem('access_token');

  private static async request<T>(
    endpoint: string,
    options: RequestInit = {}
  ): Promise<T> {
    const url = `${this.baseUrl}${endpoint}`;

    const headers = {
      'Content-Type': 'application/json',
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
          window.location.href = '/login';
          throw new Error('Authentication required');
        }
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const data = await response.json();

      // Backend returns ApiResponse<T> format
      if (data.success !== undefined) {
        if (!data.success) {
          throw new Error(data.message || 'API request failed');
        }
        return data.data;
      }

      return data;
    } catch (error) {
      console.error('API request failed:', error);
      throw error;
    }
  }
  }

  // Products API - Real backend integration
  static async getProducts(filters?: ProductFilters): Promise<PaginatedResponse<Product>> {
    const params = new URLSearchParams();

    if (filters?.page) params.append('page', filters.page.toString());
    if (filters?.pageSize) params.append('pageSize', filters.pageSize.toString());
    if (filters?.searchTerm) params.append('searchTerm', filters.searchTerm);
    if (filters?.category) {
      // Convert category slug to categoryId
      const categories = await this.getCategories();
      const category = categories.find(c => c.slug === filters.category);
      if (category) params.append('categoryId', category.id);
    }
    if (filters?.minPrice !== undefined) params.append('minPrice', filters.minPrice.toString());
    if (filters?.maxPrice !== undefined) params.append('maxPrice', filters.maxPrice.toString());
    if (filters?.brand) params.append('brand', filters.brand);
    if (filters?.inStock !== undefined) params.append('inStock', filters.inStock.toString());
    if (filters?.sortBy) params.append('sortBy', filters.sortBy);
    if (filters?.sortOrder) params.append('sortOrder', filters.sortOrder);

    const queryString = params.toString();
    const endpoint = `/products${queryString ? `?${queryString}` : ''}`;

    return await this.request<PaginatedResponse<Product>>(endpoint);
  }
  }

  static async getProduct(id: string): Promise<Product | null> {
    try {
      return await this.request<Product>(`/products/${id}`);
    } catch (error) {
      console.error('Failed to get product:', error);
      return null;
    }
  }

  static async getCategories(): Promise<Category[]> {
    return await this.request<Category[]>('/categories');
  }

  static async getFeaturedProducts(): Promise<Product[]> {
    return await this.request<Product[]>('/products/featured?count=8');
  }

  // Cart API - Real backend integration
  static async getCart(): Promise<Cart> {
    if (!this.token) {
      // Return empty cart for unauthenticated users
      return {
        id: 'guest_cart',
        items: [],
        totalItems: 0,
        totalPrice: 0
      };
    }

    try {
      const cartData = await this.request<any>('/cart');
      return {
        id: cartData.id,
        items: cartData.items.map((item: any) => ({
          id: item.id,
          product: {
            id: item.productId,
            name: item.productName,
            imageUrls: [item.productImageUrl],
            price: item.unitPrice,
            category: { name: '', slug: '', id: '', description: '', imageUrl: '' },
            brand: '',
            ageRange: '',
            inStock: true,
            stockQuantity: 1,
            rating: 0,
            reviewCount: 0,
            tags: [],
            shortDescription: '',
            description: '',
            createdAt: '',
            updatedAt: ''
          },
          quantity: item.quantity,
          price: item.unitPrice
        })),
        totalItems: cartData.totalItems,
        totalPrice: cartData.totalAmount
      };
    } catch (error) {
      console.error('Failed to get cart:', error);
      return {
        id: 'error_cart',
        items: [],
        totalItems: 0,
        totalPrice: 0
      };
    }
  }

  static async addToCart(
    productId: string,
    quantity: number = 1,
  ): Promise<Cart> {
    const cart = await this.getCart();
    const product = await this.getProduct(productId);

    if (!product) throw new Error("Product not found");

    const existingItemIndex = cart.items.findIndex(
      (item) => item.product.id === productId,
    );

    if (existingItemIndex >= 0) {
      cart.items[existingItemIndex].quantity += quantity;
    } else {
      cart.items.push({
        id: "item_" + Date.now(),
        product,
        quantity,
        price: product.price,
      });
    }

    cart.totalItems = cart.items.reduce((sum, item) => sum + item.quantity, 0);
    cart.totalPrice = cart.items.reduce(
      (sum, item) => sum + item.price * item.quantity,
      0,
    );

    localStorage.setItem("toy_store_cart", JSON.stringify(cart));
    return cart;
  }

  static async addToCart(productId: string, quantity: number = 1): Promise<Cart> {
    if (!this.token) {
      throw new Error('Authentication required');
    }

    const product = await this.getProduct(productId);
    if (!product) throw new Error('Product not found');

    const addToCartData = {
      productId: productId,
      productName: product.name,
      productImageUrl: product.imageUrls[0] || '',
      quantity: quantity,
      unitPrice: product.price
    };

    await this.request('/cart/add', {
      method: 'POST',
      body: JSON.stringify(addToCartData)
    });

    return await this.getCart();
  }
        cart.items[itemIndex].quantity = quantity;
      }
    }

    cart.totalItems = cart.items.reduce((sum, item) => sum + item.quantity, 0);
    cart.totalPrice = cart.items.reduce(
      (sum, item) => sum + item.price * item.quantity,
      0,
    );

    localStorage.setItem("toy_store_cart", JSON.stringify(cart));
    return cart;
  }

  static async removeFromCart(itemId: string): Promise<Cart> {
    return this.updateCartItem(itemId, 0);
  }

  static async clearCart(): Promise<Cart> {
    const emptyCart: Cart = {
      id: "cart_" + Date.now(),
      items: [],
      totalItems: 0,
      totalPrice: 0,
    };

    localStorage.setItem("toy_store_cart", JSON.stringify(emptyCart));
    return emptyCart;
  }

  // Auth API - IdentityServer4 Integration
  static async login(email: string, password: string): Promise<User> {
    // For demo purposes, simulate login with IdentityServer4
    // In production, this would use proper OAuth2/OIDC flow
    const mockUser: User = {
      id: 'user_1',
      email,
      firstName: email.includes('admin') ? 'Admin' : 'John',
      lastName: email.includes('admin') ? 'User' : 'Doe',
      role: email.includes('admin') ? 'admin' : 'customer',
      createdAt: new Date().toISOString()
    };

    // Simulate JWT token from IdentityServer4
    const mockToken = `eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.${btoa(JSON.stringify({
      sub: mockUser.id,
      email: mockUser.email,
      name: `${mockUser.firstName} ${mockUser.lastName}`,
      role: mockUser.role,
      aud: 'toystore.api',
      iss: API_CONFIG.IDENTITY_SERVER,
      exp: Math.floor(Date.now() / 1000) + 3600
    }))}.signature`;

    localStorage.setItem('access_token', mockToken);
    localStorage.setItem('user_data', JSON.stringify(mockUser));
    this.token = mockToken;

    return mockUser;
  }

  static async logout(): Promise<void> {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('user_data');
    this.token = null;

    // In production, also call IdentityServer4 logout endpoint
    // window.location.href = `${API_CONFIG.IDENTITY_SERVER}/connect/endsession`;
  }

  static async getCurrentUser(): Promise<User | null> {
    const userData = localStorage.getItem('user_data');
    const token = localStorage.getItem('access_token');

    if (!userData || !token) {
      return null;
    }

    // Verify token hasn't expired
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
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
    await new Promise((resolve) => setTimeout(resolve, 800));

    const newProduct: Product = {
      ...product,
      id: "product_" + Date.now(),
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    };

    MOCK_PRODUCTS.push(newProduct);
    return newProduct;
  }

  static async updateProduct(
    id: string,
    updates: Partial<Product>,
  ): Promise<Product> {
    await new Promise((resolve) => setTimeout(resolve, 600));

    const index = MOCK_PRODUCTS.findIndex((p) => p.id === id);
    if (index === -1) throw new Error("Product not found");

    MOCK_PRODUCTS[index] = {
      ...MOCK_PRODUCTS[index],
      ...updates,
      updatedAt: new Date().toISOString(),
    };

    return MOCK_PRODUCTS[index];
  }

  static async deleteProduct(id: string): Promise<void> {
    await new Promise((resolve) => setTimeout(resolve, 400));

    const index = MOCK_PRODUCTS.findIndex((p) => p.id === id);
    if (index === -1) throw new Error("Product not found");

    MOCK_PRODUCTS.splice(index, 1);
  }
}

export default ApiClient;