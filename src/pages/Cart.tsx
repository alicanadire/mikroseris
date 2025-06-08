import { useState, useEffect } from "react";
import { Link, useNavigate } from "react-router-dom";
import {
  Minus,
  Plus,
  Trash2,
  ShoppingBag,
  ArrowLeft,
  Heart,
} from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Card,
  CardContent,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Separator } from "@/components/ui/separator";
import { toast } from "@/hooks/use-toast";
import Header from "@/components/Header";
import Footer from "@/components/Footer";
import { Cart as CartType, CartItem } from "@/types";
import ApiClient from "@/lib/api";

const Cart = () => {
  const [cart, setCart] = useState<CartType | null>(null);
  const [loading, setLoading] = useState(true);
  const [updatingItems, setUpdatingItems] = useState<Set<string>>(new Set());
  const navigate = useNavigate();

  useEffect(() => {
    loadCart();
  }, []);

  const loadCart = async () => {
    try {
      const cartData = await ApiClient.getCart();
      setCart(cartData);
    } catch (error) {
      console.error("Failed to load cart:", error);
      toast({
        title: "Error",
        description: "Failed to load cart. Please try again.",
        variant: "destructive",
      });
    } finally {
      setLoading(false);
    }
  };

  const updateQuantity = async (itemId: string, newQuantity: number) => {
    if (newQuantity < 0) return;

    setUpdatingItems((prev) => new Set(prev).add(itemId));

    try {
      const updatedCart = await ApiClient.updateCartItem(itemId, newQuantity);
      setCart(updatedCart);

      if (newQuantity === 0) {
        toast({
          title: "Item Removed",
          description: "Item has been removed from your cart.",
        });
      }
    } catch (error) {
      console.error("Failed to update cart item:", error);
      toast({
        title: "Error",
        description: "Failed to update item quantity. Please try again.",
        variant: "destructive",
      });
    } finally {
      setUpdatingItems((prev) => {
        const newSet = new Set(prev);
        newSet.delete(itemId);
        return newSet;
      });
    }
  };

  const removeItem = async (itemId: string) => {
    await updateQuantity(itemId, 0);
  };

  const clearCart = async () => {
    try {
      const clearedCart = await ApiClient.clearCart();
      setCart(clearedCart);
      toast({
        title: "Cart Cleared",
        description: "All items have been removed from your cart.",
      });
    } catch (error) {
      console.error("Failed to clear cart:", error);
      toast({
        title: "Error",
        description: "Failed to clear cart. Please try again.",
        variant: "destructive",
      });
    }
  };

  const handleCheckout = () => {
    if (!cart || cart.items.length === 0) return;
    navigate("/checkout");
  };

  const calculateSubtotal = () => {
    return (
      cart?.items.reduce((sum, item) => sum + item.price * item.quantity, 0) ||
      0
    );
  };

  const calculateShipping = () => {
    const subtotal = calculateSubtotal();
    return subtotal >= 50 ? 0 : 9.99;
  };

  const calculateTax = () => {
    const subtotal = calculateSubtotal();
    return subtotal * 0.08; // 8% tax
  };

  const calculateTotal = () => {
    return calculateSubtotal() + calculateShipping() + calculateTax();
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-white">
        <Header />
        <div className="container mx-auto px-4 py-8">
          <div className="animate-pulse">
            <div className="h-8 bg-gray-200 rounded w-48 mb-8"></div>
            <div className="space-y-4">
              {[...Array(3)].map((_, i) => (
                <div key={i} className="h-32 bg-gray-200 rounded"></div>
              ))}
            </div>
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
          <div className="flex items-center gap-4">
            <Button variant="ghost" size="sm" onClick={() => navigate(-1)}>
              <ArrowLeft className="w-4 h-4 mr-2" />
              Continue Shopping
            </Button>
            <div>
              <h1 className="text-3xl font-bold text-gray-900">
                Shopping Cart
              </h1>
              <p className="text-gray-600">
                {cart?.totalItems || 0} item{cart?.totalItems !== 1 ? "s" : ""}{" "}
                in your cart
              </p>
            </div>
          </div>

          {cart && cart.items.length > 0 && (
            <Button
              variant="outline"
              onClick={clearCart}
              className="text-red-600 hover:text-red-700"
            >
              <Trash2 className="w-4 h-4 mr-2" />
              Clear Cart
            </Button>
          )}
        </div>

        {!cart || cart.items.length === 0 ? (
          /* Empty Cart */
          <div className="text-center py-16">
            <div className="mb-6">
              <ShoppingBag className="w-24 h-24 text-gray-400 mx-auto" />
            </div>
            <h2 className="text-2xl font-semibold text-gray-900 mb-4">
              Your cart is empty
            </h2>
            <p className="text-gray-600 mb-8 max-w-md mx-auto">
              Looks like you haven't added any toys to your cart yet. Start
              exploring our amazing collection!
            </p>
            <Button asChild size="lg">
              <Link to="/products">Start Shopping</Link>
            </Button>
          </div>
        ) : (
          /* Cart with Items */
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
            {/* Cart Items */}
            <div className="lg:col-span-2 space-y-4">
              {cart.items.map((item: CartItem) => (
                <Card key={item.id} className="overflow-hidden">
                  <CardContent className="p-0">
                    <div className="flex flex-col sm:flex-row">
                      {/* Product Image */}
                      <div className="w-full sm:w-32 h-32 bg-gray-100 flex items-center justify-center">
                        {item.product.imageUrls &&
                        item.product.imageUrls.length > 0 ? (
                          <img
                            src={item.product.imageUrls[0]}
                            alt={item.product.name}
                            className="w-full h-full object-cover"
                            onError={(e) => {
                              const target = e.target as HTMLImageElement;
                              target.style.display = "none";
                              target.nextElementSibling?.classList.remove(
                                "hidden",
                              );
                            }}
                          />
                        ) : null}
                        <div
                          className={`flex items-center justify-center ${item.product.imageUrls?.length ? "hidden" : ""}`}
                        >
                          <ShoppingBag className="w-8 h-8 text-gray-400" />
                        </div>
                      </div>

                      {/* Product Details */}
                      <div className="flex-1 p-6">
                        <div className="flex flex-col sm:flex-row justify-between">
                          <div className="flex-1 mb-4 sm:mb-0">
                            <Link
                              to={`/products/${item.product.id}`}
                              className="text-lg font-semibold text-gray-900 hover:text-blue-600 transition-colors"
                            >
                              {item.product.name}
                            </Link>

                            <div className="flex items-center gap-2 mt-2">
                              <Badge variant="secondary" className="text-xs">
                                {item.product.category.name}
                              </Badge>
                              <span className="text-sm text-gray-500">
                                Ages {item.product.ageRange}
                              </span>
                            </div>

                            <p className="text-sm text-gray-600 mt-2 line-clamp-2">
                              {item.product.shortDescription}
                            </p>
                          </div>

                          <div className="flex flex-col items-end space-y-4">
                            {/* Price */}
                            <div className="text-right">
                              <div className="text-lg font-semibold text-gray-900">
                                ${(item.price * item.quantity).toFixed(2)}
                              </div>
                              <div className="text-sm text-gray-500">
                                ${item.price.toFixed(2)} each
                              </div>
                            </div>

                            {/* Quantity Controls */}
                            <div className="flex items-center space-x-2">
                              <Button
                                variant="outline"
                                size="icon"
                                className="h-8 w-8"
                                onClick={() =>
                                  updateQuantity(item.id, item.quantity - 1)
                                }
                                disabled={
                                  updatingItems.has(item.id) ||
                                  item.quantity <= 1
                                }
                              >
                                <Minus className="w-3 h-3" />
                              </Button>

                              <Input
                                type="number"
                                min="1"
                                value={item.quantity}
                                onChange={(e) => {
                                  const value = parseInt(e.target.value);
                                  if (value > 0) {
                                    updateQuantity(item.id, value);
                                  }
                                }}
                                className="w-16 h-8 text-center"
                                disabled={updatingItems.has(item.id)}
                              />

                              <Button
                                variant="outline"
                                size="icon"
                                className="h-8 w-8"
                                onClick={() =>
                                  updateQuantity(item.id, item.quantity + 1)
                                }
                                disabled={updatingItems.has(item.id)}
                              >
                                <Plus className="w-3 h-3" />
                              </Button>
                            </div>

                            {/* Action Buttons */}
                            <div className="flex space-x-2">
                              <Button
                                variant="ghost"
                                size="sm"
                                className="text-gray-500 hover:text-red-600"
                                onClick={() => removeItem(item.id)}
                                disabled={updatingItems.has(item.id)}
                              >
                                <Trash2 className="w-4 h-4" />
                              </Button>
                              <Button
                                variant="ghost"
                                size="sm"
                                className="text-gray-500 hover:text-red-600"
                              >
                                <Heart className="w-4 h-4" />
                              </Button>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </CardContent>
                </Card>
              ))}
            </div>

            {/* Order Summary */}
            <div className="lg:col-span-1">
              <Card className="sticky top-24">
                <CardHeader>
                  <CardTitle>Order Summary</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div className="flex justify-between">
                    <span>Subtotal ({cart.totalItems} items)</span>
                    <span>${calculateSubtotal().toFixed(2)}</span>
                  </div>

                  <div className="flex justify-between">
                    <span>Shipping</span>
                    <span
                      className={
                        calculateShipping() === 0
                          ? "text-green-600 font-medium"
                          : ""
                      }
                    >
                      {calculateShipping() === 0
                        ? "FREE"
                        : `$${calculateShipping().toFixed(2)}`}
                    </span>
                  </div>

                  {calculateShipping() > 0 && (
                    <div className="text-sm text-gray-600 bg-blue-50 p-3 rounded-lg">
                      Add ${(50 - calculateSubtotal()).toFixed(2)} more for free
                      shipping!
                    </div>
                  )}

                  <div className="flex justify-between">
                    <span>Tax</span>
                    <span>${calculateTax().toFixed(2)}</span>
                  </div>

                  <Separator />

                  <div className="flex justify-between text-lg font-semibold">
                    <span>Total</span>
                    <span>${calculateTotal().toFixed(2)}</span>
                  </div>
                </CardContent>
                <CardFooter className="flex flex-col space-y-3">
                  <Button onClick={handleCheckout} className="w-full" size="lg">
                    Proceed to Checkout
                  </Button>
                  <Button variant="outline" asChild className="w-full">
                    <Link to="/products">Continue Shopping</Link>
                  </Button>
                </CardFooter>
              </Card>
            </div>
          </div>
        )}
      </div>

      <Footer />
    </div>
  );
};

export default Cart;
