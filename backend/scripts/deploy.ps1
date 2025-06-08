# ToyStore Microservices PowerShell Deployment Script
# Windows PowerShell için tam Docker Compose kurulumu

param(
    [string]$Environment = "development",
    [string]$Action = "up"
)

# Renkli çıktı için fonksiyonlar
function Write-Info($message) {
    Write-Host "[INFO] $message" -ForegroundColor Blue
}

function Write-Success($message) {
    Write-Host "[SUCCESS] $message" -ForegroundColor Green
}

function Write-Warning($message) {
    Write-Host "[WARNING] $message" -ForegroundColor Yellow
}

function Write-Error($message) {
    Write-Host "[ERROR] $message" -ForegroundColor Red
}

function Test-DockerRunning {
    try {
        $null = docker info 2>$null
        return $true
    }
    catch {
        return $false
    }
}

function Wait-ForService($serviceName, $url, $maxAttempts = 30) {
    Write-Info "Waiting for $serviceName to be ready..."
    
    for ($i = 1; $i -le $maxAttempts; $i++) {
        try {
            $response = Invoke-WebRequest -Uri $url -UseBasicParsing -TimeoutSec 5 -ErrorAction Stop
            if ($response.StatusCode -eq 200) {
                Write-Success "$serviceName is ready!"
                return $true
            }
        }
        catch {
            Write-Host "." -NoNewline
            Start-Sleep -Seconds 2
        }
    }
    
    Write-Error "$serviceName failed to start within expected time"
    return $false
}

# Ana script başlangıcı
Write-Host ""
Write-Host "🚀 ToyStore Microservices PowerShell Deployment" -ForegroundColor Cyan
Write-Host "=================================================" -ForegroundColor Cyan

# Docker kontrolü
if (-not (Test-DockerRunning)) {
    Write-Error "Docker is not running. Please start Docker Desktop and try again."
    exit 1
}

Write-Success "Docker environment validated ✓"

# Gerekli klasörleri oluştur
Write-Info "Creating required directories..."
$directories = @("logs", "data\sqlserver", "data\postgresql", "data\mongodb", "data\redis", "data\rabbitmq")
foreach ($dir in $directories) {
    if (-not (Test-Path $dir)) {
        New-Item -ItemType Directory -Path $dir -Force | Out-Null
    }
}

switch ($Action.ToLower()) {
    "up" {
        Write-Info "Starting ToyStore microservices in $Environment mode..."
        
        if ($Environment -eq "development") {
            # Development mode - altyapı servislerini önce başlat
            Write-Info "Starting infrastructure services..."
            docker-compose up -d sqlserver postgresql mongodb redis rabbitmq
            
            # Veritabanlarının hazır olmasını bekle
            Write-Info "Waiting for databases to initialize (30 seconds)..."
            Start-Sleep -Seconds 30
            
            # Identity Service'i önce başlat
            Write-Info "Starting Identity Service..."
            docker-compose up -d identityservice
            
            # Identity Service'in hazır olmasını bekle
            $null = Wait-ForService "Identity Service" "http://localhost:5004/health"
            
            # Diğer servisleri başlat
            Write-Info "Starting application services..."
            docker-compose up -d productservice orderservice userservice inventoryservice notificationservice
            
            # Servislerin hazır olmasını bekle
            Start-Sleep -Seconds 15
            
            # API Gateway'i en son başlat
            Write-Info "Starting API Gateway..."
            docker-compose up -d apigateway
            
        } else {
            # Production mode - tüm servisleri başlat
            Write-Info "Starting all services in production mode..."
            docker-compose -f docker-compose.yml up -d
        }
        
        # API Gateway'in hazır olmasını bekle
        $null = Wait-ForService "API Gateway" "http://localhost:5000/health"
        
        Write-Success "🎉 ToyStore microservices deployment completed!"
        Write-Info "Services are available at:"
        Write-Host "  📱 API Gateway: http://localhost:5000" -ForegroundColor White
        Write-Host "  🔐 Identity Server: http://localhost:5004" -ForegroundColor White
        Write-Host "  🛍️  Product Service: http://localhost:5001" -ForegroundColor White
        Write-Host "  📦 Order Service: http://localhost:5002" -ForegroundColor White
        Write-Host "  👤 User Service: http://localhost:5003" -ForegroundColor White
        Write-Host "  📊 Inventory Service: http://localhost:5005" -ForegroundColor White
        Write-Host "  📧 Notification Service: http://localhost:5006" -ForegroundColor White
        Write-Host "  🐰 RabbitMQ Management: http://localhost:15672 (admin/ToyStore123!)" -ForegroundColor White
        Write-Host ""
        Write-Info "Health checks:"
        Write-Host "  Invoke-WebRequest http://localhost:5000/health" -ForegroundColor Gray
        Write-Host "  Invoke-WebRequest http://localhost:5001/health" -ForegroundColor Gray
    }
    
    "down" {
        Write-Info "Stopping ToyStore microservices..."
        docker-compose down
        Write-Success "All services stopped."
    }
    
    "restart" {
        Write-Info "Restarting ToyStore microservices..."
        docker-compose restart
        Write-Success "All services restarted."
    }
    
    "logs" {
        if ($args.Count -gt 2) {
            $service = $args[2]
            docker-compose logs -f $service
        } else {
            docker-compose logs -f
        }
    }
    
    "status" {
        Write-Info "Service status:"
        docker-compose ps
    }
    
    "clean" {
        $confirmation = Read-Host "This will remove all containers, volumes, and images. Are you sure? (y/N)"
        if ($confirmation -match "^[yY]") {
            Write-Info "Cleaning up Docker environment..."
            docker-compose down -v --remove-orphans
            docker system prune -f
            Write-Success "Cleanup completed."
        } else {
            Write-Info "Cleanup cancelled."
        }
    }
    
    "build" {
        Write-Info "Building all Docker images..."
        docker-compose build
        Write-Success "Build completed."
    }
    
    "pull" {
        Write-Info "Pulling latest images..."
        docker-compose pull
        Write-Success "Pull completed."
    }
    
    default {
        Write-Host "ToyStore Microservices PowerShell Deployment Script" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "Usage: .\deploy.ps1 [-Environment <env>] [-Action <action>]" -ForegroundColor White
        Write-Host ""
        Write-Host "Environments:" -ForegroundColor Yellow
        Write-Host "  development    Development mode (default)" -ForegroundColor White
        Write-Host "  production     Production mode" -ForegroundColor White
        Write-Host ""
        Write-Host "Actions:" -ForegroundColor Yellow
        Write-Host "  up             Start all services (default)" -ForegroundColor White
        Write-Host "  down           Stop all services" -ForegroundColor White
        Write-Host "  restart        Restart all services" -ForegroundColor White
        Write-Host "  logs           View logs" -ForegroundColor White
        Write-Host "  status         Show service status" -ForegroundColor White
        Write-Host "  clean          Remove all containers and volumes" -ForegroundColor White
        Write-Host "  build          Build all Docker images" -ForegroundColor White
        Write-Host "  pull           Pull latest images" -ForegroundColor White
        Write-Host ""
        Write-Host "Examples:" -ForegroundColor Yellow
        Write-Host "  .\deploy.ps1                                    # Start in development mode" -ForegroundColor Gray
        Write-Host "  .\deploy.ps1 -Environment production -Action up # Start in production mode" -ForegroundColor Gray
        Write-Host "  .\deploy.ps1 -Action logs                       # View all logs" -ForegroundColor Gray
        Write-Host "  .\deploy.ps1 -Action down                       # Stop all services" -ForegroundColor Gray
    }
}
