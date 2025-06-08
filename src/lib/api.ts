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

// API configuration for microservices
const API_CONFIG = {
  PRODUCT_SERVICE:
    process.env.VITE_PRODUCT_SERVICE_URL || "http://localhost:5001/api",
  USER_SERVICE:
    process.env.VITE_USER_SERVICE_URL || "http://localhost:5002/api",
  ORDER_SERVICE:
    process.env.VITE_ORDER_SERVICE_URL || "http://localhost:5003/api",
  GATEWAY: process.env.VITE_API_GATEWAY_URL || "http://localhost:5000/api",
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

// API functions
class ApiClient {
  private static baseUrl = API_CONFIG.GATEWAY;
  private static token: string | null = localStorage.getItem("auth_token");

  private static async request<T>(
    endpoint: string,
    options: RequestInit = {},
  ): Promise<ApiResponse<T>> {
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
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const data = await response.json();
      return data;
    } catch (error) {
      console.error("API request failed:", error);
      throw error;
    }
  }

  // Products API
  static async getProducts(
    filters?: ProductFilters,
  ): Promise<PaginatedResponse<Product>> {
    // Mock response for development
    await new Promise((resolve) => setTimeout(resolve, 500));

    let filteredProducts = [...MOCK_PRODUCTS];

    if (filters?.category) {
      filteredProducts = filteredProducts.filter(
        (p) => p.category.slug === filters.category,
      );
    }

    if (filters?.minPrice !== undefined) {
      filteredProducts = filteredProducts.filter(
        (p) => p.price >= filters.minPrice!,
      );
    }

    if (filters?.maxPrice !== undefined) {
      filteredProducts = filteredProducts.filter(
        (p) => p.price <= filters.maxPrice!,
      );
    }

    if (filters?.inStock) {
      filteredProducts = filteredProducts.filter((p) => p.inStock);
    }

    return {
      data: filteredProducts,
      totalCount: filteredProducts.length,
      pageSize: 12,
      currentPage: 1,
      totalPages: Math.ceil(filteredProducts.length / 12),
    };
  }

  static async getProduct(id: string): Promise<Product | null> {
    await new Promise((resolve) => setTimeout(resolve, 300));
    return MOCK_PRODUCTS.find((p) => p.id === id) || null;
  }

  static async getCategories(): Promise<Category[]> {
    await new Promise((resolve) => setTimeout(resolve, 200));
    return MOCK_CATEGORIES;
  }

  static async getFeaturedProducts(): Promise<Product[]> {
    await new Promise((resolve) => setTimeout(resolve, 400));
    return MOCK_PRODUCTS.slice(0, 4);
  }

  // Cart API (using localStorage for demo)
  static async getCart(): Promise<Cart> {
    const cartData = localStorage.getItem("toy_store_cart");
    if (cartData) {
      return JSON.parse(cartData);
    }

    const emptyCart: Cart = {
      id: "cart_" + Date.now(),
      items: [],
      totalItems: 0,
      totalPrice: 0,
    };

    localStorage.setItem("toy_store_cart", JSON.stringify(emptyCart));
    return emptyCart;
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

  static async updateCartItem(itemId: string, quantity: number): Promise<Cart> {
    const cart = await this.getCart();
    const itemIndex = cart.items.findIndex((item) => item.id === itemId);

    if (itemIndex >= 0) {
      if (quantity <= 0) {
        cart.items.splice(itemIndex, 1);
      } else {
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

  // Auth API
  static async login(email: string, password: string): Promise<User> {
    await new Promise((resolve) => setTimeout(resolve, 1000));

    // Mock login - in real app, this would call IdentityServer4
    const mockUser: User = {
      id: "user_1",
      email,
      firstName: "John",
      lastName: "Doe",
      role: email.includes("admin") ? "admin" : "customer",
      createdAt: new Date().toISOString(),
    };

    const mockToken = "mock_jwt_token_" + Date.now();
    localStorage.setItem("auth_token", mockToken);
    localStorage.setItem("user_data", JSON.stringify(mockUser));
    this.token = mockToken;

    return mockUser;
  }

  static async logout(): Promise<void> {
    localStorage.removeItem("auth_token");
    localStorage.removeItem("user_data");
    this.token = null;
  }

  static async getCurrentUser(): Promise<User | null> {
    const userData = localStorage.getItem("user_data");
    return userData ? JSON.parse(userData) : null;
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
