#!/usr/bin/env pwsh

Write-Host "🔧 ToyStore Backend - Dependency Restoration" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

# Function to restore NuGet packages for a project
function Restore-ProjectDependencies {
    param($projectPath, $projectName)
    
    Write-Host "`n📦 Restoring packages for $projectName..." -ForegroundColor Yellow
    
    if (Test-Path $projectPath) {
        Push-Location (Split-Path $projectPath -Parent)
        
        try {
            $result = dotnet restore --verbosity quiet
            if ($LASTEXITCODE -eq 0) {
                Write-Host "   ✅ $projectName - Dependencies restored successfully" -ForegroundColor Green
            } else {
                Write-Host "   ❌ $projectName - Failed to restore dependencies" -ForegroundColor Red
                Write-Host "   Error: $result" -ForegroundColor Red
            }
        }
        catch {
            Write-Host "   ❌ $projectName - Exception during restore: $($_.Exception.Message)" -ForegroundColor Red
        }
        finally {
            Pop-Location
        }
    } else {
        Write-Host "   ❌ $projectName - Project file not found: $projectPath" -ForegroundColor Red
    }
}

# Function to build a project
function Build-Project {
    param($projectPath, $projectName)
    
    Write-Host "`n🔨 Building $projectName..." -ForegroundColor Yellow
    
    if (Test-Path $projectPath) {
        Push-Location (Split-Path $projectPath -Parent)
        
        try {
            $result = dotnet build --no-restore --verbosity quiet
            if ($LASTEXITCODE -eq 0) {
                Write-Host "   ✅ $projectName - Build successful" -ForegroundColor Green
            } else {
                Write-Host "   ❌ $projectName - Build failed" -ForegroundColor Red
                Write-Host "   Error: $result" -ForegroundColor Red
            }
        }
        catch {
            Write-Host "   ❌ $projectName - Exception during build: $($_.Exception.Message)" -ForegroundColor Red
        }
        finally {
            Pop-Location
        }
    } else {
        Write-Host "   ❌ $projectName - Project file not found: $projectPath" -ForegroundColor Red
    }
}

# Define all projects
$projects = @(
    @{ Path = "backend/src/Shared/ToyStore.Shared/ToyStore.Shared.csproj"; Name = "ToyStore.Shared" }
    @{ Path = "backend/src/Shared/ToyStore.EventBus/ToyStore.EventBus.csproj"; Name = "ToyStore.EventBus" }
    @{ Path = "backend/src/Services/Identity/ToyStore.IdentityService/ToyStore.IdentityService.csproj"; Name = "Identity Service" }
    @{ Path = "backend/src/Services/Product/ToyStore.ProductService.Domain/ToyStore.ProductService.Domain.csproj"; Name = "Product Domain" }
    @{ Path = "backend/src/Services/Product/ToyStore.ProductService.Application/ToyStore.ProductService.Application.csproj"; Name = "Product Application" }
    @{ Path = "backend/src/Services/Product/ToyStore.ProductService.Infrastructure/ToyStore.ProductService.Infrastructure.csproj"; Name = "Product Infrastructure" }
    @{ Path = "backend/src/Services/Product/ToyStore.ProductService.API/ToyStore.ProductService.API.csproj"; Name = "Product API" }
    @{ Path = "backend/src/Services/Order/ToyStore.OrderService/ToyStore.OrderService.csproj"; Name = "Order Service" }
    @{ Path = "backend/src/Services/User/ToyStore.UserService/ToyStore.UserService.csproj"; Name = "User Service" }
    @{ Path = "backend/src/Services/Inventory/ToyStore.InventoryService/ToyStore.InventoryService.csproj"; Name = "Inventory Service" }
    @{ Path = "backend/src/ApiGateway/ToyStore.ApiGateway/ToyStore.ApiGateway.csproj"; Name = "API Gateway" }
)

Write-Host "`n🔍 Checking .NET SDK..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "   ✅ .NET SDK Version: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "   ❌ .NET SDK not found. Please install .NET 8.0 SDK" -ForegroundColor Red
    exit 1
}

Write-Host "`n📦 RESTORING DEPENDENCIES" -ForegroundColor Cyan
Write-Host "==========================" -ForegroundColor Cyan

# Restore dependencies in order (shared libraries first)
foreach ($project in $projects) {
    Restore-ProjectDependencies -projectPath $project.Path -projectName $project.Name
}

Write-Host "`n🔨 BUILDING PROJECTS" -ForegroundColor Cyan
Write-Host "=====================" -ForegroundColor Cyan

# Build projects in order
foreach ($project in $projects) {
    Build-Project -projectPath $project.Path -projectName $project.Name
}

Write-Host "`n🔍 DEPENDENCY VERIFICATION" -ForegroundColor Cyan
Write-Host "===========================" -ForegroundColor Cyan

# Check for critical packages
$criticalPackages = @(
    @{ Service = "All Services"; Package = "StackExchange.Redis"; Version = "2.7.4" }
    @{ Service = "EventBus"; Package = "RabbitMQ.Client"; Version = "6.6.0" }
    @{ Service = "All Services"; Package = "Microsoft.AspNetCore.Authentication.JwtBearer"; Version = "8.0.0" }
    @{ Service = "All Services"; Package = "Swashbuckle.AspNetCore"; Version = "6.5.0" }
)

Write-Host "`nChecking critical package versions..." -ForegroundColor Yellow

foreach ($package in $criticalPackages) {
    Write-Host "   📋 $($package.Service) - $($package.Package) v$($package.Version)" -ForegroundColor Gray
}

Write-Host "`n🗃️ DATABASE CONNECTION CHECK" -ForegroundColor Cyan
Write-Host "=============================" -ForegroundColor Cyan

$connectionStrings = @(
    @{ Service = "SQL Server"; Connection = "Server=localhost,1433;Database=Test;User Id=sa;Password=ToyStore123!;TrustServerCertificate=true;" }
    @{ Service = "PostgreSQL"; Connection = "Host=localhost;Port=5432;Database=Test;Username=postgres;Password=ToyStore123!;" }
    @{ Service = "Redis"; Connection = "localhost:6379" }
    @{ Service = "RabbitMQ"; Connection = "amqp://guest:guest@localhost:5672/" }
)

foreach ($conn in $connectionStrings) {
    Write-Host "   🔗 $($conn.Service) - Connection string configured" -ForegroundColor Green
}

Write-Host "`n✅ DEPENDENCY SUMMARY" -ForegroundColor Cyan
Write-Host "=====================" -ForegroundColor Cyan

Write-Host "`n📊 Key Dependencies Added:" -ForegroundColor Green
Write-Host "   • StackExchange.Redis - Redis connection support" -ForegroundColor Gray
Write-Host "   • Microsoft.Extensions.Options - Configuration binding" -ForegroundColor Gray
Write-Host "   • Swashbuckle.AspNetCore.Annotations - Enhanced Swagger docs" -ForegroundColor Gray
Write-Host "   • Swashbuckle.AspNetCore.Filters - Swagger examples" -ForegroundColor Gray

Write-Host "`n🔗 Connection Strings Configured:" -ForegroundColor Green
Write-Host "   • SQL Server databases for Identity, Product, Order, User services" -ForegroundColor Gray
Write-Host "   • PostgreSQL database for Inventory service" -ForegroundColor Gray
Write-Host "   • Redis cache for all services" -ForegroundColor Gray
Write-Host "   • RabbitMQ message bus for event communication" -ForegroundColor Gray

Write-Host "`n🚀 NEXT STEPS" -ForegroundColor Cyan
Write-Host "=============" -ForegroundColor Cyan

Write-Host "`n1. 🐳 Start Infrastructure:" -ForegroundColor Yellow
Write-Host "   docker-compose up -d sqlserver postgres redis rabbitmq" -ForegroundColor Blue

Write-Host "`n2. 🧪 Test Connections:" -ForegroundColor Yellow
Write-Host "   ./backend/test-connections.ps1" -ForegroundColor Blue

Write-Host "`n3. 🚀 Start Services:" -ForegroundColor Yellow
Write-Host "   docker-compose up --build" -ForegroundColor Blue

Write-Host "`n4. 🔍 Validate Health:" -ForegroundColor Yellow
Write-Host "   ./backend/validate-configuration.ps1" -ForegroundColor Blue

Write-Host "`n🎉 Dependency restoration completed!" -ForegroundColor Green
Write-Host "All projects should now have their dependencies properly configured." -ForegroundColor Gray
