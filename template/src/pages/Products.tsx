import { useState } from "react";
import { Search, Filter, Grid, List } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import Header from "@/components/Header";
import Footer from "@/components/Footer";

const Products = () => {
  const [viewMode, setViewMode] = useState<"grid" | "list">("grid");
  const [searchTerm, setSearchTerm] = useState("");

  // Mock products data
  const products = [
    {
      id: "1",
      name: "Super Hero Action Figure",
      description: "Amazing superhero figure with movable joints",
      price: 24.99,
      category: "Action Figures",
      brand: "ToyStore",
      ageRange: "4+",
      rating: 4.5,
      reviewCount: 128,
      inStock: true,
    },
    {
      id: "2",
      name: "Building Blocks Set",
      description: "Creative building blocks for endless fun",
      price: 39.99,
      category: "Building Blocks",
      brand: "ToyStore",
      ageRange: "3+",
      rating: 4.8,
      reviewCount: 95,
      inStock: true,
    },
    {
      id: "3",
      name: "STEM Learning Kit",
      description: "Educational toy for science and math learning",
      price: 49.99,
      category: "Educational",
      brand: "ToyStore",
      ageRange: "6+",
      rating: 4.7,
      reviewCount: 67,
      inStock: true,
    },
  ];

  const categories = [
    "All",
    "Action Figures",
    "Building Blocks",
    "Educational",
    "Puzzles",
  ];

  return (
    <div className="min-h-screen bg-white">
      <Header />

      <div className="container mx-auto px-4 py-8">
        <div className="flex flex-col lg:flex-row gap-8">
          {/* Filters Sidebar */}
          <aside className="lg:w-64 space-y-6">
            <div>
              <h3 className="font-semibold text-gray-900 mb-3">Categories</h3>
              <div className="space-y-2">
                {categories.map((category) => (
                  <button
                    key={category}
                    className="block w-full text-left px-3 py-2 rounded-md hover:bg-gray-100 text-gray-700"
                  >
                    {category}
                  </button>
                ))}
              </div>
            </div>

            <div>
              <h3 className="font-semibold text-gray-900 mb-3">Price Range</h3>
              <div className="space-y-2">
                <button className="block w-full text-left px-3 py-2 rounded-md hover:bg-gray-100 text-gray-700">
                  Under $25
                </button>
                <button className="block w-full text-left px-3 py-2 rounded-md hover:bg-gray-100 text-gray-700">
                  $25 - $50
                </button>
                <button className="block w-full text-left px-3 py-2 rounded-md hover:bg-gray-100 text-gray-700">
                  $50 - $100
                </button>
                <button className="block w-full text-left px-3 py-2 rounded-md hover:bg-gray-100 text-gray-700">
                  Over $100
                </button>
              </div>
            </div>
          </aside>

          {/* Main Content */}
          <main className="flex-1">
            {/* Search and Controls */}
            <div className="flex flex-col sm:flex-row gap-4 mb-6">
              <div className="flex-1 relative">
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
                <Input
                  placeholder="Search products..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="pl-10"
                />
              </div>

              <div className="flex gap-2">
                <Button
                  variant={viewMode === "grid" ? "default" : "outline"}
                  size="sm"
                  onClick={() => setViewMode("grid")}
                >
                  <Grid className="w-4 h-4" />
                </Button>
                <Button
                  variant={viewMode === "list" ? "default" : "outline"}
                  size="sm"
                  onClick={() => setViewMode("list")}
                >
                  <List className="w-4 h-4" />
                </Button>
              </div>
            </div>

            {/* Products Grid */}
            <div
              className={`grid gap-6 ${
                viewMode === "grid"
                  ? "grid-cols-1 sm:grid-cols-2 lg:grid-cols-3"
                  : "grid-cols-1"
              }`}
            >
              {products.map((product) => (
                <Card
                  key={product.id}
                  className="hover:shadow-lg transition-shadow"
                >
                  <CardContent className="p-4">
                    <div className="aspect-square bg-gradient-to-br from-gray-100 to-gray-200 rounded-lg mb-4 flex items-center justify-center">
                      <span className="text-gray-400 text-sm">
                        Product Image
                      </span>
                    </div>

                    <div className="space-y-2">
                      <div className="flex items-center gap-2">
                        <Badge variant="secondary" className="text-xs">
                          {product.category}
                        </Badge>
                        <span className="text-xs text-gray-500">
                          {product.ageRange}
                        </span>
                      </div>

                      <h3 className="font-semibold text-gray-900">
                        {product.name}
                      </h3>

                      <p className="text-sm text-gray-600">
                        {product.description}
                      </p>

                      <div className="flex items-center justify-between">
                        <span className="text-lg font-bold text-gray-900">
                          ${product.price}
                        </span>
                        <Button size="sm">Add to Cart</Button>
                      </div>
                    </div>
                  </CardContent>
                </Card>
              ))}
            </div>

            {/* Pagination */}
            <div className="flex justify-center mt-8">
              <div className="flex gap-2">
                <Button variant="outline" size="sm" disabled>
                  Previous
                </Button>
                <Button variant="default" size="sm">
                  1
                </Button>
                <Button variant="outline" size="sm">
                  2
                </Button>
                <Button variant="outline" size="sm">
                  3
                </Button>
                <Button variant="outline" size="sm">
                  Next
                </Button>
              </div>
            </div>
          </main>
        </div>
      </div>

      <Footer />
    </div>
  );
};

export default Products;
