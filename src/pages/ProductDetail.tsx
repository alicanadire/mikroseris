import { useState, useEffect } from "react";
import { useParams, useNavigate, Link } from "react-router-dom";
import {
  ShoppingCart,
  Heart,
  Star,
  Minus,
  Plus,
  Share2,
  ArrowLeft,
  Package,
  Truck,
  Shield,
  RotateCcw,
  ChevronLeft,
  ChevronRight,
} from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Separator } from "@/components/ui/separator";
import { toast } from "@/hooks/use-toast";
import Header from "@/components/Header";
import Footer from "@/components/Footer";
import ProductCard from "@/components/ProductCard";
import { Product, Review } from "@/types";
import ApiClient from "@/lib/api";

const ProductDetail = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [product, setProduct] = useState<Product | null>(null);
  const [relatedProducts, setRelatedProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [quantity, setQuantity] = useState(1);
  const [selectedImageIndex, setSelectedImageIndex] = useState(0);
  const [isAddingToCart, setIsAddingToCart] = useState(false);
  const [isInWishlist, setIsInWishlist] = useState(false);

  useEffect(() => {
    if (id) {
      loadProduct(id);
    }
  }, [id]);

  const loadProduct = async (productId: string) => {
    setLoading(true);
    try {
      const productData = await ApiClient.getProduct(productId);
      if (productData) {
        setProduct(productData);
        // Load related products from the same category
        const relatedData = await ApiClient.getProducts({
          category: productData.category.slug,
        });
        setRelatedProducts(
          relatedData.data.filter((p) => p.id !== productId).slice(0, 4),
        );
      } else {
        navigate("/404");
      }
    } catch (error) {
      console.error("Failed to load product:", error);
      toast({
        title: "Error",
        description: "Failed to load product details.",
        variant: "destructive",
      });
    } finally {
      setLoading(false);
    }
  };

  const handleAddToCart = async () => {
    if (!product || !product.inStock) return;

    setIsAddingToCart(true);
    try {
      await ApiClient.addToCart(product.id, quantity);
      toast({
        title: "Added to Cart",
        description: `${quantity} ${product.name} added to your cart.`,
      });
    } catch (error) {
      toast({
        title: "Error",
        description: "Failed to add item to cart.",
        variant: "destructive",
      });
    } finally {
      setIsAddingToCart(false);
    }
  };

  const handleWishlistToggle = () => {
    setIsInWishlist(!isInWishlist);
    toast({
      title: isInWishlist ? "Removed from Wishlist" : "Added to Wishlist",
      description: isInWishlist
        ? `${product?.name} removed from wishlist.`
        : `${product?.name} added to wishlist.`,
    });
  };

  const handleShare = () => {
    if (navigator.share && product) {
      navigator.share({
        title: product.name,
        text: product.shortDescription,
        url: window.location.href,
      });
    } else {
      navigator.clipboard.writeText(window.location.href);
      toast({
        title: "Link Copied",
        description: "Product link copied to clipboard.",
      });
    }
  };

  const renderStars = (rating: number) => {
    return [...Array(5)].map((_, i) => (
      <Star
        key={i}
        className={`w-4 h-4 ${
          i < Math.floor(rating)
            ? "text-yellow-400 fill-current"
            : i < rating
              ? "text-yellow-400 fill-current opacity-50"
              : "text-gray-300"
        }`}
      />
    ));
  };

  const mockReviews: Review[] = [
    {
      id: "1",
      productId: product?.id || "",
      userId: "user1",
      userName: "Sarah Johnson",
      rating: 5,
      title: "Amazing quality!",
      comment:
        "My kids absolutely love this toy. Great build quality and arrived quickly.",
      createdAt: "2024-01-15T10:00:00Z",
      verified: true,
    },
    {
      id: "2",
      productId: product?.id || "",
      userId: "user2",
      userName: "Mike Chen",
      rating: 4,
      title: "Good value for money",
      comment:
        "Solid toy with good educational value. Recommended for the age group.",
      createdAt: "2024-01-10T14:30:00Z",
      verified: true,
    },
  ];

  if (loading) {
    return (
      <div className="min-h-screen bg-white">
        <Header />
        <div className="container mx-auto px-4 py-8">
          <div className="animate-pulse">
            <div className="h-8 bg-gray-200 rounded w-48 mb-8"></div>
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
              <div className="h-96 bg-gray-200 rounded"></div>
              <div className="space-y-4">
                <div className="h-8 bg-gray-200 rounded"></div>
                <div className="h-4 bg-gray-200 rounded w-3/4"></div>
                <div className="h-6 bg-gray-200 rounded w-1/2"></div>
              </div>
            </div>
          </div>
        </div>
        <Footer />
      </div>
    );
  }

  if (!product) {
    return (
      <div className="min-h-screen bg-white">
        <Header />
        <div className="container mx-auto px-4 py-8 text-center">
          <h1 className="text-2xl font-bold mb-4">Product Not Found</h1>
          <Button onClick={() => navigate("/products")}>Browse Products</Button>
        </div>
        <Footer />
      </div>
    );
  }

  const discountPercentage = product.originalPrice
    ? Math.round(
        ((product.originalPrice - product.price) / product.originalPrice) * 100,
      )
    : 0;

  return (
    <div className="min-h-screen bg-white">
      <Header />

      <div className="container mx-auto px-4 py-8">
        {/* Breadcrumb */}
        <nav className="flex items-center space-x-2 text-sm text-gray-600 mb-8">
          <Link to="/" className="hover:text-blue-600">
            Home
          </Link>
          <span>/</span>
          <Link to="/products" className="hover:text-blue-600">
            Products
          </Link>
          <span>/</span>
          <Link
            to={`/products?category=${product.category.slug}`}
            className="hover:text-blue-600"
          >
            {product.category.name}
          </Link>
          <span>/</span>
          <span className="text-gray-900">{product.name}</span>
        </nav>

        {/* Back Button */}
        <Button variant="ghost" onClick={() => navigate(-1)} className="mb-6">
          <ArrowLeft className="w-4 h-4 mr-2" />
          Back
        </Button>

        {/* Product Content */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-12 mb-16">
          {/* Product Images */}
          <div className="space-y-4">
            {/* Main Image */}
            <div className="aspect-square bg-gray-100 rounded-lg overflow-hidden relative">
              {product.imageUrls && product.imageUrls.length > 0 ? (
                <img
                  src={product.imageUrls[selectedImageIndex]}
                  alt={product.name}
                  className="w-full h-full object-cover"
                  onError={(e) => {
                    const target = e.target as HTMLImageElement;
                    target.style.display = "none";
                    target.nextElementSibling?.classList.remove("hidden");
                  }}
                />
              ) : null}

              <div
                className={`absolute inset-0 flex items-center justify-center bg-gray-100 ${product.imageUrls?.length ? "hidden" : ""}`}
              >
                <Package className="w-24 h-24 text-gray-400" />
              </div>

              {/* Image Navigation */}
              {product.imageUrls && product.imageUrls.length > 1 && (
                <>
                  <Button
                    variant="ghost"
                    size="icon"
                    className="absolute left-2 top-1/2 transform -translate-y-1/2 bg-white/80 hover:bg-white"
                    onClick={() =>
                      setSelectedImageIndex(
                        selectedImageIndex > 0
                          ? selectedImageIndex - 1
                          : product.imageUrls.length - 1,
                      )
                    }
                  >
                    <ChevronLeft className="w-4 h-4" />
                  </Button>
                  <Button
                    variant="ghost"
                    size="icon"
                    className="absolute right-2 top-1/2 transform -translate-y-1/2 bg-white/80 hover:bg-white"
                    onClick={() =>
                      setSelectedImageIndex(
                        selectedImageIndex < product.imageUrls.length - 1
                          ? selectedImageIndex + 1
                          : 0,
                      )
                    }
                  >
                    <ChevronRight className="w-4 h-4" />
                  </Button>
                </>
              )}

              {/* Badges */}
              <div className="absolute top-4 left-4 flex flex-col gap-2">
                {!product.inStock && (
                  <Badge variant="destructive">Out of Stock</Badge>
                )}
                {discountPercentage > 0 && (
                  <Badge variant="destructive">-{discountPercentage}%</Badge>
                )}
              </div>
            </div>

            {/* Thumbnail Images */}
            {product.imageUrls && product.imageUrls.length > 1 && (
              <div className="flex space-x-2 overflow-x-auto">
                {product.imageUrls.map((imageUrl, index) => (
                  <button
                    key={index}
                    onClick={() => setSelectedImageIndex(index)}
                    className={`flex-shrink-0 w-20 h-20 bg-gray-100 rounded-lg overflow-hidden border-2 ${
                      selectedImageIndex === index
                        ? "border-blue-600"
                        : "border-transparent"
                    }`}
                  >
                    <img
                      src={imageUrl}
                      alt={`${product.name} ${index + 1}`}
                      className="w-full h-full object-cover"
                    />
                  </button>
                ))}
              </div>
            )}
          </div>

          {/* Product Info */}
          <div className="space-y-6">
            {/* Category & Brand */}
            <div className="flex items-center justify-between">
              <Badge variant="secondary">{product.category.name}</Badge>
              <div className="flex space-x-2">
                <Button
                  variant="ghost"
                  size="icon"
                  onClick={handleWishlistToggle}
                >
                  <Heart
                    className={`w-5 h-5 ${isInWishlist ? "text-red-500 fill-current" : "text-gray-400"}`}
                  />
                </Button>
                <Button variant="ghost" size="icon" onClick={handleShare}>
                  <Share2 className="w-5 h-5" />
                </Button>
              </div>
            </div>

            {/* Product Name */}
            <h1 className="text-3xl lg:text-4xl font-bold text-gray-900">
              {product.name}
            </h1>

            {/* Brand & Age */}
            <div className="flex items-center space-x-4 text-gray-600">
              <span className="font-medium">Brand: {product.brand}</span>
              <span>•</span>
              <span>Ages {product.ageRange}</span>
            </div>

            {/* Rating */}
            <div className="flex items-center space-x-2">
              <div className="flex items-center">
                {renderStars(product.rating)}
              </div>
              <span className="text-lg font-medium">{product.rating}</span>
              <span className="text-gray-600">
                ({product.reviewCount} reviews)
              </span>
            </div>

            {/* Price */}
            <div className="flex items-center space-x-4">
              <span className="text-3xl font-bold text-gray-900">
                ${product.price.toFixed(2)}
              </span>
              {product.originalPrice && (
                <span className="text-xl text-gray-500 line-through">
                  ${product.originalPrice.toFixed(2)}
                </span>
              )}
              {discountPercentage > 0 && (
                <Badge variant="destructive" className="text-sm">
                  Save {discountPercentage}%
                </Badge>
              )}
            </div>

            {/* Short Description */}
            <p className="text-lg text-gray-600 leading-relaxed">
              {product.shortDescription}
            </p>

            {/* Stock Status */}
            <div className="flex items-center space-x-2">
              <div
                className={`w-3 h-3 rounded-full ${product.inStock ? "bg-green-500" : "bg-red-500"}`}
              ></div>
              <span
                className={`font-medium ${product.inStock ? "text-green-600" : "text-red-600"}`}
              >
                {product.inStock
                  ? `In Stock (${product.stockQuantity} available)`
                  : "Out of Stock"}
              </span>
            </div>

            {/* Quantity & Add to Cart */}
            {product.inStock && (
              <div className="flex items-center space-x-4">
                <div className="flex items-center border rounded-lg">
                  <Button
                    variant="ghost"
                    size="icon"
                    onClick={() => setQuantity(Math.max(1, quantity - 1))}
                    disabled={quantity <= 1}
                  >
                    <Minus className="w-4 h-4" />
                  </Button>
                  <Input
                    type="number"
                    min="1"
                    max={product.stockQuantity}
                    value={quantity}
                    onChange={(e) =>
                      setQuantity(Math.max(1, parseInt(e.target.value) || 1))
                    }
                    className="w-20 border-0 text-center focus-visible:ring-0"
                  />
                  <Button
                    variant="ghost"
                    size="icon"
                    onClick={() =>
                      setQuantity(Math.min(product.stockQuantity, quantity + 1))
                    }
                    disabled={quantity >= product.stockQuantity}
                  >
                    <Plus className="w-4 h-4" />
                  </Button>
                </div>

                <Button
                  onClick={handleAddToCart}
                  disabled={isAddingToCart}
                  size="lg"
                  className="flex-1"
                >
                  {isAddingToCart ? (
                    <>
                      <div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin mr-2" />
                      Adding...
                    </>
                  ) : (
                    <>
                      <ShoppingCart className="w-5 h-5 mr-2" />
                      Add to Cart
                    </>
                  )}
                </Button>
              </div>
            )}

            {/* Features */}
            <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 pt-6 border-t">
              <div className="flex items-center space-x-2 text-sm text-gray-600">
                <Truck className="w-4 h-4" />
                <span>Free shipping over $50</span>
              </div>
              <div className="flex items-center space-x-2 text-sm text-gray-600">
                <Shield className="w-4 h-4" />
                <span>Safe & secure checkout</span>
              </div>
              <div className="flex items-center space-x-2 text-sm text-gray-600">
                <RotateCcw className="w-4 h-4" />
                <span>30-day returns</span>
              </div>
            </div>

            {/* Tags */}
            {product.tags.length > 0 && (
              <div className="flex flex-wrap gap-2">
                {product.tags.map((tag, index) => (
                  <Badge key={index} variant="outline" className="text-xs">
                    {tag}
                  </Badge>
                ))}
              </div>
            )}
          </div>
        </div>

        {/* Product Details Tabs */}
        <Tabs defaultValue="description" className="mb-16">
          <TabsList className="grid w-full grid-cols-3">
            <TabsTrigger value="description">Description</TabsTrigger>
            <TabsTrigger value="specifications">Specifications</TabsTrigger>
            <TabsTrigger value="reviews">Reviews</TabsTrigger>
          </TabsList>

          <TabsContent value="description" className="mt-6">
            <Card>
              <CardContent className="p-6">
                <div className="prose max-w-none">
                  <p className="text-gray-700 leading-relaxed text-lg">
                    {product.description}
                  </p>
                </div>
              </CardContent>
            </Card>
          </TabsContent>

          <TabsContent value="specifications" className="mt-6">
            <Card>
              <CardContent className="p-6">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <div>
                    <h3 className="font-semibold mb-3">Product Details</h3>
                    <dl className="space-y-2">
                      <div className="flex justify-between">
                        <dt className="text-gray-600">Brand:</dt>
                        <dd className="font-medium">{product.brand}</dd>
                      </div>
                      <div className="flex justify-between">
                        <dt className="text-gray-600">Age Range:</dt>
                        <dd className="font-medium">{product.ageRange}</dd>
                      </div>
                      <div className="flex justify-between">
                        <dt className="text-gray-600">Category:</dt>
                        <dd className="font-medium">{product.category.name}</dd>
                      </div>
                      <div className="flex justify-between">
                        <dt className="text-gray-600">Stock:</dt>
                        <dd className="font-medium">
                          {product.stockQuantity} units
                        </dd>
                      </div>
                    </dl>
                  </div>
                  <div>
                    <h3 className="font-semibold mb-3">Ratings</h3>
                    <dl className="space-y-2">
                      <div className="flex justify-between">
                        <dt className="text-gray-600">Average Rating:</dt>
                        <dd className="font-medium">{product.rating}/5</dd>
                      </div>
                      <div className="flex justify-between">
                        <dt className="text-gray-600">Total Reviews:</dt>
                        <dd className="font-medium">{product.reviewCount}</dd>
                      </div>
                    </dl>
                  </div>
                </div>
              </CardContent>
            </Card>
          </TabsContent>

          <TabsContent value="reviews" className="mt-6">
            <div className="space-y-6">
              {mockReviews.map((review) => (
                <Card key={review.id}>
                  <CardContent className="p-6">
                    <div className="flex items-start justify-between mb-4">
                      <div>
                        <div className="flex items-center space-x-2 mb-2">
                          <span className="font-semibold">
                            {review.userName}
                          </span>
                          {review.verified && (
                            <Badge variant="secondary" className="text-xs">
                              Verified Purchase
                            </Badge>
                          )}
                        </div>
                        <div className="flex items-center space-x-2">
                          <div className="flex">
                            {renderStars(review.rating)}
                          </div>
                          <span className="text-sm text-gray-600">
                            {new Date(review.createdAt).toLocaleDateString()}
                          </span>
                        </div>
                      </div>
                    </div>
                    <h4 className="font-semibold mb-2">{review.title}</h4>
                    <p className="text-gray-700">{review.comment}</p>
                  </CardContent>
                </Card>
              ))}
            </div>
          </TabsContent>
        </Tabs>

        {/* Related Products */}
        {relatedProducts.length > 0 && (
          <section>
            <div className="flex items-center justify-between mb-8">
              <h2 className="text-2xl font-bold text-gray-900">
                Related Products
              </h2>
              <Button asChild variant="outline">
                <Link to={`/products?category=${product.category.slug}`}>
                  View All in {product.category.name}
                </Link>
              </Button>
            </div>

            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
              {relatedProducts.map((relatedProduct) => (
                <ProductCard key={relatedProduct.id} product={relatedProduct} />
              ))}
            </div>
          </section>
        )}
      </div>

      <Footer />
    </div>
  );
};

export default ProductDetail;
