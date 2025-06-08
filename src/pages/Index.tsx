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
      subtitle: "Complete your collection with the newest superhero figures",
      cta: "Explore Collection",
      link: "/products?category=action-figures",
      bgColor: "from-red-600 to-orange-600",
      image: "/images/hero/action-figures.jpg",
    },
    {
      title: "Building Dreams",
      subtitle: "Create, build, and imagine with our premium building sets",
      cta: "Start Building",
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
                        <div className="bg-white/10 rounded-2xl p-8 backdrop-blur-sm">
                          <div className="aspect-square bg-white/20 rounded-xl flex items-center justify-center">
                            <Gift className="w-32 h-32 text-white/60" />
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>

                  {/* Decorative elements */}
                  <div className="absolute top-0 right-0 w-1/3 h-full opacity-10">
                    <div className="w-full h-full bg-white rounded-l-full transform rotate-12"></div>
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
              <div key={index} className="text-center">
                <div
                  className={`inline-flex items-center justify-center w-16 h-16 rounded-full bg-white shadow-lg mb-4`}
                >
                  <feature.icon className={`w-8 h-8 ${feature.color}`} />
                </div>
                <h3 className="text-lg font-semibold mb-2">{feature.title}</h3>
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
            <h2 className="text-3xl lg:text-4xl font-bold text-gray-900 mb-4">
              Shop by Category
            </h2>
            <p className="text-xl text-gray-600 max-w-2xl mx-auto">
              Discover toys that inspire creativity, learning, and endless fun
              for every age group
            </p>
          </div>

          <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-6">
            {categories.map((category) => (
              <Link
                key={category.id}
                to={`/products?category=${category.slug}`}
                className="group"
              >
                <Card className="overflow-hidden hover:shadow-lg transition-all duration-300 hover:-translate-y-2">
                  <CardContent className="p-0">
                    <div className="aspect-square bg-gradient-to-br from-blue-50 to-purple-50 flex items-center justify-center relative overflow-hidden">
                      <div className="w-16 h-16 bg-gradient-to-br from-blue-500 to-purple-600 rounded-full flex items-center justify-center">
                        <Gift className="w-8 h-8 text-white" />
                      </div>

                      {/* Hover overlay */}
                      <div className="absolute inset-0 bg-blue-600/20 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center">
                        <ArrowRight className="w-6 h-6 text-blue-600" />
                      </div>
                    </div>

                    <div className="p-4 text-center">
                      <h3 className="font-semibold text-gray-900 group-hover:text-blue-600 transition-colors">
                        {category.name}
                      </h3>
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
          <div className="flex items-center justify-between mb-12">
            <div>
              <h2 className="text-3xl lg:text-4xl font-bold text-gray-900 mb-4">
                Featured Products
              </h2>
              <p className="text-xl text-gray-600">
                Hand-picked favorites that kids absolutely love
              </p>
            </div>
            <Button asChild variant="outline" size="lg">
              <Link to="/products">
                View All Products
                <ArrowRight className="ml-2 w-5 h-5" />
              </Link>
            </Button>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-8">
            {featuredProducts.map((product) => (
              <ProductCard key={product.id} product={product} />
            ))}
          </div>
        </div>
      </section>

      {/* Newsletter Section */}
      <section className="py-16 bg-gradient-to-r from-blue-600 to-purple-600">
        <div className="container mx-auto px-4 text-center">
          <div className="max-w-2xl mx-auto text-white">
            <h2 className="text-3xl lg:text-4xl font-bold mb-4">
              Stay in the Loop
            </h2>
            <p className="text-xl mb-8 opacity-90">
              Get the latest updates on new toys, special offers, and parenting
              tips
            </p>

            <div className="flex flex-col sm:flex-row gap-4 max-w-md mx-auto">
              <input
                type="email"
                placeholder="Enter your email"
                className="flex-1 px-4 py-3 rounded-lg border-0 text-gray-900 placeholder-gray-500"
              />
              <Button
                size="lg"
                className="bg-white text-blue-600 hover:bg-gray-100 font-semibold"
              >
                Subscribe
              </Button>
            </div>

            <p className="text-sm mt-4 opacity-75">
              We respect your privacy. Unsubscribe at any time.
            </p>
          </div>
        </div>
      </section>

      {/* Reviews Section */}
      <section className="py-16">
        <div className="container mx-auto px-4">
          <div className="text-center mb-12">
            <h2 className="text-3xl lg:text-4xl font-bold text-gray-900 mb-4">
              What Parents Say
            </h2>
            <p className="text-xl text-gray-600">
              Join thousands of happy families who trust ToyStore
            </p>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            {[
              {
                name: "Sarah Johnson",
                rating: 5,
                comment:
                  "Amazing quality toys! My kids absolutely love everything we've bought here. Fast shipping and great customer service.",
                product: "STEM Robotics Kit",
              },
              {
                name: "Mike Chen",
                rating: 5,
                comment:
                  "The building sets are fantastic. Great educational value and my son spends hours creating amazing structures.",
                product: "Ultimate Building Castle Set",
              },
              {
                name: "Emily Rodriguez",
                rating: 5,
                comment:
                  "Perfect for my daughter's birthday! The dollhouse exceeded expectations and the attention to detail is incredible.",
                product: "Fashion Doll Dream House",
              },
            ].map((review, index) => (
              <Card key={index} className="p-6">
                <CardContent className="p-0">
                  <div className="flex items-center mb-4">
                    {[...Array(review.rating)].map((_, i) => (
                      <Star
                        key={i}
                        className="w-5 h-5 text-yellow-400 fill-current"
                      />
                    ))}
                  </div>
                  <p className="text-gray-600 mb-4 italic">
                    "{review.comment}"
                  </p>
                  <div>
                    <p className="font-semibold text-gray-900">{review.name}</p>
                    <p className="text-sm text-gray-500">
                      Purchased: {review.product}
                    </p>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        </div>
      </section>

      <Footer />
    </div>
  );
};

export default Index;
