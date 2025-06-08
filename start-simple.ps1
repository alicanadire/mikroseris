# ToyStore Simple Starter - PowerShell
# Basit ve güvenli başlatma scripti

param(
    [switch]$Frontend
)

Write-Host "🎮 ToyStore Microservices Starter" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan
Write-Host ""

# Docker kontrolü
Write-Host "Checking Docker..." -ForegroundColor Yellow
$dockerRunning = $false
try {
    $dockerInfo = docker info 2>$null
    if ($LASTEXITCODE -eq 0) {
        $dockerRunning = $true
        Write-Host "✓ Docker is running" -ForegroundColor Green
    }
}
catch {
    Write-Host "✗ Docker check failed" -ForegroundColor Red
}

if (-not $dockerRunning) {
    Write-Host "✗ Docker is not running. Please start Docker Desktop first!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Steps to fix:" -ForegroundColor Yellow
    Write-Host "1. Open Docker Desktop" -ForegroundColor White
    Write-Host "2. Wait for it to say 'Engine running'" -ForegroundColor White
    Write-Host "3. Run this script again" -ForegroundColor White
    exit 1
}

# Docker Compose dosya kontrolü
Write-Host "Checking files..." -ForegroundColor Yellow
if (-not (Test-Path "backend")) {
    Write-Host "✗ Backend folder not found!" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path "backend\docker-compose-full.yml")) {
    Write-Host "✗ Docker Compose file not found!" -ForegroundColor Red
    exit 1
}

Write-Host "✓ All files found" -ForegroundColor Green
Write-Host ""

# Ana klasörü kaydet
$startLocation = Get-Location

# Backend klasörüne geç
Set-Location backend

Write-Host "🚀 Starting ToyStore services..." -ForegroundColor Yellow
Write-Host ""

if ($Frontend) {
    Write-Host "Starting ALL services (including frontend)..." -ForegroundColor Cyan
    docker-compose -f docker-compose-full.yml up -d
}
else {
    Write-Host "Starting BACKEND services only..." -ForegroundColor Cyan
    docker-compose -f docker-compose-full.yml up -d sqlserver postgresql mongodb redis rabbitmq identityservice productservice orderservice userservice inventoryservice notificationservice apigateway adminer redis-commander
}

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "⏳ Waiting for services to initialize..." -ForegroundColor Yellow
    Start-Sleep -Seconds 30
    
    Write-Host ""
    Write-Host "📊 Checking service status:" -ForegroundColor Cyan
    docker-compose -f docker-compose-full.yml ps
    
    Write-Host ""
    Write-Host "🎉 ToyStore is starting up!" -ForegroundColor Green
    Write-Host ""
    Write-Host "📱 Available services:" -ForegroundColor Yellow
    Write-Host "  🌐 API Gateway:       http://localhost:5000" -ForegroundColor White
    Write-Host "  📝 Swagger Docs:      http://localhost:5001/swagger" -ForegroundColor White
    Write-Host "  🐰 RabbitMQ:          http://localhost:15672" -ForegroundColor White
    Write-Host "  🗄️  Database Admin:    http://localhost:8080" -ForegroundColor White
    Write-Host "  ⚡ Redis Admin:       http://localhost:8081" -ForegroundColor White
    
    if ($Frontend) {
        Write-Host "  🎨 Frontend:          http://localhost:3000" -ForegroundColor White
    }
    else {
        Write-Host ""
        Write-Host "💡 To start frontend:" -ForegroundColor Cyan
        Write-Host "   npm install && npm run dev" -ForegroundColor Gray
    }
    
    Write-Host ""
    Write-Host "🔑 Login credentials:" -ForegroundColor Yellow
    Write-Host "  Admin:     admin@toystore.com / Admin123!" -ForegroundColor White
    Write-Host "  Customer:  customer@toystore.com / Customer123!" -ForegroundColor White
    Write-Host "  RabbitMQ:  admin / ToyStore123!" -ForegroundColor White
    
    Write-Host ""
    Write-Host "🔧 Useful commands:" -ForegroundColor Yellow
    Write-Host "  View logs:    docker-compose -f docker-compose-full.yml logs -f" -ForegroundColor Gray
    Write-Host "  Stop all:     docker-compose -f docker-compose-full.yml down" -ForegroundColor Gray
    Write-Host "  Restart:      docker-compose -f docker-compose-full.yml restart" -ForegroundColor Gray
}
else {
    Write-Host ""
    Write-Host "❌ Failed to start services!" -ForegroundColor Red
    Write-Host "Check the error messages above." -ForegroundColor Yellow
}

# Orijinal klasöre dön
Set-Location $startLocation

Write-Host ""
Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
