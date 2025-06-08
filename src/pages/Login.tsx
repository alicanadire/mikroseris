import { useState } from "react";
import { Link, useNavigate, useLocation } from "react-router-dom";
import { Eye, EyeOff, Package, Mail, Lock, User, Phone } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Checkbox } from "@/components/ui/checkbox";
import { Separator } from "@/components/ui/separator";
import { toast } from "@/hooks/use-toast";
import Header from "@/components/Header";
import Footer from "@/components/Footer";
import ApiClient from "@/lib/api";

const Login = () => {
  const [loginForm, setLoginForm] = useState({
    email: "",
    password: "",
    rememberMe: false,
  });

  const [registerForm, setRegisterForm] = useState({
    firstName: "",
    lastName: "",
    email: "",
    password: "",
    confirmPassword: "",
    phone: "",
    agreeToTerms: false,
  });

  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [activeTab, setActiveTab] = useState("login");

  const navigate = useNavigate();
  const location = useLocation();

  const from = location.state?.from?.pathname || "/";

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!loginForm.email || !loginForm.password) {
      toast({
        title: "Error",
        description: "Please fill in all fields.",
        variant: "destructive",
      });
      return;
    }

    setIsLoading(true);
    try {
      await ApiClient.login(loginForm.email, loginForm.password);

      toast({
        title: "Welcome back!",
        description: "You have been successfully logged in.",
      });

      navigate(from, { replace: true });
    } catch (error) {
      toast({
        title: "Login Failed",
        description: "Invalid email or password. Please try again.",
        variant: "destructive",
      });
    } finally {
      setIsLoading(false);
    }
  };

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();

    if (
      !registerForm.firstName ||
      !registerForm.lastName ||
      !registerForm.email ||
      !registerForm.password
    ) {
      toast({
        title: "Error",
        description: "Please fill in all required fields.",
        variant: "destructive",
      });
      return;
    }

    if (registerForm.password !== registerForm.confirmPassword) {
      toast({
        title: "Error",
        description: "Passwords do not match.",
        variant: "destructive",
      });
      return;
    }

    if (registerForm.password.length < 6) {
      toast({
        title: "Error",
        description: "Password must be at least 6 characters long.",
        variant: "destructive",
      });
      return;
    }

    if (!registerForm.agreeToTerms) {
      toast({
        title: "Error",
        description: "Please agree to the Terms of Service and Privacy Policy.",
        variant: "destructive",
      });
      return;
    }

    setIsLoading(true);
    try {
      // In a real app, this would call the register API
      await new Promise((resolve) => setTimeout(resolve, 1500)); // Simulate API call

      toast({
        title: "Account Created!",
        description:
          "Your account has been created successfully. Please log in.",
      });

      setActiveTab("login");
      setLoginForm((prev) => ({ ...prev, email: registerForm.email }));
    } catch (error) {
      toast({
        title: "Registration Failed",
        description:
          "An error occurred while creating your account. Please try again.",
        variant: "destructive",
      });
    } finally {
      setIsLoading(false);
    }
  };

  const handleSocialLogin = (provider: string) => {
    toast({
      title: "Coming Soon",
      description: `${provider} login will be available soon!`,
    });
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-purple-50">
      <Header />

      <div className="container mx-auto px-4 py-8">
        <div className="max-w-md mx-auto">
          {/* Logo and Welcome */}
          <div className="text-center mb-8">
            <Link
              to="/"
              className="inline-flex items-center space-x-2 font-bold text-3xl text-blue-600 mb-4"
            >
              <div className="w-10 h-10 bg-gradient-to-br from-blue-500 to-purple-600 rounded-lg flex items-center justify-center">
                <Package className="w-6 h-6 text-white" />
              </div>
              <span>ToyStore</span>
            </Link>
            <h1 className="text-2xl font-bold text-gray-900 mb-2">Welcome!</h1>
            <p className="text-gray-600">
              Sign in to your account or create a new one
            </p>
          </div>

          <Card className="shadow-lg">
            <Tabs value={activeTab} onValueChange={setActiveTab}>
              <TabsList className="grid w-full grid-cols-2">
                <TabsTrigger value="login">Sign In</TabsTrigger>
                <TabsTrigger value="register">Sign Up</TabsTrigger>
              </TabsList>

              {/* Login Tab */}
              <TabsContent value="login">
                <form onSubmit={handleLogin}>
                  <CardHeader>
                    <CardTitle>Sign In</CardTitle>
                    <CardDescription>
                      Enter your credentials to access your account
                    </CardDescription>
                  </CardHeader>

                  <CardContent className="space-y-4">
                    <div className="space-y-2">
                      <Label htmlFor="email">Email</Label>
                      <div className="relative">
                        <Mail className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-4 h-4" />
                        <Input
                          id="email"
                          type="email"
                          placeholder="Enter your email"
                          value={loginForm.email}
                          onChange={(e) =>
                            setLoginForm((prev) => ({
                              ...prev,
                              email: e.target.value,
                            }))
                          }
                          className="pl-10"
                          required
                        />
                      </div>
                    </div>

                    <div className="space-y-2">
                      <Label htmlFor="password">Password</Label>
                      <div className="relative">
                        <Lock className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-4 h-4" />
                        <Input
                          id="password"
                          type={showPassword ? "text" : "password"}
                          placeholder="Enter your password"
                          value={loginForm.password}
                          onChange={(e) =>
                            setLoginForm((prev) => ({
                              ...prev,
                              password: e.target.value,
                            }))
                          }
                          className="pl-10 pr-10"
                          required
                        />
                        <Button
                          type="button"
                          variant="ghost"
                          size="icon"
                          className="absolute right-0 top-0 h-full px-3 hover:bg-transparent"
                          onClick={() => setShowPassword(!showPassword)}
                        >
                          {showPassword ? (
                            <EyeOff className="w-4 h-4" />
                          ) : (
                            <Eye className="w-4 h-4" />
                          )}
                        </Button>
                      </div>
                    </div>

                    <div className="flex items-center justify-between">
                      <div className="flex items-center space-x-2">
                        <Checkbox
                          id="remember"
                          checked={loginForm.rememberMe}
                          onCheckedChange={(checked) =>
                            setLoginForm((prev) => ({
                              ...prev,
                              rememberMe: checked as boolean,
                            }))
                          }
                        />
                        <Label htmlFor="remember" className="text-sm">
                          Remember me
                        </Label>
                      </div>
                      <Link
                        to="/forgot-password"
                        className="text-sm text-blue-600 hover:underline"
                      >
                        Forgot password?
                      </Link>
                    </div>

                    {/* Demo credentials */}
                    <div className="bg-gray-50 p-3 rounded-lg text-sm">
                      <p className="font-medium text-gray-700 mb-1">
                        Demo credentials:
                      </p>
                      <p className="text-gray-600">
                        Customer: customer@toystore.com / password
                      </p>
                      <p className="text-gray-600">
                        Admin: admin@toystore.com / password
                      </p>
                    </div>
                  </CardContent>

                  <CardFooter className="flex flex-col space-y-4">
                    <Button
                      type="submit"
                      className="w-full"
                      disabled={isLoading}
                      size="lg"
                    >
                      {isLoading ? (
                        <>
                          <div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin mr-2" />
                          Signing in...
                        </>
                      ) : (
                        "Sign In"
                      )}
                    </Button>

                    <div className="relative w-full">
                      <Separator />
                      <span className="absolute left-1/2 top-1/2 transform -translate-x-1/2 -translate-y-1/2 bg-white px-2 text-sm text-gray-500">
                        or continue with
                      </span>
                    </div>

                    <div className="grid grid-cols-2 gap-3">
                      <Button
                        type="button"
                        variant="outline"
                        onClick={() => handleSocialLogin("Google")}
                      >
                        <svg className="w-4 h-4 mr-2" viewBox="0 0 24 24">
                          <path
                            fill="currentColor"
                            d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"
                          />
                          <path
                            fill="currentColor"
                            d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"
                          />
                          <path
                            fill="currentColor"
                            d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"
                          />
                          <path
                            fill="currentColor"
                            d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"
                          />
                        </svg>
                        Google
                      </Button>
                      <Button
                        type="button"
                        variant="outline"
                        onClick={() => handleSocialLogin("Facebook")}
                      >
                        <svg
                          className="w-4 h-4 mr-2"
                          fill="currentColor"
                          viewBox="0 0 24 24"
                        >
                          <path d="M24 12.073c0-6.627-5.373-12-12-12s-12 5.373-12 12c0 5.99 4.388 10.954 10.125 11.854v-8.385H7.078v-3.47h3.047V9.43c0-3.007 1.792-4.669 4.533-4.669 1.312 0 2.686.235 2.686.235v2.953H15.83c-1.491 0-1.956.925-1.956 1.874v2.25h3.328l-.532 3.47h-2.796v8.385C19.612 23.027 24 18.062 24 12.073z" />
                        </svg>
                        Facebook
                      </Button>
                    </div>
                  </CardFooter>
                </form>
              </TabsContent>

              {/* Register Tab */}
              <TabsContent value="register">
                <form onSubmit={handleRegister}>
                  <CardHeader>
                    <CardTitle>Create Account</CardTitle>
                    <CardDescription>
                      Join ToyStore and start your shopping journey
                    </CardDescription>
                  </CardHeader>

                  <CardContent className="space-y-4">
                    <div className="grid grid-cols-2 gap-4">
                      <div className="space-y-2">
                        <Label htmlFor="firstName">First Name</Label>
                        <Input
                          id="firstName"
                          placeholder="John"
                          value={registerForm.firstName}
                          onChange={(e) =>
                            setRegisterForm((prev) => ({
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
                          placeholder="Doe"
                          value={registerForm.lastName}
                          onChange={(e) =>
                            setRegisterForm((prev) => ({
                              ...prev,
                              lastName: e.target.value,
                            }))
                          }
                          required
                        />
                      </div>
                    </div>

                    <div className="space-y-2">
                      <Label htmlFor="registerEmail">Email</Label>
                      <div className="relative">
                        <Mail className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-4 h-4" />
                        <Input
                          id="registerEmail"
                          type="email"
                          placeholder="john@example.com"
                          value={registerForm.email}
                          onChange={(e) =>
                            setRegisterForm((prev) => ({
                              ...prev,
                              email: e.target.value,
                            }))
                          }
                          className="pl-10"
                          required
                        />
                      </div>
                    </div>

                    <div className="space-y-2">
                      <Label htmlFor="phone">Phone (Optional)</Label>
                      <div className="relative">
                        <Phone className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-4 h-4" />
                        <Input
                          id="phone"
                          type="tel"
                          placeholder="+1 (555) 123-4567"
                          value={registerForm.phone}
                          onChange={(e) =>
                            setRegisterForm((prev) => ({
                              ...prev,
                              phone: e.target.value,
                            }))
                          }
                          className="pl-10"
                        />
                      </div>
                    </div>

                    <div className="space-y-2">
                      <Label htmlFor="registerPassword">Password</Label>
                      <div className="relative">
                        <Lock className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-4 h-4" />
                        <Input
                          id="registerPassword"
                          type={showPassword ? "text" : "password"}
                          placeholder="Create a strong password"
                          value={registerForm.password}
                          onChange={(e) =>
                            setRegisterForm((prev) => ({
                              ...prev,
                              password: e.target.value,
                            }))
                          }
                          className="pl-10 pr-10"
                          required
                        />
                        <Button
                          type="button"
                          variant="ghost"
                          size="icon"
                          className="absolute right-0 top-0 h-full px-3 hover:bg-transparent"
                          onClick={() => setShowPassword(!showPassword)}
                        >
                          {showPassword ? (
                            <EyeOff className="w-4 h-4" />
                          ) : (
                            <Eye className="w-4 h-4" />
                          )}
                        </Button>
                      </div>
                    </div>

                    <div className="space-y-2">
                      <Label htmlFor="confirmPassword">Confirm Password</Label>
                      <div className="relative">
                        <Lock className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-4 h-4" />
                        <Input
                          id="confirmPassword"
                          type={showConfirmPassword ? "text" : "password"}
                          placeholder="Confirm your password"
                          value={registerForm.confirmPassword}
                          onChange={(e) =>
                            setRegisterForm((prev) => ({
                              ...prev,
                              confirmPassword: e.target.value,
                            }))
                          }
                          className="pl-10 pr-10"
                          required
                        />
                        <Button
                          type="button"
                          variant="ghost"
                          size="icon"
                          className="absolute right-0 top-0 h-full px-3 hover:bg-transparent"
                          onClick={() =>
                            setShowConfirmPassword(!showConfirmPassword)
                          }
                        >
                          {showConfirmPassword ? (
                            <EyeOff className="w-4 h-4" />
                          ) : (
                            <Eye className="w-4 h-4" />
                          )}
                        </Button>
                      </div>
                    </div>

                    <div className="flex items-start space-x-2">
                      <Checkbox
                        id="terms"
                        checked={registerForm.agreeToTerms}
                        onCheckedChange={(checked) =>
                          setRegisterForm((prev) => ({
                            ...prev,
                            agreeToTerms: checked as boolean,
                          }))
                        }
                        className="mt-1"
                      />
                      <Label
                        htmlFor="terms"
                        className="text-sm leading-relaxed"
                      >
                        I agree to the{" "}
                        <Link
                          to="/terms"
                          className="text-blue-600 hover:underline"
                        >
                          Terms of Service
                        </Link>{" "}
                        and{" "}
                        <Link
                          to="/privacy"
                          className="text-blue-600 hover:underline"
                        >
                          Privacy Policy
                        </Link>
                      </Label>
                    </div>
                  </CardContent>

                  <CardFooter>
                    <Button
                      type="submit"
                      className="w-full"
                      disabled={isLoading}
                      size="lg"
                    >
                      {isLoading ? (
                        <>
                          <div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin mr-2" />
                          Creating account...
                        </>
                      ) : (
                        "Create Account"
                      )}
                    </Button>
                  </CardFooter>
                </form>
              </TabsContent>
            </Tabs>
          </Card>
        </div>
      </div>

      <Footer />
    </div>
  );
};

export default Login;
