# Minimal ToyStore Backend Starter - Only Essential Services
Write-Host ""
Write-Host "🎮 ToyStore Minimal Backend Start" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan

# Docker kontrolü
try {
    $null = docker info 2>$null
    Write-Host "✓ Docker is running" -ForegroundColor Green
}
catch {
    Write-Host "✗ Docker is not running. Please start Docker Desktop first!" -ForegroundColor Red
    exit 1
}

$originalLocation = Get-Location

try {
    Set-Location backend
    
    Write-Host "🚀 Starting minimal infrastructure..." -ForegroundColor Yellow
    Write-Host "  (SQL Server, Redis, RabbitMQ only)" -ForegroundColor Gray
    
    # Create a minimal docker-compose for just the databases
    $minimalCompose = @"
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: toystore-sqlserver-min
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=ToyStore123!
      - MSSQL_PID=Express
    ports:
      - "1433:1433"
    volumes:
      - sqlserver_data_min:/var/opt/mssql
    networks:
      - toystore-network-min
    restart: unless-stopped
    healthcheck:
      test: ["CMD-SHELL", "/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P ToyStore123! -Q 'SELECT 1'"]
      interval: 30s
      timeout: 10s
      retries: 5

  redis:
    image: redis:7.2-alpine
    container_name: toystore-redis-min
    command: redis-server --requirepass ToyStore123! --appendonly yes
    ports:
      - "6379:6379"
    volumes:
      - redis_data_min:/data
    networks:
      - toystore-network-min
    restart: unless-stopped

  rabbitmq:
    image: rabbitmq:3.12-management
    container_name: toystore-rabbitmq-min
    environment:
      - RABBITMQ_DEFAULT_USER=admin
      - RABBITMQ_DEFAULT_PASS=ToyStore123!
    ports:
      - "5672:5672"
      - "15672:15672"
    volumes:
      - rabbitmq_data_min:/var/lib/rabbitmq
    networks:
      - toystore-network-min
    restart: unless-stopped

volumes:
  sqlserver_data_min:
  redis_data_min:
  rabbitmq_data_min:

networks:
  toystore-network-min:
    driver: bridge
"@
    
    $minimalCompose | Out-File -FilePath "docker-compose-minimal.yml" -Encoding UTF8
    
    # Start minimal services
    docker-compose -f docker-compose-minimal.yml up -d
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Minimal services started successfully" -ForegroundColor Green
        
        Write-Host ""
        Write-Host "⏳ Waiting for services to initialize..." -ForegroundColor Yellow
        Start-Sleep -Seconds 30
        
        Write-Host ""
        Write-Host "📊 Service Status:" -ForegroundColor Cyan
        docker-compose -f docker-compose-minimal.yml ps
        
        Write-Host ""
        Write-Host "🎉 Minimal backend is ready!" -ForegroundColor Green
        Write-Host ""
        Write-Host "📱 Available Services:" -ForegroundColor Yellow
        Write-Host "  • SQL Server:         localhost:1433" -ForegroundColor White
        Write-Host "  • Redis:              localhost:6379" -ForegroundColor White  
        Write-Host "  • RabbitMQ Management: http://localhost:15672" -ForegroundColor White
        Write-Host "    Username: admin, Password: ToyStore123!" -ForegroundColor Gray
        
        Write-Host ""
        Write-Host "🎨 Frontend: http://localhost:8080" -ForegroundColor Green
        Write-Host ""
        Write-Host "🔧 To stop services:" -ForegroundColor Yellow
        Write-Host "   docker-compose -f docker-compose-minimal.yml down" -ForegroundColor Gray
    } else {
        throw "Docker Compose failed"
    }
}
catch {
    Write-Host ""
    Write-Host "✗ Error: $_" -ForegroundColor Red
    exit 1
}
finally {
    Set-Location $originalLocation
}
