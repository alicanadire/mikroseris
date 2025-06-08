import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import {
  ArrowRight,
  Star,
  Truck,
  Shield,
  RotateCcw,
  Heart,
  Gift,
} from "lucide-react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent } from "@/components/ui/card";
import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselNext,
  CarouselPrevious,
} from "@/components/ui/carousel";
import Header from "@/components/Header";
import Footer from "@/components/Footer";
import ProductCard from "@/components/ProductCard";
import { Product, Category } from "@/types";
import ApiClient from "@/lib/api";

const Index = () => {
  const [featuredProducts, setFeaturedProducts] = useState<Product[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    // Set immediate fallback data for fast loading
    const fallbackCategories = [
      {
        id: "1",
        name: "Action Figures",
        slug: "action-figures",
        description: "Superhero and character action figures",
        imageUrl: "/images/categories/action-figures.jpg",
        sortOrder: 1,
        isActive: true,
        createdAt: "",
        updatedAt: "",
      },
      {
        id: "2",
        name: "Building Blocks",
        slug: "building-blocks",
        description: "LEGO and other building sets",
        imageUrl: "/images/categories/building-blocks.jpg",
        sortOrder: 2,
        isActive: true,
        createdAt: "",
        updatedAt: "",
      },
      {
        id: "3",
        name: "Educational Toys",
        slug: "educational-toys",
        description: "Learning and STEM toys",
        imageUrl: "/images/categories/educational.jpg",
        sortOrder: 3,
        isActive: true,
        createdAt: "",
        updatedAt: "",
      },
    ];

    const fallbackProducts = [
      {
        id: "1",
        name: "Super Hero Action Figure",
        description: "Amazing superhero figure with movable joints",
        shortDescription: "Amazing superhero figure",
        price: 24.99,
        imageUrls: ["/images/products/action-figure-1.jpg"],
        category: fallbackCategories[0],
        brand: "ToyStore",
        ageRange: "4+",
        inStock: true,
        stockQuantity: 50,
        rating: 4.5,
        reviewCount: 128,
        tags: ["superhero", "action"],
        createdAt: "",
        updatedAt: "",
      },
      {
        id: "2",
        name: "Building Blocks Set",
        description: "Creative building blocks for endless fun",
        shortDescription: "Creative building blocks",
        price: 39.99,
        imageUrls: ["/images/products/blocks-1.jpg"],
        category: fallbackCategories[1],
        brand: "ToyStore",
        ageRange: "3+",
        inStock: true,
        stockQuantity: 30,
        rating: 4.8,
        reviewCount: 95,
        tags: ["building", "creative"],
        createdAt: "",
        updatedAt: "",
      },
      {
        id: "3",
        name: "STEM Learning Kit",
        description: "Educational toy for science and math learning",
        shortDescription: "Educational STEM kit",
        price: 49.99,
        imageUrls: ["/images/products/stem-kit-1.jpg"],
        category: fallbackCategories[2],
        brand: "ToyStore",
        ageRange: "6+",
        inStock: true,
        stockQuantity: 25,
        rating: 4.7,
        reviewCount: 67,
        tags: ["educational", "STEM"],
        createdAt: "",
        updatedAt: "",
      },
      {
        id: "4",
        name: "Puzzle Adventure",
        description: "Challenging puzzle for problem-solving fun",
        shortDescription: "Challenging puzzle game",
        price: 19.99,
        imageUrls: ["/images/products/puzzle-1.jpg"],
        category: fallbackCategories[2],
        brand: "ToyStore",
        ageRange: "5+",
        inStock: true,
        stockQuantity: 40,
        rating: 4.6,
        reviewCount: 89,
        tags: ["puzzle", "brain"],
        createdAt: "",
        updatedAt: "",
      },
    ];

    // Set fallback data immediately for fast UI
    setCategories(fallbackCategories);
    setFeaturedProducts(fallbackProducts);
    setLoading(false);

    // Try to load real data in background
    try {
      const [productsData, categoriesData] = await Promise.all([
        ApiClient.getFeaturedProducts(),
        ApiClient.getCategories(),
      ]);

      setFeaturedProducts(productsData);
      setCategories(categoriesData);
    } catch (error) {
      console.warn("Backend not available, using fallback data:", error);
      // Fallback data is already set, so no need to do anything
    }
  };

  const heroSlides = [
    {
      title: "New STEM Toys Collection",
      subtitle: "Spark curiosity and learning with our latest educational toys",
      cta: "Shop STEM Toys",
      link: "/products?category=educational-toys",
      bgColor: "from-blue-600 to-purple-600",
      image: "/images/hero/stem-toys.jpg",
    },
    {
      title: "Action Heroes Assemble!",
      subtitle: "Discover the most exciting action figures and collectibles",
      cta: "Shop Action Figures",
      link: "/products?category=action-figures",
      bgColor: "from-red-600 to-orange-600",
      image: "/images/hero/action-figures.jpg",
    },
    {
      title: "Creative Building Adventures",
      subtitle: "Build, create, and imagine with our premium building sets",
      cta: "Shop Building Sets",
      link: "/products?category=building-blocks",
      bgColor: "from-green-600 to-teal-600",
      image: "/images/hero/building-blocks.jpg",
    },
  ];

  const features = [
    {
      icon: Truck,
      title: "Free Shipping",
      description: "Free delivery on orders over $50",
      color: "text-blue-600",
    },
    {
      icon: Shield,
      title: "Safe & Secure",
      description: "100% secure payment processing",
      color: "text-green-600",
    },
    {
      icon: RotateCcw,
      title: "Easy Returns",
      description: "30-day hassle-free returns",
      color: "text-purple-600",
    },
    {
      icon: Heart,
      title: "Customer Love",
      description: "Join thousands of happy families",
      color: "text-red-600",
    },
  ];

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
          <p className="text-gray-600">Loading awesome toys...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-white">
      <Header />

      {/* Hero Section */}
      <section className="relative overflow-hidden">
        <Carousel className="w-full">
          <CarouselContent>
            {heroSlides.map((slide, index) => (
              <CarouselItem key={index}>
                <div
                  className={`relative h-[500px] bg-gradient-to-r ${slide.bgColor} flex items-center`}
                >
                  <div className="container mx-auto px-4 py-16">
                    <div className="grid grid-cols-1 lg:grid-cols-2 gap-12 items-center">
                      <div className="text-white space-y-6">
                        <h1 className="text-5xl lg:text-6xl font-bold leading-tight">
                          {slide.title}
                        </h1>
                        <p className="text-xl lg:text-2xl opacity-90 leading-relaxed">
                          {slide.subtitle}
                        </p>
                        <div className="flex flex-col sm:flex-row gap-4">
                          <Button
                            asChild
                            size="lg"
                            className="bg-white text-gray-900 hover:bg-gray-100 font-semibold"
                          >
                            <Link to={slide.link}>
                              {slide.cta}
                              <ArrowRight className="ml-2 w-5 h-5" />
                            </Link>
                          </Button>
                          <Button
                            asChild
                            variant="outline"
                            size="lg"
                            className="border-white text-white hover:bg-white hover:text-gray-900"
                          >
                            <Link to="/products">View All Products</Link>
                          </Button>
                        </div>
                      </div>

                      {/* Hero image placeholder */}
                      <div className="hidden lg:block">
                        <div className="bg-white/10 rounded-2xl backdrop-blur-sm p-8 text-center text-white">
                          <Gift className="w-24 h-24 mx-auto mb-4 opacity-60" />
                          <p className="text-lg font-medium">Premium Toys</p>
                          <p className="text-sm opacity-80">
                            Quality Guaranteed
                          </p>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </CarouselItem>
            ))}
          </CarouselContent>
          <CarouselPrevious className="left-4" />
          <CarouselNext className="right-4" />
        </Carousel>
      </section>

      {/* Features Section */}
      <section className="py-16 bg-gray-50">
        <div className="container mx-auto px-4">
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8">
            {features.map((feature, index) => (
              <div
                key={index}
                className="text-center p-6 bg-white rounded-xl shadow-sm hover:shadow-md transition-shadow"
              >
                <feature.icon
                  className={`w-12 h-12 mx-auto mb-4 ${feature.color}`}
                />
                <h3 className="text-lg font-semibold text-gray-900 mb-2">
                  {feature.title}
                </h3>
                <p className="text-gray-600">{feature.description}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Categories Section */}
      <section className="py-16">
        <div className="container mx-auto px-4">
          <div className="text-center mb-12">
            <h2 className="text-4xl font-bold text-gray-900 mb-4">
              Shop by Category
            </h2>
            <p className="text-xl text-gray-600 max-w-2xl mx-auto">
              Discover amazing toys organized by what your child loves most
            </p>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            {categories.map((category) => (
              <Link
                key={category.id}
                to={`/products?category=${category.slug}`}
                className="group"
              >
                <Card className="overflow-hidden hover:shadow-xl transition-all duration-300 group-hover:scale-105">
                  <div className="aspect-video bg-gradient-to-br from-gray-100 to-gray-200 flex items-center justify-center">
                    <Gift className="w-16 h-16 text-gray-400" />
                  </div>
                  <CardContent className="p-6">
                    <h3 className="text-xl font-semibold text-gray-900 mb-2">
                      {category.name}
                    </h3>
                    <p className="text-gray-600 mb-4">{category.description}</p>
                    <div className="flex items-center text-blue-600 font-medium">
                      Shop Now
                      <ArrowRight className="ml-2 w-4 h-4 group-hover:translate-x-1 transition-transform" />
                    </div>
                  </CardContent>
                </Card>
              </Link>
            ))}
          </div>
        </div>
      </section>

      {/* Featured Products Section */}
      <section className="py-16 bg-gray-50">
        <div className="container mx-auto px-4">
          <div className="text-center mb-12">
            <h2 className="text-4xl font-bold text-gray-900 mb-4">
              Featured Products
            </h2>
            <p className="text-xl text-gray-600 max-w-2xl mx-auto">
              Hand-picked favorites that kids absolutely love
            </p>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
            {featuredProducts.map((product) => (
              <ProductCard key={product.id} product={product} />
            ))}
          </div>

          <div className="text-center mt-12">
            <Button asChild size="lg" className="bg-blue-600 hover:bg-blue-700">
              <Link to="/products">
                View All Products
                <ArrowRight className="ml-2 w-5 h-5" />
              </Link>
            </Button>
          </div>
        </div>
      </section>

      {/* Newsletter Section */}
      <section className="py-16 bg-blue-600">
        <div className="container mx-auto px-4 text-center">
          <div className="max-w-2xl mx-auto">
            <h2 className="text-3xl font-bold text-white mb-4">
              Stay Updated with New Arrivals
            </h2>
            <p className="text-blue-100 mb-8 text-lg">
              Get exclusive access to new products, special offers, and toy care
              tips
            </p>
            <div className="flex flex-col sm:flex-row gap-4 max-w-md mx-auto">
              <input
                type="email"
                placeholder="Enter your email"
                className="flex-1 px-4 py-3 rounded-lg border-0 focus:ring-2 focus:ring-blue-300"
              />
              <Button className="bg-white text-blue-600 hover:bg-gray-100 font-semibold px-8">
                Subscribe
              </Button>
            </div>
          </div>
        </div>
      </section>

      <Footer />
    </div>
  );
};

export default Index;
