import { Link } from "react-router-dom";
import { Home, Search, Package, ArrowLeft } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import Header from "@/components/Header";
import Footer from "@/components/Footer";

const NotFound = () => {
  return (
    <div className="min-h-screen bg-white">
      <Header />

      <div className="container mx-auto px-4 py-16">
        <div className="max-w-2xl mx-auto text-center">
          {/* 404 Illustration */}
          <div className="mb-8">
            <div className="relative">
              <div className="text-9xl font-bold text-gray-200 mb-4">404</div>
              <div className="absolute inset-0 flex items-center justify-center">
                <Package className="w-24 h-24 text-gray-400" />
              </div>
            </div>
          </div>

          {/* Error Message */}
          <h1 className="text-4xl font-bold text-gray-900 mb-4">
            Oops! Toy Not Found
          </h1>
          <p className="text-xl text-gray-600 mb-8">
            The page you're looking for seems to have wandered off to the toy
            chest. Let's help you find what you're looking for!
          </p>

          {/* Search Bar */}
          <div className="mb-8 max-w-md mx-auto">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-4 h-4" />
              <Input
                type="search"
                placeholder="Search for toys..."
                className="pl-10 pr-4"
              />
            </div>
          </div>

          {/* Quick Actions */}
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 mb-8">
            <Button
              asChild
              variant="outline"
              className="h-auto flex flex-col p-6"
            >
              <Link to="/">
                <Home className="w-8 h-8 mb-2 text-blue-600" />
                <span className="font-semibold">Go Home</span>
                <span className="text-sm text-gray-500">Back to homepage</span>
              </Link>
            </Button>

            <Button
              asChild
              variant="outline"
              className="h-auto flex flex-col p-6"
            >
              <Link to="/products">
                <Package className="w-8 h-8 mb-2 text-green-600" />
                <span className="font-semibold">Browse Toys</span>
                <span className="text-sm text-gray-500">
                  Explore our collection
                </span>
              </Link>
            </Button>

            <Button
              asChild
              variant="outline"
              className="h-auto flex flex-col p-6"
            >
              <Link to="/contact">
                <Search className="w-8 h-8 mb-2 text-purple-600" />
                <span className="font-semibold">Get Help</span>
                <span className="text-sm text-gray-500">Contact support</span>
              </Link>
            </Button>
          </div>

          {/* Popular Categories */}
          <div className="text-left max-w-lg mx-auto">
            <h2 className="text-lg font-semibold text-gray-900 mb-4">
              Popular Categories
            </h2>
            <div className="grid grid-cols-2 gap-2">
              <Link
                to="/products?category=action-figures"
                className="text-blue-600 hover:text-blue-800 text-sm"
              >
                → Action Figures
              </Link>
              <Link
                to="/products?category=building-blocks"
                className="text-blue-600 hover:text-blue-800 text-sm"
              >
                → Building Blocks
              </Link>
              <Link
                to="/products?category=dolls"
                className="text-blue-600 hover:text-blue-800 text-sm"
              >
                → Dolls
              </Link>
              <Link
                to="/products?category=educational-toys"
                className="text-blue-600 hover:text-blue-800 text-sm"
              >
                → Educational Toys
              </Link>
              <Link
                to="/products?category=remote-control"
                className="text-blue-600 hover:text-blue-800 text-sm"
              >
                → RC Toys
              </Link>
              <Link
                to="/products?category=board-games"
                className="text-blue-600 hover:text-blue-800 text-sm"
              >
                → Board Games
              </Link>
            </div>
          </div>

          {/* Go Back Button */}
          <div className="mt-8">
            <Button
              variant="ghost"
              onClick={() => window.history.back()}
              className="text-gray-600 hover:text-gray-800"
            >
              <ArrowLeft className="w-4 h-4 mr-2" />
              Go Back
            </Button>
          </div>
        </div>
      </div>

      <Footer />
    </div>
  );
};

export default NotFound;
