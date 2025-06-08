#!/usr/bin/env pwsh

Write-Host "🔧 ToyStore Backend Configuration Validation" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

$configIssues = @()
$configSuccess = @()

# Function to check if a service has proper Redis configuration
function Test-RedisConfiguration {
    param($servicePath, $serviceName)
    
    $programFile = Join-Path $servicePath "Program.cs"
    if (Test-Path $programFile) {
        $content = Get-Content $programFile -Raw
        
        if ($content -match "IConnectionMultiplexer" -and $content -match "ConnectionMultiplexer\.Connect") {
            $configSuccess += "✅ $serviceName - Redis configuration OK"
            return $true
        } else {
            $configIssues += "❌ $serviceName - Missing Redis ConnectionMultiplexer configuration"
            return $false
        }
    } else {
        $configIssues += "❌ $serviceName - Program.cs file not found"
        return $false
    }
}

# Function to check if a service has proper RabbitMQ/EventBus configuration
function Test-EventBusConfiguration {
    param($servicePath, $serviceName)
    
    $programFile = Join-Path $servicePath "Program.cs"
    if (Test-Path $programFile) {
        $content = Get-Content $programFile -Raw
        
        if ($content -match "IEventBus" -and $content -match "RabbitMQEventBus" -and $content -match "RabbitMQSettings") {
            $configSuccess += "✅ $serviceName - EventBus configuration OK"
            return $true
        } else {
            $configIssues += "❌ $serviceName - Missing EventBus/RabbitMQ configuration"
            return $false
        }
    } else {
        $configIssues += "❌ $serviceName - Program.cs file not found"
        return $false
    }
}

# Function to check if a service has proper Swagger configuration
function Test-SwaggerConfiguration {
    param($servicePath, $serviceName)
    
    $programFile = Join-Path $servicePath "Program.cs"
    if (Test-Path $programFile) {
        $content = Get-Content $programFile -Raw
        
        if ($content -match "AddSwaggerGen" -and $content -match "UseSwagger" -and $content -match "JWT|Bearer") {
            $configSuccess += "✅ $serviceName - Swagger configuration OK"
            return $true
        } else {
            $configIssues += "❌ $serviceName - Missing or incomplete Swagger configuration"
            return $false
        }
    } else {
        $configIssues += "❌ $serviceName - Program.cs file not found"
        return $false
    }
}

Write-Host "`n🔍 Checking Service Configurations..." -ForegroundColor Yellow

# Define services to check
$services = @(
    @{ Path = "backend/src/Services/Product/ToyStore.ProductService.API"; Name = "Product Service" }
    @{ Path = "backend/src/Services/Order/ToyStore.OrderService"; Name = "Order Service" }
    @{ Path = "backend/src/Services/User/ToyStore.UserService"; Name = "User Service" }
    @{ Path = "backend/src/Services/Inventory/ToyStore.InventoryService"; Name = "Inventory Service" }
    @{ Path = "backend/src/Services/Identity/ToyStore.IdentityService"; Name = "Identity Service" }
)

Write-Host "`n📡 REDIS CONFIGURATION CHECK:" -ForegroundColor Cyan
Write-Host "==============================" -ForegroundColor Cyan

foreach ($service in $services) {
    Test-RedisConfiguration -servicePath $service.Path -serviceName $service.Name
}

Write-Host "`n🐰 RABBITMQ/EVENTBUS CONFIGURATION CHECK:" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan

foreach ($service in $services) {
    Test-EventBusConfiguration -servicePath $service.Path -serviceName $service.Name
}

Write-Host "`n📚 SWAGGER CONFIGURATION CHECK:" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan

foreach ($service in $services) {
    Test-SwaggerConfiguration -servicePath $service.Path -serviceName $service.Name
}

Write-Host "`n🔗 API GATEWAY CONFIGURATION CHECK:" -ForegroundColor Cyan
Write-Host "====================================" -ForegroundColor Cyan

$ocelotFile = "backend/src/ApiGateway/ToyStore.ApiGateway/ocelot.json"
if (Test-Path $ocelotFile) {
    $ocelotContent = Get-Content $ocelotFile -Raw
    
    if ($ocelotContent -match "/api/v1/") {
        $configSuccess += "✅ API Gateway - API versioning (v1) configured"
    } else {
        $configIssues += "❌ API Gateway - Missing API versioning (v1)"
    }
    
    if ($ocelotContent -match "/health") {
        $configSuccess += "✅ API Gateway - Health check routes configured"
    } else {
        $configIssues += "❌ API Gateway - Missing health check routes"
    }
    
    if ($ocelotContent -match "notifications") {
        $configIssues += "❌ API Gateway - Still contains notification service routes (should be removed)"
    } else {
        $configSuccess += "✅ API Gateway - Notification service routes removed"
    }
} else {
    $configIssues += "❌ API Gateway - ocelot.json file not found"
}

Write-Host "`n🗃️ DATABASE CONFIGURATION CHECK:" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan

# Check if services have proper database context configuration
$dbServices = @(
    @{ Path = "backend/src/Services/Product/ToyStore.ProductService.API"; Name = "Product Service"; ExpectedContext = "ProductDbContext" }
    @{ Path = "backend/src/Services/Order/ToyStore.OrderService"; Name = "Order Service"; ExpectedContext = "OrderDbContext" }
    @{ Path = "backend/src/Services/User/ToyStore.UserService"; Name = "User Service"; ExpectedContext = "UserDbContext" }
    @{ Path = "backend/src/Services/Inventory/ToyStore.InventoryService"; Name = "Inventory Service"; ExpectedContext = "InventoryDbContext" }
    @{ Path = "backend/src/Services/Identity/ToyStore.IdentityService"; Name = "Identity Service"; ExpectedContext = "ApplicationDbContext" }
)

foreach ($service in $dbServices) {
    $programFile = Join-Path $service.Path "Program.cs"
    if (Test-Path $programFile) {
        $content = Get-Content $programFile -Raw
        
        if ($content -match $service.ExpectedContext -and $content -match "AddDbContext") {
            $configSuccess += "✅ $($service.Name) - Database context configured"
        } else {
            $configIssues += "❌ $($service.Name) - Missing or incorrect database context configuration"
        }
    }
}

Write-Host "`n📦 PACKAGE DEPENDENCIES CHECK:" -ForegroundColor Cyan
Write-Host "===============================" -ForegroundColor Cyan

# Check for critical NuGet packages
$packageChecks = @(
    @{ Path = "backend/src/Shared/ToyStore.EventBus/ToyStore.EventBus.csproj"; Packages = @("RabbitMQ.Client") }
    @{ Path = "backend/src/Shared/ToyStore.Shared/ToyStore.Shared.csproj"; Packages = @("StackExchange.Redis") }
)

foreach ($check in $packageChecks) {
    if (Test-Path $check.Path) {
        $content = Get-Content $check.Path -Raw
        
        foreach ($package in $check.Packages) {
            if ($content -match $package) {
                $configSuccess += "✅ Package $package found in $(Split-Path $check.Path -Leaf)"
            } else {
                $configIssues += "❌ Package $package missing in $(Split-Path $check.Path -Leaf)"
            }
        }
    }
}

Write-Host "`n📊 CONFIGURATION SUMMARY:" -ForegroundColor Cyan
Write-Host "=========================" -ForegroundColor Cyan

Write-Host "`n✅ SUCCESSFUL CONFIGURATIONS ($($configSuccess.Count)):" -ForegroundColor Green
foreach ($success in $configSuccess) {
    Write-Host "   $success" -ForegroundColor Green
}

if ($configIssues.Count -gt 0) {
    Write-Host "`n❌ CONFIGURATION ISSUES ($($configIssues.Count)):" -ForegroundColor Red
    foreach ($issue in $configIssues) {
        Write-Host "   $issue" -ForegroundColor Red
    }
    
    Write-Host "`n🔧 RECOMMENDED FIXES:" -ForegroundColor Yellow
    Write-Host "=====================" -ForegroundColor Yellow
    
    if ($configIssues -match "Redis") {
        Write-Host "   • Add Redis ConnectionMultiplexer configuration to services" -ForegroundColor Yellow
        Write-Host "   • Ensure Redis connection string is properly configured" -ForegroundColor Yellow
    }
    
    if ($configIssues -match "EventBus") {
        Write-Host "   • Add RabbitMQ EventBus configuration to services" -ForegroundColor Yellow
        Write-Host "   • Configure RabbitMQSettings in appsettings.json" -ForegroundColor Yellow
    }
    
    if ($configIssues -match "Swagger") {
        Write-Host "   • Add complete Swagger configuration with JWT support" -ForegroundColor Yellow
        Write-Host "   • Enable XML documentation generation" -ForegroundColor Yellow
    }
    
    if ($configIssues -match "API Gateway") {
        Write-Host "   • Update API Gateway routes to use /api/v1/ versioning" -ForegroundColor Yellow
        Write-Host "   • Remove deprecated notification service routes" -ForegroundColor Yellow
    }
} else {
    Write-Host "`n🎉 ALL CONFIGURATIONS ARE CORRECT!" -ForegroundColor Green
}

Write-Host "`n💡 NEXT STEPS:" -ForegroundColor Cyan
Write-Host "===============" -ForegroundColor Cyan

if ($configIssues.Count -eq 0) {
    Write-Host "   1. ✅ Configuration validation complete - all services properly configured" -ForegroundColor Green
    Write-Host "   2. 🚀 Ready to start all services and test the backend" -ForegroundColor Green
    Write-Host "   3. 🧪 Run integration tests to verify functionality" -ForegroundColor Green
    Write-Host "   4. 🔄 Test event publishing between services" -ForegroundColor Green
} else {
    Write-Host "   1. 🔧 Fix the configuration issues listed above" -ForegroundColor Yellow
    Write-Host "   2. 🔁 Re-run this validation script" -ForegroundColor Yellow
    Write-Host "   3. 🚀 Start services once all issues are resolved" -ForegroundColor Yellow
}

Write-Host "`n🏁 Configuration validation completed!" -ForegroundColor Cyan

# Return exit code based on issues found
if ($configIssues.Count -eq 0) {
    exit 0
} else {
    exit 1
}
