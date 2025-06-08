# ToyStore Quick Start Script for PowerShell
# Bu script her şeyi otomatik olarak başlatır

param(
    [switch]$IncludeFrontend = $false,
    [switch]$ShowLogs = $false
)

Write-Host ""
Write-Host "🎮 ToyStore Microservices Quick Start" -ForegroundColor Cyan
Write-Host "====================================" -ForegroundColor Cyan

# Docker kontrolü
try {
    $null = docker info 2>$null
    Write-Host "✓ Docker is running" -ForegroundColor Green
}
catch {
    Write-Host "✗ Docker is not running. Please start Docker Desktop first!" -ForegroundColor Red
    exit 1
}

# Docker Compose dosya kontrolü
$dockerComposeFile = "backend\docker-compose-full.yml"
if (-not (Test-Path $dockerComposeFile)) {
    Write-Host "✗ Docker Compose file not found: $dockerComposeFile" -ForegroundColor Red
    exit 1
}

Write-Host "✓ Docker Compose file found" -ForegroundColor Green

# Backend servislerini başlat
Write-Host ""
Write-Host "🚀 Starting all backend services..." -ForegroundColor Yellow

$originalLocation = Get-Location
Set-Location backend

try {
    # Tüm servisleri başlat
    if ($IncludeFrontend) {
        Write-Host "Starting all services including frontend..." -ForegroundColor Yellow
        docker-compose -f docker-compose-full.yml up -d
        Write-Host "✓ All services (including frontend) started" -ForegroundColor Green
    } 
    else {
        Write-Host "Starting backend services only..." -ForegroundColor Yellow
        # Sadece backend servisleri (frontend hariç)
        docker-compose -f docker-compose-full.yml up -d --scale frontend=0
        Write-Host "✓ Backend services started" -ForegroundColor Green
    }
    
    Write-Host ""
    Write-Host "⏳ Waiting for services to be ready (this may take a few minutes)..." -ForegroundColor Yellow
    Start-Sleep -Seconds 45
    
    # Servis durumunu kontrol et
    Write-Host ""
    Write-Host "📊 Service Status:" -ForegroundColor Cyan
    docker-compose -f docker-compose-full.yml ps
    
    Write-Host ""
    Write-Host "🎉 ToyStore is ready!" -ForegroundColor Green
    Write-Host ""
    Write-Host "📱 Access Points:" -ForegroundColor Yellow
    Write-Host "  • API Gateway:        http://localhost:5000" -ForegroundColor White
    Write-Host "  • Swagger UI:         http://localhost:5001/swagger" -ForegroundColor White
    Write-Host "  • RabbitMQ Admin:     http://localhost:15672 (admin/ToyStore123!)" -ForegroundColor White
    Write-Host "  • Database Admin:     http://localhost:8080" -ForegroundColor White
    Write-Host "  • Redis Admin:        http://localhost:8081" -ForegroundColor White
    
    if ($IncludeFrontend) {
        Write-Host "  • Frontend:           http://localhost:3000" -ForegroundColor White
    } 
    else {
        Write-Host ""
        Write-Host "💡 To start frontend separately:" -ForegroundColor Cyan
        Write-Host "   cd .." -ForegroundColor Gray
        Write-Host "   npm install && npm run dev" -ForegroundColor Gray
    }
    
    Write-Host ""
    Write-Host "🔧 Management Commands:" -ForegroundColor Yellow
    Write-Host "  • View logs:          docker-compose -f docker-compose-full.yml logs -f" -ForegroundColor Gray
    Write-Host "  • Stop services:      docker-compose -f docker-compose-full.yml down" -ForegroundColor Gray
    Write-Host "  • Restart services:   docker-compose -f docker-compose-full.yml restart" -ForegroundColor Gray
    
    # Logları göster
    if ($ShowLogs) {
        Write-Host ""
        Write-Host "📝 Showing logs (Ctrl+C to exit)..." -ForegroundColor Yellow
        docker-compose -f docker-compose-full.yml logs -f
    }
}
catch {
    Write-Host "✗ Error starting services: $_" -ForegroundColor Red
    exit 1
}
finally {
    Set-Location $originalLocation
}
