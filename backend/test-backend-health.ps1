#!/usr/bin/env pwsh

Write-Host "🔍 ToyStore Backend Health Check" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan

$services = @(
    @{ Name = "Product Service"; Port = 5001; Path = "/swagger/v1/swagger.json" }
    @{ Name = "Order Service"; Port = 5002; Path = "/swagger/v1/swagger.json" }
    @{ Name = "Inventory Service"; Port = 5003; Path = "/swagger/v1/swagger.json" }
    @{ Name = "User Service"; Port = 5004; Path = "/swagger/v1/swagger.json" }
    @{ Name = "Notification Service"; Port = 5005; Path = "/swagger/v1/swagger.json" }
    @{ Name = "Identity Service"; Port = 5006; Path = "/.well-known/openid_configuration" }
    @{ Name = "API Gateway"; Port = 5000; Path = "/health" }
)

$healthResults = @()

Write-Host "`n🚀 Checking Service Health..." -ForegroundColor Yellow

foreach ($service in $services) {
    Write-Host "`n⏳ Testing $($service.Name)..." -ForegroundColor Gray
    
    try {
        $url = "http://localhost:$($service.Port)$($service.Path)"
        $response = Invoke-WebRequest -Uri $url -Method GET -TimeoutSec 10 -ErrorAction Stop
        
        if ($response.StatusCode -eq 200) {
            Write-Host "✅ $($service.Name) - HEALTHY (Port: $($service.Port))" -ForegroundColor Green
            $healthResults += @{ Service = $service.Name; Status = "HEALTHY"; Port = $service.Port; Response = $response.StatusCode }
        } else {
            Write-Host "⚠️  $($service.Name) - UNHEALTHY (Status: $($response.StatusCode))" -ForegroundColor Yellow
            $healthResults += @{ Service = $service.Name; Status = "UNHEALTHY"; Port = $service.Port; Response = $response.StatusCode }
        }
    }
    catch {
        Write-Host "❌ $($service.Name) - DOWN (Port: $($service.Port)) - $($_.Exception.Message)" -ForegroundColor Red
        $healthResults += @{ Service = $service.Name; Status = "DOWN"; Port = $service.Port; Response = $_.Exception.Message }
    }
}

Write-Host "`n📊 HEALTH SUMMARY" -ForegroundColor Cyan
Write-Host "=================" -ForegroundColor Cyan

$healthyCount = ($healthResults | Where-Object { $_.Status -eq "HEALTHY" }).Count
$totalCount = $healthResults.Count

Write-Host "`n📈 Overall Health: $healthyCount/$totalCount services healthy" -ForegroundColor $(if ($healthyCount -eq $totalCount) { "Green" } else { "Red" })

foreach ($result in $healthResults) {
    $color = switch ($result.Status) {
        "HEALTHY" { "Green" }
        "UNHEALTHY" { "Yellow" }
        "DOWN" { "Red" }
    }
    Write-Host "   $($result.Service): $($result.Status)" -ForegroundColor $color
}

Write-Host "`n🛠️  SWAGGER ENDPOINTS" -ForegroundColor Cyan
Write-Host "=====================" -ForegroundColor Cyan

$swaggerServices = $services | Where-Object { $_.Path -like "*/swagger/*" }
foreach ($service in $swaggerServices) {
    $swaggerUrl = "http://localhost:$($service.Port)/swagger"
    Write-Host "   $($service.Name): $swaggerUrl" -ForegroundColor Blue
}

Write-Host "`n🔍 API TESTING EXAMPLES" -ForegroundColor Cyan
Write-Host "=======================" -ForegroundColor Cyan

Write-Host "`n# Test Product Service:" -ForegroundColor Gray
Write-Host "curl -X GET 'http://localhost:5001/api/v1/products?page=1&pageSize=10'" -ForegroundColor Blue

Write-Host "`n# Test Health Endpoints:" -ForegroundColor Gray
Write-Host "curl -X GET 'http://localhost:5001/health'" -ForegroundColor Blue
Write-Host "curl -X GET 'http://localhost:5002/health'" -ForegroundColor Blue

Write-Host "`n# Test Search Functionality:" -ForegroundColor Gray
Write-Host "curl -X GET 'http://localhost:5001/api/v1/products?searchTerm=toy&minPrice=10&maxPrice=100'" -ForegroundColor Blue

Write-Host "`n🎯 NEXT STEPS" -ForegroundColor Cyan
Write-Host "=============" -ForegroundColor Cyan

if ($healthyCount -lt $totalCount) {
    Write-Host "❗ Some services are down. Check the following:" -ForegroundColor Red
    Write-Host "   1. Ensure Docker containers are running: docker-compose up -d" -ForegroundColor Gray
    Write-Host "   2. Check database connections" -ForegroundColor Gray
    Write-Host "   3. Verify Redis and RabbitMQ are accessible" -ForegroundColor Gray
    Write-Host "   4. Check application logs for errors" -ForegroundColor Gray
} else {
    Write-Host "🎉 All services are healthy! Backend is ready for production." -ForegroundColor Green
    Write-Host "   You can now test the full API functionality through Swagger UIs." -ForegroundColor Gray
}

Write-Host "`n📋 CONFIGURATION CHECK" -ForegroundColor Cyan
Write-Host "======================" -ForegroundColor Cyan

# Check if configuration files exist
$configFiles = @(
    "backend/docker-compose.yml"
    "backend/src/Services/Product/ToyStore.ProductService.API/appsettings.json"
    "backend/src/Services/Order/ToyStore.OrderService/appsettings.json"
)

foreach ($configFile in $configFiles) {
    if (Test-Path $configFile) {
        Write-Host "✅ $configFile exists" -ForegroundColor Green
    } else {
        Write-Host "❌ $configFile missing" -ForegroundColor Red
    }
}

Write-Host "`nHealth check completed! 🏁" -ForegroundColor Cyan
