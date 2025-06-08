import { useState, useEffect } from "react";
import {
  Package,
  Users,
  ShoppingCart,
  DollarSign,
  TrendingUp,
  Plus,
  Edit,
  Trash2,
  Eye,
  Search,
  Filter,
  Download,
  RefreshCw,
} from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { toast } from "@/hooks/use-toast";
import Header from "@/components/Header";
import Footer from "@/components/Footer";
import { Product, Category, Order, User } from "@/types";
import ApiClient from "@/lib/api";

const Admin = () => {
  const [activeTab, setActiveTab] = useState("dashboard");
  const [products, setProducts] = useState<Product[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");
  const [isAddProductOpen, setIsAddProductOpen] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);

  const [newProduct, setNewProduct] = useState({
    name: "",
    description: "",
    shortDescription: "",
    price: 0,
    originalPrice: 0,
    categoryId: "",
    ageRange: "",
    brand: "",
    stockQuantity: 0,
    tags: "",
    imageUrls: [],
  });

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const [productsData, categoriesData] = await Promise.all([
        ApiClient.getProducts(),
        ApiClient.getCategories(),
      ]);

      setProducts(productsData.data);
      setCategories(categoriesData);
    } catch (error) {
      console.error("Failed to load admin data:", error);
      toast({
        title: "Error",
        description: "Failed to load admin data.",
        variant: "destructive",
      });
    } finally {
      setLoading(false);
    }
  };

  const handleAddProduct = async () => {
    try {
      const category = categories.find((c) => c.id === newProduct.categoryId);
      if (!category) {
        toast({
          title: "Error",
          description: "Please select a valid category.",
          variant: "destructive",
        });
        return;
      }

      const productData = {
        ...newProduct,
        category,
        inStock: newProduct.stockQuantity > 0,
        rating: 0,
        reviewCount: 0,
        tags: newProduct.tags
          .split(",")
          .map((tag) => tag.trim())
          .filter(Boolean),
        imageUrls: ["/images/products/placeholder.jpg"], // Default placeholder
      };

      await ApiClient.createProduct(productData);

      toast({
        title: "Success",
        description: "Product created successfully.",
      });

      setIsAddProductOpen(false);
      setNewProduct({
        name: "",
        description: "",
        shortDescription: "",
        price: 0,
        originalPrice: 0,
        categoryId: "",
        ageRange: "",
        brand: "",
        stockQuantity: 0,
        tags: "",
        imageUrls: [],
      });

      loadData();
    } catch (error) {
      toast({
        title: "Error",
        description: "Failed to create product.",
        variant: "destructive",
      });
    }
  };

  const handleDeleteProduct = async (productId: string) => {
    try {
      await ApiClient.deleteProduct(productId);
      toast({
        title: "Success",
        description: "Product deleted successfully.",
      });
      loadData();
    } catch (error) {
      toast({
        title: "Error",
        description: "Failed to delete product.",
        variant: "destructive",
      });
    }
  };

  const filteredProducts = products.filter(
    (product) =>
      product.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
      product.brand.toLowerCase().includes(searchTerm.toLowerCase()) ||
      product.category.name.toLowerCase().includes(searchTerm.toLowerCase()),
  );

  // Mock stats data
  const stats = {
    totalProducts: products.length,
    totalOrders: 245,
    totalRevenue: 12450.0,
    totalCustomers: 186,
    recentOrders: [
      {
        id: "ORD-001",
        customer: "John Doe",
        total: 89.99,
        status: "completed",
      },
      {
        id: "ORD-002",
        customer: "Jane Smith",
        total: 149.99,
        status: "processing",
      },
      {
        id: "ORD-003",
        customer: "Bob Johnson",
        total: 199.99,
        status: "shipped",
      },
    ],
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-white">
        <Header />
        <div className="flex items-center justify-center h-96">
          <div className="text-center">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
            <p className="text-gray-600">Loading admin dashboard...</p>
          </div>
        </div>
        <Footer />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <Header />

      <div className="container mx-auto px-4 py-8">
        {/* Header */}
        <div className="flex items-center justify-between mb-8">
          <div>
            <h1 className="text-3xl font-bold text-gray-900">
              Admin Dashboard
            </h1>
            <p className="text-gray-600">
              Manage your toy store from one place
            </p>
          </div>
          <Button onClick={() => window.location.reload()}>
            <RefreshCw className="w-4 h-4 mr-2" />
            Refresh Data
          </Button>
        </div>

        <Tabs value={activeTab} onValueChange={setActiveTab}>
          <TabsList className="grid w-full grid-cols-4">
            <TabsTrigger value="dashboard">Dashboard</TabsTrigger>
            <TabsTrigger value="products">Products</TabsTrigger>
            <TabsTrigger value="orders">Orders</TabsTrigger>
            <TabsTrigger value="customers">Customers</TabsTrigger>
          </TabsList>

          {/* Dashboard Tab */}
          <TabsContent value="dashboard" className="space-y-6">
            {/* Stats Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
              <Card>
                <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                  <CardTitle className="text-sm font-medium">
                    Total Products
                  </CardTitle>
                  <Package className="h-4 w-4 text-muted-foreground" />
                </CardHeader>
                <CardContent>
                  <div className="text-2xl font-bold">
                    {stats.totalProducts}
                  </div>
                  <p className="text-xs text-muted-foreground">
                    +2 from last month
                  </p>
                </CardContent>
              </Card>

              <Card>
                <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                  <CardTitle className="text-sm font-medium">
                    Total Orders
                  </CardTitle>
                  <ShoppingCart className="h-4 w-4 text-muted-foreground" />
                </CardHeader>
                <CardContent>
                  <div className="text-2xl font-bold">{stats.totalOrders}</div>
                  <p className="text-xs text-muted-foreground">
                    +15% from last month
                  </p>
                </CardContent>
              </Card>

              <Card>
                <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                  <CardTitle className="text-sm font-medium">
                    Total Revenue
                  </CardTitle>
                  <DollarSign className="h-4 w-4 text-muted-foreground" />
                </CardHeader>
                <CardContent>
                  <div className="text-2xl font-bold">
                    ${stats.totalRevenue.toFixed(2)}
                  </div>
                  <p className="text-xs text-muted-foreground">
                    +25% from last month
                  </p>
                </CardContent>
              </Card>

              <Card>
                <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                  <CardTitle className="text-sm font-medium">
                    Total Customers
                  </CardTitle>
                  <Users className="h-4 w-4 text-muted-foreground" />
                </CardHeader>
                <CardContent>
                  <div className="text-2xl font-bold">
                    {stats.totalCustomers}
                  </div>
                  <p className="text-xs text-muted-foreground">
                    +8 from last week
                  </p>
                </CardContent>
              </Card>
            </div>

            {/* Recent Orders */}
            <Card>
              <CardHeader>
                <CardTitle>Recent Orders</CardTitle>
                <CardDescription>Latest customer orders</CardDescription>
              </CardHeader>
              <CardContent>
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Order ID</TableHead>
                      <TableHead>Customer</TableHead>
                      <TableHead>Total</TableHead>
                      <TableHead>Status</TableHead>
                      <TableHead>Actions</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {stats.recentOrders.map((order) => (
                      <TableRow key={order.id}>
                        <TableCell className="font-medium">
                          {order.id}
                        </TableCell>
                        <TableCell>{order.customer}</TableCell>
                        <TableCell>${order.total}</TableCell>
                        <TableCell>
                          <Badge
                            variant={
                              order.status === "completed"
                                ? "default"
                                : order.status === "processing"
                                  ? "secondary"
                                  : "outline"
                            }
                          >
                            {order.status}
                          </Badge>
                        </TableCell>
                        <TableCell>
                          <Button variant="ghost" size="sm">
                            <Eye className="w-4 h-4" />
                          </Button>
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </CardContent>
            </Card>
          </TabsContent>

          {/* Products Tab */}
          <TabsContent value="products" className="space-y-6">
            {/* Products Header */}
            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-4">
                <div className="relative">
                  <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-4 h-4" />
                  <Input
                    placeholder="Search products..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="pl-10 w-64"
                  />
                </div>
                <Select>
                  <SelectTrigger className="w-40">
                    <SelectValue placeholder="Filter by category" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">All Categories</SelectItem>
                    {categories.map((category) => (
                      <SelectItem key={category.id} value={category.slug}>
                        {category.name}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              <Dialog
                open={isAddProductOpen}
                onOpenChange={setIsAddProductOpen}
              >
                <DialogTrigger asChild>
                  <Button>
                    <Plus className="w-4 h-4 mr-2" />
                    Add Product
                  </Button>
                </DialogTrigger>
                <DialogContent className="max-w-2xl max-h-[80vh] overflow-y-auto">
                  <DialogHeader>
                    <DialogTitle>Add New Product</DialogTitle>
                    <DialogDescription>
                      Create a new product for your store
                    </DialogDescription>
                  </DialogHeader>

                  <div className="grid grid-cols-2 gap-4">
                    <div className="space-y-2">
                      <Label htmlFor="name">Product Name</Label>
                      <Input
                        id="name"
                        value={newProduct.name}
                        onChange={(e) =>
                          setNewProduct((prev) => ({
                            ...prev,
                            name: e.target.value,
                          }))
                        }
                        placeholder="Super Hero Action Figure"
                      />
                    </div>

                    <div className="space-y-2">
                      <Label htmlFor="brand">Brand</Label>
                      <Input
                        id="brand"
                        value={newProduct.brand}
                        onChange={(e) =>
                          setNewProduct((prev) => ({
                            ...prev,
                            brand: e.target.value,
                          }))
                        }
                        placeholder="Hero Toys"
                      />
                    </div>

                    <div className="space-y-2 col-span-2">
                      <Label htmlFor="shortDescription">
                        Short Description
                      </Label>
                      <Input
                        id="shortDescription"
                        value={newProduct.shortDescription}
                        onChange={(e) =>
                          setNewProduct((prev) => ({
                            ...prev,
                            shortDescription: e.target.value,
                          }))
                        }
                        placeholder="Brief product description"
                      />
                    </div>

                    <div className="space-y-2 col-span-2">
                      <Label htmlFor="description">Full Description</Label>
                      <Textarea
                        id="description"
                        value={newProduct.description}
                        onChange={(e) =>
                          setNewProduct((prev) => ({
                            ...prev,
                            description: e.target.value,
                          }))
                        }
                        placeholder="Detailed product description"
                        rows={3}
                      />
                    </div>

                    <div className="space-y-2">
                      <Label htmlFor="price">Price ($)</Label>
                      <Input
                        id="price"
                        type="number"
                        step="0.01"
                        value={newProduct.price}
                        onChange={(e) =>
                          setNewProduct((prev) => ({
                            ...prev,
                            price: parseFloat(e.target.value) || 0,
                          }))
                        }
                      />
                    </div>

                    <div className="space-y-2">
                      <Label htmlFor="originalPrice">Original Price ($)</Label>
                      <Input
                        id="originalPrice"
                        type="number"
                        step="0.01"
                        value={newProduct.originalPrice}
                        onChange={(e) =>
                          setNewProduct((prev) => ({
                            ...prev,
                            originalPrice: parseFloat(e.target.value) || 0,
                          }))
                        }
                      />
                    </div>

                    <div className="space-y-2">
                      <Label htmlFor="category">Category</Label>
                      <Select
                        value={newProduct.categoryId}
                        onValueChange={(value) =>
                          setNewProduct((prev) => ({
                            ...prev,
                            categoryId: value,
                          }))
                        }
                      >
                        <SelectTrigger>
                          <SelectValue placeholder="Select category" />
                        </SelectTrigger>
                        <SelectContent>
                          {categories.map((category) => (
                            <SelectItem key={category.id} value={category.id}>
                              {category.name}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                    </div>

                    <div className="space-y-2">
                      <Label htmlFor="ageRange">Age Range</Label>
                      <Select
                        value={newProduct.ageRange}
                        onValueChange={(value) =>
                          setNewProduct((prev) => ({
                            ...prev,
                            ageRange: value,
                          }))
                        }
                      >
                        <SelectTrigger>
                          <SelectValue placeholder="Select age range" />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="0-2 years">0-2 years</SelectItem>
                          <SelectItem value="3-5 years">3-5 years</SelectItem>
                          <SelectItem value="6-8 years">6-8 years</SelectItem>
                          <SelectItem value="9-12 years">9-12 years</SelectItem>
                          <SelectItem value="12+ years">12+ years</SelectItem>
                        </SelectContent>
                      </Select>
                    </div>

                    <div className="space-y-2">
                      <Label htmlFor="stock">Stock Quantity</Label>
                      <Input
                        id="stock"
                        type="number"
                        value={newProduct.stockQuantity}
                        onChange={(e) =>
                          setNewProduct((prev) => ({
                            ...prev,
                            stockQuantity: parseInt(e.target.value) || 0,
                          }))
                        }
                      />
                    </div>

                    <div className="space-y-2">
                      <Label htmlFor="tags">Tags (comma-separated)</Label>
                      <Input
                        id="tags"
                        value={newProduct.tags}
                        onChange={(e) =>
                          setNewProduct((prev) => ({
                            ...prev,
                            tags: e.target.value,
                          }))
                        }
                        placeholder="superhero, action, collectible"
                      />
                    </div>
                  </div>

                  <DialogFooter>
                    <Button
                      variant="outline"
                      onClick={() => setIsAddProductOpen(false)}
                    >
                      Cancel
                    </Button>
                    <Button onClick={handleAddProduct}>Create Product</Button>
                  </DialogFooter>
                </DialogContent>
              </Dialog>
            </div>

            {/* Products Table */}
            <Card>
              <CardContent className="p-0">
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Product</TableHead>
                      <TableHead>Category</TableHead>
                      <TableHead>Price</TableHead>
                      <TableHead>Stock</TableHead>
                      <TableHead>Status</TableHead>
                      <TableHead>Actions</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {filteredProducts.map((product) => (
                      <TableRow key={product.id}>
                        <TableCell>
                          <div className="flex items-center space-x-3">
                            <div className="w-12 h-12 bg-gray-100 rounded-lg flex items-center justify-center">
                              <Package className="w-6 h-6 text-gray-400" />
                            </div>
                            <div>
                              <div className="font-medium">{product.name}</div>
                              <div className="text-sm text-gray-500">
                                {product.brand}
                              </div>
                            </div>
                          </div>
                        </TableCell>
                        <TableCell>{product.category.name}</TableCell>
                        <TableCell>
                          <div>
                            <div className="font-medium">${product.price}</div>
                            {product.originalPrice && (
                              <div className="text-sm text-gray-500 line-through">
                                ${product.originalPrice}
                              </div>
                            )}
                          </div>
                        </TableCell>
                        <TableCell>
                          <Badge
                            variant={
                              product.stockQuantity > 10
                                ? "default"
                                : product.stockQuantity > 0
                                  ? "secondary"
                                  : "destructive"
                            }
                          >
                            {product.stockQuantity} units
                          </Badge>
                        </TableCell>
                        <TableCell>
                          <Badge
                            variant={
                              product.inStock ? "default" : "destructive"
                            }
                          >
                            {product.inStock ? "In Stock" : "Out of Stock"}
                          </Badge>
                        </TableCell>
                        <TableCell>
                          <div className="flex space-x-2">
                            <Button variant="ghost" size="sm">
                              <Eye className="w-4 h-4" />
                            </Button>
                            <Button variant="ghost" size="sm">
                              <Edit className="w-4 h-4" />
                            </Button>
                            <Button
                              variant="ghost"
                              size="sm"
                              onClick={() => handleDeleteProduct(product.id)}
                              className="text-red-600 hover:text-red-700"
                            >
                              <Trash2 className="w-4 h-4" />
                            </Button>
                          </div>
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </CardContent>
            </Card>
          </TabsContent>

          {/* Orders Tab */}
          <TabsContent value="orders" className="space-y-6">
            <Card>
              <CardHeader>
                <CardTitle>Order Management</CardTitle>
                <CardDescription>
                  View and manage customer orders
                </CardDescription>
              </CardHeader>
              <CardContent>
                <div className="text-center py-8">
                  <ShoppingCart className="w-16 h-16 text-gray-400 mx-auto mb-4" />
                  <p className="text-gray-600">
                    Order management system will be integrated with your
                    microservices backend.
                  </p>
                </div>
              </CardContent>
            </Card>
          </TabsContent>

          {/* Customers Tab */}
          <TabsContent value="customers" className="space-y-6">
            <Card>
              <CardHeader>
                <CardTitle>Customer Management</CardTitle>
                <CardDescription>
                  View and manage customer accounts
                </CardDescription>
              </CardHeader>
              <CardContent>
                <div className="text-center py-8">
                  <Users className="w-16 h-16 text-gray-400 mx-auto mb-4" />
                  <p className="text-gray-600">
                    Customer management system will be integrated with
                    IdentityServer4.
                  </p>
                </div>
              </CardContent>
            </Card>
          </TabsContent>
        </Tabs>
      </div>

      <Footer />
    </div>
  );
};

export default Admin;
