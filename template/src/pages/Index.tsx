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
  ShoppingCart,
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

const Index = () => {
  const [featuredProducts, setFeaturedProducts] = useState([
    {
      id: "1",
      name: "Super Hero Action Figure",
      description: "Amazing superhero figure with movable joints",
      price: 24.99,
      imageUrls: ["/images/products/action-figure-1.jpg"],
      category: { name: "Action Figures" },
      brand: "ToyStore",
      ageRange: "4+",
      rating: 4.5,
      reviewCount: 128,
    },
    {
      id: "2",
      name: "Building Blocks Set",
      description: "Creative building blocks for endless fun",
      price: 39.99,
      imageUrls: ["/images/products/blocks-1.jpg"],
      category: { name: "Building Blocks" },
      brand: "ToyStore",
      ageRange: "3+",
      rating: 4.8,
      reviewCount: 95,
    },
    {
      id: "3",
      name: "STEM Learning Kit",
      description: "Educational toy for science and math learning",
      price: 49.99,
      imageUrls: ["/images/products/stem-kit-1.jpg"],
      category: { name: "Educational" },
      brand: "ToyStore",
      ageRange: "6+",
      rating: 4.7,
      reviewCount: 67,
    },
    {
      id: "4",
      name: "Puzzle Adventure",
      description: "Challenging puzzle for problem-solving fun",
      price: 19.99,
      imageUrls: ["/images/products/puzzle-1.jpg"],
      category: { name: "Educational" },
      brand: "ToyStore",
      ageRange: "5+",
      rating: 4.6,
      reviewCount: 89,
    },
  ]);

  const categories = [
    {
      id: "1",
      name: "Action Figures",
      description: "Superhero and character action figures",
      slug: "action-figures",
    },
    {
      id: "2",
      name: "Building Blocks",
      description: "LEGO and other building sets",
      slug: "building-blocks",
    },
    {
      id: "3",
      name: "Educational Toys",
      description: "Learning and STEM toys",
      slug: "educational-toys",
    },
  ];

  const heroSlides = [
    {
      title: "Welcome to ToyStore!",
      subtitle: "Discover amazing toys that inspire creativity and learning",
      cta: "Shop Now",
      link: "/products",
      bgColor: "from-blue-600 to-purple-600",
    },
    {
      title: "Action Heroes Assemble!",
      subtitle: "Discover the most exciting action figures and collectibles",
      cta: "Shop Action Figures",
      link: "/products?category=action-figures",
      bgColor: "from-red-600 to-orange-600",
    },
    {
      title: "Creative Building Adventures",
      subtitle: "Build, create, and imagine with our premium building sets",
      cta: "Shop Building Sets",
      link: "/products?category=building-blocks",
      bgColor: "from-green-600 to-teal-600",
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
              <Card
                key={product.id}
                className="group hover:shadow-lg transition-shadow"
              >
                <div className="aspect-square bg-gradient-to-br from-gray-100 to-gray-200 flex items-center justify-center">
                  <Gift className="w-20 h-20 text-gray-400" />
                </div>
                <CardContent className="p-4">
                  <div className="flex items-center gap-2 mb-2">
                    <Badge variant="secondary" className="text-xs">
                      {product.category.name}
                    </Badge>
                    <span className="text-xs text-gray-500">
                      {product.ageRange}
                    </span>
                  </div>

                  <h3 className="font-semibold text-gray-900 mb-2 line-clamp-2">
                    {product.name}
                  </h3>

                  <p className="text-sm text-gray-600 mb-3 line-clamp-2">
                    {product.description}
                  </p>

                  <div className="flex items-center gap-1 mb-3">
                    <div className="flex">
                      {[...Array(5)].map((_, i) => (
                        <Star
                          key={i}
                          className={`w-4 h-4 ${
                            i < Math.floor(product.rating)
                              ? "text-yellow-400 fill-current"
                              : "text-gray-300"
                          }`}
                        />
                      ))}
                    </div>
                    <span className="text-sm text-gray-600">
                      {product.rating} ({product.reviewCount})
                    </span>
                  </div>

                  <div className="flex items-center justify-between">
                    <span className="text-lg font-bold text-gray-900">
                      ${product.price}
                    </span>
                    <Button size="sm" className="flex items-center gap-1">
                      <ShoppingCart className="w-4 h-4" />
                      Add to Cart
                    </Button>
                  </div>
                </CardContent>
              </Card>
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
