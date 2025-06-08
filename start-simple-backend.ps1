# Simple and Reliable ToyStore Backend Starter
param(
    [switch]$UseSimple = $true,
    [switch]$ShowLogs = $false
)

Write-Host ""
Write-Host "🎮 ToyStore Backend Simple Start" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan

# Docker kontrolü
try {
    $null = docker info 2>$null
    Write-Host "✓ Docker is running" -ForegroundColor Green
}
catch {
    Write-Host "✗ Docker is not running. Please start Docker Desktop first!" -ForegroundColor Red
    Write-Host "  Then press any key to continue..." -ForegroundColor Yellow
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit 1
}

$originalLocation = Get-Location

try {
    # Backend dizinine git
    Set-Location backend
    
    if ($UseSimple) {
        Write-Host "🚀 Starting essential services only..." -ForegroundColor Yellow
        Write-Host "  (SQL Server, Redis, RabbitMQ, PostgreSQL)" -ForegroundColor Gray
        
        # Basit setup ile başlat
        docker-compose -f docker-compose-simple.yml up -d
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ Essential services started successfully" -ForegroundColor Green
        } else {
            throw "Docker Compose failed with exit code $LASTEXITCODE"
        }
    } else {
        Write-Host "🚀 Starting all services..." -ForegroundColor Yellow
        docker-compose -f docker-compose.yml up -d
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ All services started successfully" -ForegroundColor Green
        } else {
            throw "Docker Compose failed with exit code $LASTEXITCODE"
        }
    }
    
    Write-Host ""
    Write-Host "⏳ Waiting for services to initialize..." -ForegroundColor Yellow
    
    # Servis durumunu kontrol et
    Start-Sleep -Seconds 30
    
    Write-Host ""
    Write-Host "📊 Service Status:" -ForegroundColor Cyan
    if ($UseSimple) {
        docker-compose -f docker-compose-simple.yml ps
    } else {
        docker-compose -f docker-compose.yml ps
    }
    
    Write-Host ""
    Write-Host "🎉 ToyStore backend is ready!" -ForegroundColor Green
    Write-Host ""
    Write-Host "📱 Available Services:" -ForegroundColor Yellow
    Write-Host "  • SQL Server:         localhost:1433" -ForegroundColor White
    Write-Host "  • PostgreSQL:         localhost:5432" -ForegroundColor White
    Write-Host "  • Redis:              localhost:6379" -ForegroundColor White
    Write-Host "  • RabbitMQ Management: http://localhost:15672" -ForegroundColor White
    Write-Host "    Username: admin, Password: ToyStore123!" -ForegroundColor Gray
    
    if (-not $UseSimple) {
        Write-Host "  • API Gateway:        http://localhost:5000" -ForegroundColor White
        Write-Host "  • Identity Service:   http://localhost:5004" -ForegroundColor White
    } else {
        Write-Host ""
        Write-Host "💡 To start .NET services:" -ForegroundColor Cyan
        Write-Host "   Use full docker-compose.yml or run services manually" -ForegroundColor Gray
    }
    
    Write-Host ""
    Write-Host "🔧 Management Commands:" -ForegroundColor Yellow
    if ($UseSimple) {
        Write-Host "  • View logs:          docker-compose -f docker-compose-simple.yml logs -f" -ForegroundColor Gray
        Write-Host "  • Stop services:      docker-compose -f docker-compose-simple.yml down" -ForegroundColor Gray
        Write-Host "  • Restart services:   docker-compose -f docker-compose-simple.yml restart" -ForegroundColor Gray
    } else {
        Write-Host "  • View logs:          docker-compose logs -f" -ForegroundColor Gray
        Write-Host "  • Stop services:      docker-compose down" -ForegroundColor Gray
        Write-Host "  • Restart services:   docker-compose restart" -ForegroundColor Gray
    }
    
    # Logları göster
    if ($ShowLogs) {
        Write-Host ""
        Write-Host "📝 Showing logs (Ctrl+C to exit)..." -ForegroundColor Yellow
        if ($UseSimple) {
            docker-compose -f docker-compose-simple.yml logs -f
        } else {
            docker-compose logs -f
        }
    }
}
catch {
    Write-Host ""
    Write-Host "✗ Error: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "🔧 Troubleshooting:" -ForegroundColor Yellow
    Write-Host "  1. Make sure Docker Desktop is running" -ForegroundColor Gray
    Write-Host "  2. Check if ports are available (1433, 5432, 6379, 15672)" -ForegroundColor Gray
    Write-Host "  3. Try: docker system prune -f" -ForegroundColor Gray
    Write-Host "  4. Restart Docker Desktop" -ForegroundColor Gray
    exit 1
}
finally {
    Set-Location $originalLocation
}
