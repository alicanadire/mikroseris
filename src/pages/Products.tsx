import { useState, useEffect } from "react";
import { useSearchParams } from "react-router-dom";
import { Filter, Grid, List, SlidersHorizontal, X } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Checkbox } from "@/components/ui/checkbox";
import { Slider } from "@/components/ui/slider";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@/components/ui/sheet";
import { Badge } from "@/components/ui/badge";
import { Separator } from "@/components/ui/separator";
import Header from "@/components/Header";
import Footer from "@/components/Footer";
import ProductCard from "@/components/ProductCard";
import { Product, Category, ProductFilters } from "@/types";
import ApiClient from "@/lib/api";

const Products = () => {
  const [searchParams, setSearchParams] = useSearchParams();
  const [products, setProducts] = useState<Product[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [viewMode, setViewMode] = useState<"grid" | "list">("grid");
  const [isFilterOpen, setIsFilterOpen] = useState(false);

  const [filters, setFilters] = useState<ProductFilters>({
    category: searchParams.get("category") || "",
    minPrice: 0,
    maxPrice: 500,
    brand: "",
    ageRange: "",
    inStock: true,
    sortBy: "name",
    sortOrder: "asc",
  });

  const brands = [
    "Hero Toys",
    "BlockMaster",
    "DreamPlay",
    "TechKids",
    "SkyTech",
    "GameMaster",
  ];
  const ageRanges = [
    "0-2 years",
    "3-5 years",
    "6-8 years",
    "9-12 years",
    "12+ years",
  ];

  useEffect(() => {
    loadCategories();
  }, []);

  useEffect(() => {
    loadProducts();
  }, [filters, searchParams]);

  useEffect(() => {
    // Update filters based on URL params
    const categoryFromUrl = searchParams.get("category");
    const searchFromUrl = searchParams.get("search");

    if (categoryFromUrl && categoryFromUrl !== filters.category) {
      setFilters((prev) => ({ ...prev, category: categoryFromUrl }));
    }
  }, [searchParams]);

  const loadCategories = async () => {
    try {
      const categoriesData = await ApiClient.getCategories();
      setCategories(categoriesData);
    } catch (error) {
      console.error("Failed to load categories:", error);
    }
  };

  const loadProducts = async () => {
    setLoading(true);
    try {
      const response = await ApiClient.getProducts(filters);
      setProducts(response.data);
    } catch (error) {
      console.error("Failed to load products:", error);
    } finally {
      setLoading(false);
    }
  };

  const updateFilter = <K extends keyof ProductFilters>(
    key: K,
    value: ProductFilters[K],
  ) => {
    setFilters((prev) => ({ ...prev, [key]: value }));
  };

  const clearFilters = () => {
    setFilters({
      category: "",
      minPrice: 0,
      maxPrice: 500,
      brand: "",
      ageRange: "",
      inStock: true,
      sortBy: "name",
      sortOrder: "asc",
    });
    setSearchParams({});
  };

  const getActiveFiltersCount = () => {
    let count = 0;
    if (filters.category) count++;
    if (filters.brand) count++;
    if (filters.ageRange) count++;
    if (filters.minPrice && filters.minPrice > 0) count++;
    if (filters.maxPrice && filters.maxPrice < 500) count++;
    return count;
  };

  const FilterContent = () => (
    <div className="space-y-6">
      {/* Categories */}
      <div>
        <Label className="text-base font-semibold mb-3 block">Categories</Label>
        <div className="space-y-2">
          <div className="flex items-center space-x-2">
            <Checkbox
              checked={filters.category === ""}
              onCheckedChange={() => updateFilter("category", "")}
            />
            <label className="text-sm">All Categories</label>
          </div>
          {categories.map((category) => (
            <div key={category.id} className="flex items-center space-x-2">
              <Checkbox
                checked={filters.category === category.slug}
                onCheckedChange={() => updateFilter("category", category.slug)}
              />
              <label className="text-sm">{category.name}</label>
            </div>
          ))}
        </div>
      </div>

      <Separator />

      {/* Price Range */}
      <div>
        <Label className="text-base font-semibold mb-3 block">
          Price Range
        </Label>
        <div className="space-y-4">
          <Slider
            value={[filters.minPrice || 0, filters.maxPrice || 500]}
            onValueChange={([min, max]) => {
              updateFilter("minPrice", min);
              updateFilter("maxPrice", max);
            }}
            max={500}
            step={10}
            className="w-full"
          />
          <div className="flex items-center justify-between text-sm text-gray-600">
            <span>${filters.minPrice || 0}</span>
            <span>${filters.maxPrice || 500}</span>
          </div>
        </div>
      </div>

      <Separator />

      {/* Brands */}
      <div>
        <Label className="text-base font-semibold mb-3 block">Brands</Label>
        <div className="space-y-2">
          <div className="flex items-center space-x-2">
            <Checkbox
              checked={filters.brand === ""}
              onCheckedChange={() => updateFilter("brand", "")}
            />
            <label className="text-sm">All Brands</label>
          </div>
          {brands.map((brand) => (
            <div key={brand} className="flex items-center space-x-2">
              <Checkbox
                checked={filters.brand === brand}
                onCheckedChange={() => updateFilter("brand", brand)}
              />
              <label className="text-sm">{brand}</label>
            </div>
          ))}
        </div>
      </div>

      <Separator />

      {/* Age Range */}
      <div>
        <Label className="text-base font-semibold mb-3 block">Age Range</Label>
        <div className="space-y-2">
          <div className="flex items-center space-x-2">
            <Checkbox
              checked={filters.ageRange === ""}
              onCheckedChange={() => updateFilter("ageRange", "")}
            />
            <label className="text-sm">All Ages</label>
          </div>
          {ageRanges.map((age) => (
            <div key={age} className="flex items-center space-x-2">
              <Checkbox
                checked={filters.ageRange === age}
                onCheckedChange={() => updateFilter("ageRange", age)}
              />
              <label className="text-sm">{age}</label>
            </div>
          ))}
        </div>
      </div>

      <Separator />

      {/* Availability */}
      <div>
        <Label className="text-base font-semibold mb-3 block">
          Availability
        </Label>
        <div className="flex items-center space-x-2">
          <Checkbox
            checked={filters.inStock}
            onCheckedChange={(checked) =>
              updateFilter("inStock", checked as boolean)
            }
          />
          <label className="text-sm">In Stock Only</label>
        </div>
      </div>

      {/* Clear Filters */}
      {getActiveFiltersCount() > 0 && (
        <Button variant="outline" onClick={clearFilters} className="w-full">
          <X className="w-4 h-4 mr-2" />
          Clear All Filters
        </Button>
      )}
    </div>
  );

  return (
    <div className="min-h-screen bg-white">
      <Header />

      <div className="container mx-auto px-4 py-8">
        {/* Page Header */}
        <div className="mb-8">
          <h1 className="text-3xl lg:text-4xl font-bold text-gray-900 mb-4">
            {filters.category
              ? categories.find((c) => c.slug === filters.category)?.name ||
                "Products"
              : "All Products"}
          </h1>
          <p className="text-gray-600">
            Discover our amazing collection of toys for every age and interest
          </p>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-4 gap-8">
          {/* Desktop Filters Sidebar */}
          <div className="hidden lg:block">
            <div className="sticky top-24 bg-white border rounded-lg p-6">
              <div className="flex items-center justify-between mb-6">
                <h2 className="text-lg font-semibold">Filters</h2>
                {getActiveFiltersCount() > 0 && (
                  <Badge variant="secondary">
                    {getActiveFiltersCount()} active
                  </Badge>
                )}
              </div>
              <FilterContent />
            </div>
          </div>

          {/* Main Content */}
          <div className="lg:col-span-3">
            {/* Toolbar */}
            <div className="flex items-center justify-between mb-6 p-4 bg-gray-50 rounded-lg">
              <div className="flex items-center gap-4">
                {/* Mobile Filter Button */}
                <Sheet open={isFilterOpen} onOpenChange={setIsFilterOpen}>
                  <SheetTrigger asChild>
                    <Button variant="outline" size="sm" className="lg:hidden">
                      <SlidersHorizontal className="w-4 h-4 mr-2" />
                      Filters
                      {getActiveFiltersCount() > 0 && (
                        <Badge variant="secondary" className="ml-2">
                          {getActiveFiltersCount()}
                        </Badge>
                      )}
                    </Button>
                  </SheetTrigger>
                  <SheetContent side="left" className="w-80">
                    <SheetHeader>
                      <SheetTitle>Filters</SheetTitle>
                    </SheetHeader>
                    <div className="mt-6">
                      <FilterContent />
                    </div>
                  </SheetContent>
                </Sheet>

                <span className="text-sm text-gray-600">
                  {products.length} products found
                </span>
              </div>

              <div className="flex items-center gap-4">
                {/* Sort */}
                <Select
                  value={filters.sortBy}
                  onValueChange={(value) =>
                    updateFilter("sortBy", value as any)
                  }
                >
                  <SelectTrigger className="w-40">
                    <SelectValue placeholder="Sort by" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="name">Name</SelectItem>
                    <SelectItem value="price">Price</SelectItem>
                    <SelectItem value="rating">Rating</SelectItem>
                    <SelectItem value="newest">Newest</SelectItem>
                  </SelectContent>
                </Select>

                {/* View Mode */}
                <div className="hidden sm:flex border rounded-lg">
                  <Button
                    variant={viewMode === "grid" ? "default" : "ghost"}
                    size="sm"
                    onClick={() => setViewMode("grid")}
                    className="rounded-r-none"
                  >
                    <Grid className="w-4 h-4" />
                  </Button>
                  <Button
                    variant={viewMode === "list" ? "default" : "ghost"}
                    size="sm"
                    onClick={() => setViewMode("list")}
                    className="rounded-l-none"
                  >
                    <List className="w-4 h-4" />
                  </Button>
                </div>
              </div>
            </div>

            {/* Products Grid/List */}
            {loading ? (
              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
                {[...Array(6)].map((_, i) => (
                  <div
                    key={i}
                    className="bg-gray-200 rounded-lg h-96 animate-pulse"
                  />
                ))}
              </div>
            ) : products.length === 0 ? (
              <div className="text-center py-16">
                <div className="text-gray-400 mb-4">
                  <Filter className="w-16 h-16 mx-auto" />
                </div>
                <h3 className="text-xl font-semibold text-gray-900 mb-2">
                  No products found
                </h3>
                <p className="text-gray-600 mb-6">
                  Try adjusting your filters or search terms
                </p>
                <Button onClick={clearFilters} variant="outline">
                  Clear Filters
                </Button>
              </div>
            ) : (
              <div
                className={
                  viewMode === "grid"
                    ? "grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6"
                    : "space-y-6"
                }
              >
                {products.map((product) => (
                  <ProductCard key={product.id} product={product} />
                ))}
              </div>
            )}

            {/* Load More Button */}
            {products.length > 0 && (
              <div className="text-center mt-12">
                <Button variant="outline" size="lg">
                  Load More Products
                </Button>
              </div>
            )}
          </div>
        </div>
      </div>

      <Footer />
    </div>
  );
};

export default Products;
