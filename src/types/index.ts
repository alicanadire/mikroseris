export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: "customer" | "admin";
  createdAt: string;
}

export interface Category {
  id: string;
  name: string;
  description: string;
  imageUrl: string;
  slug: string;
}

export interface Product {
  id: string;
  name: string;
  description: string;
  shortDescription: string;
  price: number;
  originalPrice?: number;
  imageUrls: string[];
  category: Category;
  ageRange: string;
  brand: string;
  inStock: boolean;
  stockQuantity: number;
  rating: number;
  reviewCount: number;
  tags: string[];
  createdAt: string;
  updatedAt: string;
}

export interface CartItem {
  id: string;
  product: Product;
  quantity: number;
  price: number;
}

export interface Cart {
  id: string;
  items: CartItem[];
  totalItems: number;
  totalPrice: number;
  userId?: string;
}

export interface Order {
  id: string;
  userId: string;
  items: CartItem[];
  totalPrice: number;
  status: "pending" | "processing" | "shipped" | "delivered" | "cancelled";
  shippingAddress: Address;
  billingAddress: Address;
  paymentMethod: "credit_card" | "paypal" | "bank_transfer";
  createdAt: string;
  updatedAt: string;
}

export interface Address {
  firstName: string;
  lastName: string;
  company?: string;
  street: string;
  city: string;
  state: string;
  zipCode: string;
  country: string;
  phone: string;
}

export interface Review {
  id: string;
  productId: string;
  userId: string;
  userName: string;
  rating: number;
  title: string;
  comment: string;
  createdAt: string;
  verified: boolean;
}

export interface AuthState {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  token: string | null;
}

export interface ApiResponse<T> {
  data: T;
  message: string;
  success: boolean;
  statusCode: number;
}

export interface PaginatedResponse<T> {
  data: T[];
  totalCount: number;
  pageSize: number;
  currentPage: number;
  totalPages: number;
}

export interface ProductFilters {
  page?: number;
  pageSize?: number;
  searchTerm?: string;
  category?: string;
  minPrice?: number;
  maxPrice?: number;
  brand?: string;
  ageRange?: string;
  inStock?: boolean;
  rating?: number;
  sortBy?: "name" | "price" | "rating" | "newest";
  sortOrder?: "asc" | "desc";
}
