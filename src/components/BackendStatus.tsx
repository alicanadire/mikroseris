import { useState, useEffect } from "react";
import { CheckCircle, XCircle, AlertCircle, RefreshCw } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";

interface ServiceStatus {
  name: string;
  url: string;
  status: "healthy" | "unhealthy" | "unknown";
  responseTime?: number;
  lastChecked?: Date;
}

const BackendStatus = () => {
  const [services, setServices] = useState<ServiceStatus[]>([
    {
      name: "API Gateway",
      url: "http://localhost:5000/health",
      status: "unknown",
    },
    {
      name: "Identity Service",
      url: "http://localhost:5004/health",
      status: "unknown",
    },
    {
      name: "Product Service",
      url: "http://localhost:5001/health",
      status: "unknown",
    },
    {
      name: "Order Service",
      url: "http://localhost:5002/health",
      status: "unknown",
    },
    {
      name: "User Service",
      url: "http://localhost:5003/health",
      status: "unknown",
    },
    {
      name: "Inventory Service",
      url: "http://localhost:5005/health",
      status: "unknown",
    },
    {
      name: "Notification Service",
      url: "http://localhost:5006/health",
      status: "unknown",
    },
  ]);
  const [isChecking, setIsChecking] = useState(false);

  const checkServiceHealth = async (
    service: ServiceStatus,
  ): Promise<ServiceStatus> => {
    const startTime = Date.now();
    try {
      const response = await fetch(service.url, {
        method: "GET",
        headers: { Accept: "application/json" },
        signal: AbortSignal.timeout(5000), // 5 second timeout
      });

      const responseTime = Date.now() - startTime;

      return {
        ...service,
        status: response.ok ? "healthy" : "unhealthy",
        responseTime,
        lastChecked: new Date(),
      };
    } catch (error) {
      return {
        ...service,
        status: "unhealthy",
        responseTime: Date.now() - startTime,
        lastChecked: new Date(),
      };
    }
  };

  const checkAllServices = async () => {
    setIsChecking(true);

    const updatedServices = await Promise.all(
      services.map((service) => checkServiceHealth(service)),
    );

    setServices(updatedServices);
    setIsChecking(false);
  };

  useEffect(() => {
    checkAllServices();

    // Check every 30 seconds
    const interval = setInterval(checkAllServices, 30000);
    return () => clearInterval(interval);
  }, []);

  const getStatusIcon = (status: ServiceStatus["status"]) => {
    switch (status) {
      case "healthy":
        return <CheckCircle className="w-4 h-4 text-green-600" />;
      case "unhealthy":
        return <XCircle className="w-4 h-4 text-red-600" />;
      default:
        return <AlertCircle className="w-4 h-4 text-gray-400" />;
    }
  };

  const getStatusBadge = (status: ServiceStatus["status"]) => {
    switch (status) {
      case "healthy":
        return <Badge className="bg-green-100 text-green-800">Healthy</Badge>;
      case "unhealthy":
        return <Badge variant="destructive">Unhealthy</Badge>;
      default:
        return <Badge variant="secondary">Unknown</Badge>;
    }
  };

  const healthyCount = services.filter((s) => s.status === "healthy").length;
  const totalCount = services.length;

  return (
    <Card className="w-full max-w-4xl mx-auto">
      <CardHeader className="flex flex-row items-center justify-between">
        <div>
          <CardTitle className="flex items-center gap-2">
            Backend Services Status
            <Badge
              variant={healthyCount === totalCount ? "default" : "destructive"}
            >
              {healthyCount}/{totalCount} Healthy
            </Badge>
          </CardTitle>
          <p className="text-sm text-gray-600 mt-1">
            Real-time status of ToyStore microservices
          </p>
        </div>
        <Button
          onClick={checkAllServices}
          disabled={isChecking}
          variant="outline"
          size="sm"
        >
          {isChecking ? (
            <RefreshCw className="w-4 h-4 animate-spin mr-2" />
          ) : (
            <RefreshCw className="w-4 h-4 mr-2" />
          )}
          Refresh
        </Button>
      </CardHeader>
      <CardContent>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {services.map((service) => (
            <div
              key={service.name}
              className="border rounded-lg p-4 space-y-2 hover:shadow-sm transition-shadow"
            >
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-2">
                  {getStatusIcon(service.status)}
                  <span className="font-medium text-sm">{service.name}</span>
                </div>
                {getStatusBadge(service.status)}
              </div>

              <div className="text-xs text-gray-500 space-y-1">
                <div>URL: {service.url}</div>
                {service.responseTime && (
                  <div>Response: {service.responseTime}ms</div>
                )}
                {service.lastChecked && (
                  <div>
                    Last checked: {service.lastChecked.toLocaleTimeString()}
                  </div>
                )}
              </div>
            </div>
          ))}
        </div>

        {/* Backend Setup Instructions */}
        {healthyCount === 0 && (
          <div className="mt-6 p-4 bg-blue-50 rounded-lg border border-blue-200">
            <h3 className="font-medium text-blue-900 mb-2">
              Backend Not Running?
            </h3>
            <p className="text-sm text-blue-800 mb-3">
              The ToyStore backend microservices are not running. To start them:
            </p>
            <div className="bg-blue-900 text-blue-100 p-3 rounded font-mono text-sm">
              <div>cd backend</div>
              <div>./scripts/deploy.sh</div>
              <div className="mt-2 text-blue-300">
                # Or manually with Docker:
              </div>
              <div>docker-compose up -d</div>
            </div>
          </div>
        )}

        {/* Partial Backend Warning */}
        {healthyCount > 0 && healthyCount < totalCount && (
          <div className="mt-6 p-4 bg-yellow-50 rounded-lg border border-yellow-200">
            <h3 className="font-medium text-yellow-900 mb-2">
              Partial Backend Status
            </h3>
            <p className="text-sm text-yellow-800">
              Some microservices are not responding. The application may have
              limited functionality.
            </p>
          </div>
        )}
      </CardContent>
    </Card>
  );
};

export default BackendStatus;
