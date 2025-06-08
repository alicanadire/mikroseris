import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import {
  CreditCard,
  Truck,
  Shield,
  MapPin,
  User,
  Mail,
  Phone,
  Lock,
  CheckCircle,
} from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Checkbox } from "@/components/ui/checkbox";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Separator } from "@/components/ui/separator";
import { Badge } from "@/components/ui/badge";
import { toast } from "@/hooks/use-toast";
import Header from "@/components/Header";
import Footer from "@/components/Footer";
import { Cart, Address } from "@/types";
import ApiClient from "@/lib/api";

const Checkout = () => {
  const navigate = useNavigate();
  const [cart, setCart] = useState<Cart | null>(null);
  const [loading, setLoading] = useState(true);
  const [isPlacingOrder, setIsPlacingOrder] = useState(false);
  const [currentStep, setCurrentStep] = useState(1);

  const [shippingAddress, setShippingAddress] = useState<Address>({
    firstName: "",
    lastName: "",
    company: "",
    street: "",
    city: "",
    state: "",
    zipCode: "",
    country: "United States",
    phone: "",
  });

  const [billingAddress, setBillingAddress] = useState<Address>({
    firstName: "",
    lastName: "",
    company: "",
    street: "",
    city: "",
    state: "",
    zipCode: "",
    country: "United States",
    phone: "",
  });

  const [paymentInfo, setPaymentInfo] = useState({
    cardNumber: "",
    expiryMonth: "",
    expiryYear: "",
    cvv: "",
    cardholderName: "",
    billingAddressSame: true,
  });

  const [contactInfo, setContactInfo] = useState({
    email: "",
    phone: "",
    newsletter: false,
  });

  useEffect(() => {
    loadCart();
  }, []);

  const loadCart = async () => {
    try {
      const cartData = await ApiClient.getCart();
      setCart(cartData);

      if (!cartData || cartData.items.length === 0) {
        toast({
          title: "Empty Cart",
          description: "Your cart is empty. Please add items before checkout.",
          variant: "destructive",
        });
        navigate("/cart");
      }
    } catch (error) {
      console.error("Failed to load cart:", error);
      toast({
        title: "Error",
        description: "Failed to load cart data.",
        variant: "destructive",
      });
    } finally {
      setLoading(false);
    }
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

  const validateStep = (step: number): boolean => {
    switch (step) {
      case 1:
        return (
          contactInfo.email &&
          shippingAddress.firstName &&
          shippingAddress.lastName &&
          shippingAddress.street &&
          shippingAddress.city &&
          shippingAddress.state &&
          shippingAddress.zipCode &&
          shippingAddress.phone
        );
      case 2:
        return (
          paymentInfo.cardNumber &&
          paymentInfo.expiryMonth &&
          paymentInfo.expiryYear &&
          paymentInfo.cvv &&
          paymentInfo.cardholderName
        );
      default:
        return true;
    }
  };

  const handleNextStep = () => {
    if (validateStep(currentStep)) {
      setCurrentStep(currentStep + 1);
    } else {
      toast({
        title: "Incomplete Information",
        description: "Please fill in all required fields.",
        variant: "destructive",
      });
    }
  };

  const handlePlaceOrder = async () => {
    if (!validateStep(2)) {
      toast({
        title: "Incomplete Information",
        description: "Please fill in all required fields.",
        variant: "destructive",
      });
      return;
    }

    setIsPlacingOrder(true);
    try {
      // Simulate order placement
      await new Promise((resolve) => setTimeout(resolve, 2000));

      // Clear cart after successful order
      await ApiClient.clearCart();

      toast({
        title: "Order Placed Successfully!",
        description:
          "Thank you for your order. You will receive a confirmation email shortly.",
      });

      navigate("/order-confirmation", {
        state: {
          orderNumber: "ORD-" + Date.now(),
          total: calculateTotal(),
        },
      });
    } catch (error) {
      toast({
        title: "Order Failed",
        description:
          "There was an error processing your order. Please try again.",
        variant: "destructive",
      });
    } finally {
      setIsPlacingOrder(false);
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-white">
        <Header />
        <div className="container mx-auto px-4 py-8">
          <div className="animate-pulse">
            <div className="h-8 bg-gray-200 rounded w-48 mb-8"></div>
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
              <div className="lg:col-span-2 space-y-6">
                <div className="h-96 bg-gray-200 rounded"></div>
              </div>
              <div className="h-96 bg-gray-200 rounded"></div>
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
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900 mb-4">Checkout</h1>

          {/* Progress Steps */}
          <div className="flex items-center space-x-4 mb-6">
            {[
              { step: 1, title: "Shipping", icon: Truck },
              { step: 2, title: "Payment", icon: CreditCard },
              { step: 3, title: "Review", icon: CheckCircle },
            ].map(({ step, title, icon: Icon }) => (
              <div key={step} className="flex items-center">
                <div
                  className={`flex items-center justify-center w-10 h-10 rounded-full ${
                    currentStep >= step
                      ? "bg-blue-600 text-white"
                      : "bg-gray-200 text-gray-600"
                  }`}
                >
                  {currentStep > step ? (
                    <CheckCircle className="w-5 h-5" />
                  ) : (
                    <Icon className="w-5 h-5" />
                  )}
                </div>
                <span
                  className={`ml-2 text-sm font-medium ${
                    currentStep >= step ? "text-blue-600" : "text-gray-500"
                  }`}
                >
                  {title}
                </span>
                {step < 3 && (
                  <div
                    className={`w-8 h-0.5 ml-4 ${
                      currentStep > step ? "bg-blue-600" : "bg-gray-200"
                    }`}
                  />
                )}
              </div>
            ))}
          </div>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          {/* Main Content */}
          <div className="lg:col-span-2">
            {/* Step 1: Shipping Information */}
            {currentStep === 1 && (
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center">
                    <Truck className="w-5 h-5 mr-2" />
                    Shipping Information
                  </CardTitle>
                </CardHeader>
                <CardContent className="space-y-6">
                  {/* Contact Information */}
                  <div>
                    <h3 className="text-lg font-semibold mb-4">
                      Contact Information
                    </h3>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      <div className="space-y-2">
                        <Label htmlFor="email">Email Address</Label>
                        <Input
                          id="email"
                          type="email"
                          value={contactInfo.email}
                          onChange={(e) =>
                            setContactInfo((prev) => ({
                              ...prev,
                              email: e.target.value,
                            }))
                          }
                          placeholder="john@example.com"
                          required
                        />
                      </div>
                      <div className="space-y-2">
                        <Label htmlFor="contactPhone">Phone Number</Label>
                        <Input
                          id="contactPhone"
                          type="tel"
                          value={contactInfo.phone}
                          onChange={(e) =>
                            setContactInfo((prev) => ({
                              ...prev,
                              phone: e.target.value,
                            }))
                          }
                          placeholder="+1 (555) 123-4567"
                        />
                      </div>
                    </div>
                    <div className="flex items-center space-x-2 mt-4">
                      <Checkbox
                        id="newsletter"
                        checked={contactInfo.newsletter}
                        onCheckedChange={(checked) =>
                          setContactInfo((prev) => ({
                            ...prev,
                            newsletter: checked as boolean,
                          }))
                        }
                      />
                      <Label htmlFor="newsletter" className="text-sm">
                        Subscribe to our newsletter for updates and special
                        offers
                      </Label>
                    </div>
                  </div>

                  <Separator />

                  {/* Shipping Address */}
                  <div>
                    <h3 className="text-lg font-semibold mb-4">
                      Shipping Address
                    </h3>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      <div className="space-y-2">
                        <Label htmlFor="firstName">First Name</Label>
                        <Input
                          id="firstName"
                          value={shippingAddress.firstName}
                          onChange={(e) =>
                            setShippingAddress((prev) => ({
                              ...prev,
                              firstName: e.target.value,
                            }))
                          }
                          required
                        />
                      </div>
                      <div className="space-y-2">
                        <Label htmlFor="lastName">Last Name</Label>
                        <Input
                          id="lastName"
                          value={shippingAddress.lastName}
                          onChange={(e) =>
                            setShippingAddress((prev) => ({
                              ...prev,
                              lastName: e.target.value,
                            }))
                          }
                          required
                        />
                      </div>
                      <div className="space-y-2 md:col-span-2">
                        <Label htmlFor="company">Company (Optional)</Label>
                        <Input
                          id="company"
                          value={shippingAddress.company}
                          onChange={(e) =>
                            setShippingAddress((prev) => ({
                              ...prev,
                              company: e.target.value,
                            }))
                          }
                        />
                      </div>
                      <div className="space-y-2 md:col-span-2">
                        <Label htmlFor="street">Street Address</Label>
                        <Input
                          id="street"
                          value={shippingAddress.street}
                          onChange={(e) =>
                            setShippingAddress((prev) => ({
                              ...prev,
                              street: e.target.value,
                            }))
                          }
                          required
                        />
                      </div>
                      <div className="space-y-2">
                        <Label htmlFor="city">City</Label>
                        <Input
                          id="city"
                          value={shippingAddress.city}
                          onChange={(e) =>
                            setShippingAddress((prev) => ({
                              ...prev,
                              city: e.target.value,
                            }))
                          }
                          required
                        />
                      </div>
                      <div className="space-y-2">
                        <Label htmlFor="state">State</Label>
                        <Select
                          value={shippingAddress.state}
                          onValueChange={(value) =>
                            setShippingAddress((prev) => ({
                              ...prev,
                              state: value,
                            }))
                          }
                        >
                          <SelectTrigger>
                            <SelectValue placeholder="Select state" />
                          </SelectTrigger>
                          <SelectContent>
                            <SelectItem value="CA">California</SelectItem>
                            <SelectItem value="NY">New York</SelectItem>
                            <SelectItem value="TX">Texas</SelectItem>
                            <SelectItem value="FL">Florida</SelectItem>
                          </SelectContent>
                        </Select>
                      </div>
                      <div className="space-y-2">
                        <Label htmlFor="zipCode">ZIP Code</Label>
                        <Input
                          id="zipCode"
                          value={shippingAddress.zipCode}
                          onChange={(e) =>
                            setShippingAddress((prev) => ({
                              ...prev,
                              zipCode: e.target.value,
                            }))
                          }
                          required
                        />
                      </div>
                      <div className="space-y-2">
                        <Label htmlFor="shippingPhone">Phone</Label>
                        <Input
                          id="shippingPhone"
                          type="tel"
                          value={shippingAddress.phone}
                          onChange={(e) =>
                            setShippingAddress((prev) => ({
                              ...prev,
                              phone: e.target.value,
                            }))
                          }
                          required
                        />
                      </div>
                    </div>
                  </div>
                </CardContent>
              </Card>
            )}

            {/* Step 2: Payment Information */}
            {currentStep === 2 && (
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center">
                    <CreditCard className="w-5 h-5 mr-2" />
                    Payment Information
                  </CardTitle>
                </CardHeader>
                <CardContent className="space-y-6">
                  <Tabs defaultValue="card">
                    <TabsList className="grid w-full grid-cols-3">
                      <TabsTrigger value="card">Credit Card</TabsTrigger>
                      <TabsTrigger value="paypal">PayPal</TabsTrigger>
                      <TabsTrigger value="bank">Bank Transfer</TabsTrigger>
                    </TabsList>

                    <TabsContent value="card" className="space-y-4">
                      <div className="space-y-2">
                        <Label htmlFor="cardNumber">Card Number</Label>
                        <Input
                          id="cardNumber"
                          placeholder="1234 5678 9012 3456"
                          value={paymentInfo.cardNumber}
                          onChange={(e) =>
                            setPaymentInfo((prev) => ({
                              ...prev,
                              cardNumber: e.target.value,
                            }))
                          }
                          required
                        />
                      </div>

                      <div className="grid grid-cols-3 gap-4">
                        <div className="space-y-2">
                          <Label htmlFor="expiryMonth">Month</Label>
                          <Select
                            value={paymentInfo.expiryMonth}
                            onValueChange={(value) =>
                              setPaymentInfo((prev) => ({
                                ...prev,
                                expiryMonth: value,
                              }))
                            }
                          >
                            <SelectTrigger>
                              <SelectValue placeholder="MM" />
                            </SelectTrigger>
                            <SelectContent>
                              {Array.from({ length: 12 }, (_, i) => (
                                <SelectItem
                                  key={i + 1}
                                  value={String(i + 1).padStart(2, "0")}
                                >
                                  {String(i + 1).padStart(2, "0")}
                                </SelectItem>
                              ))}
                            </SelectContent>
                          </Select>
                        </div>

                        <div className="space-y-2">
                          <Label htmlFor="expiryYear">Year</Label>
                          <Select
                            value={paymentInfo.expiryYear}
                            onValueChange={(value) =>
                              setPaymentInfo((prev) => ({
                                ...prev,
                                expiryYear: value,
                              }))
                            }
                          >
                            <SelectTrigger>
                              <SelectValue placeholder="YYYY" />
                            </SelectTrigger>
                            <SelectContent>
                              {Array.from({ length: 10 }, (_, i) => {
                                const year = new Date().getFullYear() + i;
                                return (
                                  <SelectItem key={year} value={String(year)}>
                                    {year}
                                  </SelectItem>
                                );
                              })}
                            </SelectContent>
                          </Select>
                        </div>

                        <div className="space-y-2">
                          <Label htmlFor="cvv">CVV</Label>
                          <Input
                            id="cvv"
                            placeholder="123"
                            value={paymentInfo.cvv}
                            onChange={(e) =>
                              setPaymentInfo((prev) => ({
                                ...prev,
                                cvv: e.target.value,
                              }))
                            }
                            maxLength={4}
                            required
                          />
                        </div>
                      </div>

                      <div className="space-y-2">
                        <Label htmlFor="cardholderName">Cardholder Name</Label>
                        <Input
                          id="cardholderName"
                          placeholder="John Doe"
                          value={paymentInfo.cardholderName}
                          onChange={(e) =>
                            setPaymentInfo((prev) => ({
                              ...prev,
                              cardholderName: e.target.value,
                            }))
                          }
                          required
                        />
                      </div>
                    </TabsContent>

                    <TabsContent value="paypal" className="text-center py-8">
                      <div className="space-y-4">
                        <div className="w-16 h-16 bg-blue-600 rounded-full flex items-center justify-center mx-auto">
                          <CreditCard className="w-8 h-8 text-white" />
                        </div>
                        <h3 className="text-lg font-semibold">
                          PayPal Payment
                        </h3>
                        <p className="text-gray-600">
                          You will be redirected to PayPal to complete your
                          payment.
                        </p>
                      </div>
                    </TabsContent>

                    <TabsContent value="bank" className="text-center py-8">
                      <div className="space-y-4">
                        <div className="w-16 h-16 bg-green-600 rounded-full flex items-center justify-center mx-auto">
                          <CreditCard className="w-8 h-8 text-white" />
                        </div>
                        <h3 className="text-lg font-semibold">Bank Transfer</h3>
                        <p className="text-gray-600">
                          Bank transfer details will be provided after order
                          confirmation.
                        </p>
                      </div>
                    </TabsContent>
                  </Tabs>

                  {/* Security Notice */}
                  <div className="flex items-center space-x-2 text-sm text-gray-600 bg-gray-50 p-3 rounded-lg">
                    <Shield className="w-4 h-4 text-green-600" />
                    <span>
                      Your payment information is encrypted and secure
                    </span>
                  </div>
                </CardContent>
              </Card>
            )}

            {/* Step 3: Review Order */}
            {currentStep === 3 && (
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center">
                    <CheckCircle className="w-5 h-5 mr-2" />
                    Review Your Order
                  </CardTitle>
                </CardHeader>
                <CardContent className="space-y-6">
                  {/* Order Items */}
                  <div>
                    <h3 className="font-semibold mb-4">Order Items</h3>
                    <div className="space-y-4">
                      {cart?.items.map((item) => (
                        <div
                          key={item.id}
                          className="flex items-center space-x-4 py-4 border-b"
                        >
                          <div className="w-16 h-16 bg-gray-100 rounded-lg flex items-center justify-center">
                            <img
                              src={item.product.imageUrls[0] || ""}
                              alt={item.product.name}
                              className="w-full h-full object-cover rounded-lg"
                              onError={(e) => {
                                const target = e.target as HTMLImageElement;
                                target.style.display = "none";
                              }}
                            />
                          </div>
                          <div className="flex-1">
                            <h4 className="font-medium">{item.product.name}</h4>
                            <p className="text-sm text-gray-600">
                              Qty: {item.quantity}
                            </p>
                          </div>
                          <div className="text-right">
                            <p className="font-semibold">
                              ${(item.price * item.quantity).toFixed(2)}
                            </p>
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>

                  {/* Shipping & Billing Info */}
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                    <div>
                      <h3 className="font-semibold mb-2">Shipping Address</h3>
                      <div className="text-sm text-gray-600 space-y-1">
                        <p>
                          {shippingAddress.firstName} {shippingAddress.lastName}
                        </p>
                        <p>{shippingAddress.street}</p>
                        <p>
                          {shippingAddress.city}, {shippingAddress.state}{" "}
                          {shippingAddress.zipCode}
                        </p>
                        <p>{shippingAddress.phone}</p>
                      </div>
                    </div>
                    <div>
                      <h3 className="font-semibold mb-2">Payment Method</h3>
                      <div className="text-sm text-gray-600">
                        <p>
                          Credit Card ending in ****
                          {paymentInfo.cardNumber.slice(-4)}
                        </p>
                        <p>{paymentInfo.cardholderName}</p>
                      </div>
                    </div>
                  </div>
                </CardContent>
              </Card>
            )}

            {/* Navigation Buttons */}
            <div className="flex justify-between mt-6">
              <Button
                variant="outline"
                onClick={() => setCurrentStep(Math.max(1, currentStep - 1))}
                disabled={currentStep === 1}
              >
                Previous
              </Button>

              {currentStep < 3 ? (
                <Button onClick={handleNextStep}>Next</Button>
              ) : (
                <Button
                  onClick={handlePlaceOrder}
                  disabled={isPlacingOrder}
                  size="lg"
                  className="bg-green-600 hover:bg-green-700"
                >
                  {isPlacingOrder ? (
                    <>
                      <div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin mr-2" />
                      Processing...
                    </>
                  ) : (
                    <>
                      <Lock className="w-4 h-4 mr-2" />
                      Place Order
                    </>
                  )}
                </Button>
              )}
            </div>
          </div>

          {/* Order Summary Sidebar */}
          <div>
            <Card className="sticky top-24">
              <CardHeader>
                <CardTitle>Order Summary</CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                <div className="space-y-2">
                  {cart?.items.map((item) => (
                    <div key={item.id} className="flex justify-between text-sm">
                      <span>
                        {item.product.name} × {item.quantity}
                      </span>
                      <span>${(item.price * item.quantity).toFixed(2)}</span>
                    </div>
                  ))}
                </div>

                <Separator />

                <div className="space-y-2">
                  <div className="flex justify-between">
                    <span>Subtotal</span>
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

                  <div className="flex justify-between">
                    <span>Tax</span>
                    <span>${calculateTax().toFixed(2)}</span>
                  </div>
                </div>

                <Separator />

                <div className="flex justify-between text-lg font-semibold">
                  <span>Total</span>
                  <span>${calculateTotal().toFixed(2)}</span>
                </div>

                {/* Security Badges */}
                <div className="grid grid-cols-3 gap-2 pt-4 text-center">
                  <div className="flex flex-col items-center text-xs text-gray-600">
                    <Shield className="w-6 h-6 mb-1 text-green-600" />
                    <span>Secure</span>
                  </div>
                  <div className="flex flex-col items-center text-xs text-gray-600">
                    <Truck className="w-6 h-6 mb-1 text-blue-600" />
                    <span>Fast Ship</span>
                  </div>
                  <div className="flex flex-col items-center text-xs text-gray-600">
                    <CheckCircle className="w-6 h-6 mb-1 text-purple-600" />
                    <span>Guaranteed</span>
                  </div>
                </div>
              </CardContent>
            </Card>
          </div>
        </div>
      </div>

      <Footer />
    </div>
  );
};

export default Checkout;
