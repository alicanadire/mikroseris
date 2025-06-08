#!/usr/bin/env pwsh

Write-Host "🔗 ToyStore Backend - Connection Testing" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan

$connectionResults = @()

# Function to test SQL Server connection
function Test-SqlServerConnection {
    param($connectionString, $serviceName)
    
    Write-Host "`n🗃️ Testing SQL Server connection for $serviceName..." -ForegroundColor Yellow
    
    try {
        # Extract server info from connection string
        if ($connectionString -match "Server=([^;]+)") {
            $server = $matches[1]
            
            # Test basic TCP connectivity
            $tcpTest = Test-NetConnection -ComputerName "localhost" -Port 1433 -WarningAction SilentlyContinue
            
            if ($tcpTest.TcpTestSucceeded) {
                Write-Host "   ✅ $serviceName - TCP connection to SQL Server successful" -ForegroundColor Green
                $connectionResults += @{ Service = $serviceName; Type = "SQL Server"; Status = "Connected"; Details = "TCP port 1433 accessible" }
            } else {
                Write-Host "   ❌ $serviceName - Cannot connect to SQL Server port 1433" -ForegroundColor Red
                $connectionResults += @{ Service = $serviceName; Type = "SQL Server"; Status = "Failed"; Details = "Port 1433 not accessible" }
            }
        }
    }
    catch {
        Write-Host "   ❌ $serviceName - SQL Server connection error: $($_.Exception.Message)" -ForegroundColor Red
        $connectionResults += @{ Service = $serviceName; Type = "SQL Server"; Status = "Error"; Details = $_.Exception.Message }
    }
}

# Function to test PostgreSQL connection
function Test-PostgreSqlConnection {
    param($connectionString, $serviceName)
    
    Write-Host "`n🐘 Testing PostgreSQL connection for $serviceName..." -ForegroundColor Yellow
    
    try {
        # Test basic TCP connectivity
        $tcpTest = Test-NetConnection -ComputerName "localhost" -Port 5432 -WarningAction SilentlyContinue
        
        if ($tcpTest.TcpTestSucceeded) {
            Write-Host "   ✅ $serviceName - TCP connection to PostgreSQL successful" -ForegroundColor Green
            $connectionResults += @{ Service = $serviceName; Type = "PostgreSQL"; Status = "Connected"; Details = "TCP port 5432 accessible" }
        } else {
            Write-Host "   ❌ $serviceName - Cannot connect to PostgreSQL port 5432" -ForegroundColor Red
            $connectionResults += @{ Service = $serviceName; Type = "PostgreSQL"; Status = "Failed"; Details = "Port 5432 not accessible" }
        }
    }
    catch {
        Write-Host "   ❌ $serviceName - PostgreSQL connection error: $($_.Exception.Message)" -ForegroundColor Red
        $connectionResults += @{ Service = $serviceName; Type = "PostgreSQL"; Status = "Error"; Details = $_.Exception.Message }
    }
}

# Function to test Redis connection
function Test-RedisConnection {
    param($connectionString, $serviceName)
    
    Write-Host "`n📮 Testing Redis connection for $serviceName..." -ForegroundColor Yellow
    
    try {
        # Test basic TCP connectivity
        $tcpTest = Test-NetConnection -ComputerName "localhost" -Port 6379 -WarningAction SilentlyContinue
        
        if ($tcpTest.TcpTestSucceeded) {
            Write-Host "   ✅ $serviceName - TCP connection to Redis successful" -ForegroundColor Green
            $connectionResults += @{ Service = $serviceName; Type = "Redis"; Status = "Connected"; Details = "TCP port 6379 accessible" }
        } else {
            Write-Host "   ❌ $serviceName - Cannot connect to Redis port 6379" -ForegroundColor Red
            $connectionResults += @{ Service = $serviceName; Type = "Redis"; Status = "Failed"; Details = "Port 6379 not accessible" }
        }
    }
    catch {
        Write-Host "   ❌ $serviceName - Redis connection error: $($_.Exception.Message)" -ForegroundColor Red
        $connectionResults += @{ Service = $serviceName; Type = "Redis"; Status = "Error"; Details = $_.Exception.Message }
    }
}

# Function to test RabbitMQ connection
function Test-RabbitMqConnection {
    param($connectionString, $serviceName)
    
    Write-Host "`n🐰 Testing RabbitMQ connection for $serviceName..." -ForegroundColor Yellow
    
    try {
        # Test basic TCP connectivity
        $tcpTest = Test-NetConnection -ComputerName "localhost" -Port 5672 -WarningAction SilentlyContinue
        
        if ($tcpTest.TcpTestSucceeded) {
            Write-Host "   ✅ $serviceName - TCP connection to RabbitMQ successful" -ForegroundColor Green
            $connectionResults += @{ Service = $serviceName; Type = "RabbitMQ"; Status = "Connected"; Details = "TCP port 5672 accessible" }
        } else {
            Write-Host "   ❌ $serviceName - Cannot connect to RabbitMQ port 5672" -ForegroundColor Red
            $connectionResults += @{ Service = $serviceName; Type = "RabbitMQ"; Status = "Failed"; Details = "Port 5672 not accessible" }
        }
    }
    catch {
        Write-Host "   ❌ $serviceName - RabbitMQ connection error: $($_.Exception.Message)" -ForegroundColor Red
        $connectionResults += @{ Service = $serviceName; Type = "RabbitMQ"; Status = "Error"; Details = $_.Exception.Message }
    }
}

Write-Host "`n🔍 Testing Infrastructure Services..." -ForegroundColor Yellow

# Test SQL Server
Test-SqlServerConnection -connectionString "Server=localhost,1433;Database=ToyStoreProductDb;User Id=sa;Password=ToyStore123!;TrustServerCertificate=true;" -serviceName "Product Service"
Test-SqlServerConnection -connectionString "Server=localhost,1433;Database=ToyStoreOrderDb;User Id=sa;Password=ToyStore123!;TrustServerCertificate=true;" -serviceName "Order Service"
Test-SqlServerConnection -connectionString "Server=localhost,1433;Database=ToyStoreUserDb;User Id=sa;Password=ToyStore123!;TrustServerCertificate=true;" -serviceName "User Service"
Test-SqlServerConnection -connectionString "Server=localhost,1433;Database=ToyStoreIdentityDb;User Id=sa;Password=ToyStore123!;TrustServerCertificate=true;" -serviceName "Identity Service"

# Test PostgreSQL
Test-PostgreSqlConnection -connectionString "Host=localhost;Port=5432;Database=ToyStoreInventoryDb;Username=postgres;Password=ToyStore123!;" -serviceName "Inventory Service"

# Test Redis
Test-RedisConnection -connectionString "localhost:6379" -serviceName "All Services"

# Test RabbitMQ
Test-RabbitMqConnection -connectionString "amqp://guest:guest@localhost:5672/" -serviceName "All Services"

Write-Host "`n📊 CONNECTION SUMMARY" -ForegroundColor Cyan
Write-Host "======================" -ForegroundColor Cyan

$successfulConnections = ($connectionResults | Where-Object { $_.Status -eq "Connected" }).Count
$totalConnections = $connectionResults.Count

Write-Host "`n📈 Overall Status: $successfulConnections/$totalConnections connections successful" -ForegroundColor $(if ($successfulConnections -eq $totalConnections) { "Green" } else { "Yellow" })

Write-Host "`n✅ SUCCESSFUL CONNECTIONS:" -ForegroundColor Green
$connectionResults | Where-Object { $_.Status -eq "Connected" } | ForEach-Object {
    Write-Host "   $($_.Service) - $($_.Type): $($_.Details)" -ForegroundColor Green
}

$failedConnections = $connectionResults | Where-Object { $_.Status -ne "Connected" }
if ($failedConnections.Count -gt 0) {
    Write-Host "`n❌ FAILED CONNECTIONS:" -ForegroundColor Red
    $failedConnections | ForEach-Object {
        Write-Host "   $($_.Service) - $($_.Type): $($_.Details)" -ForegroundColor Red
    }
    
    Write-Host "`n🔧 TROUBLESHOOTING GUIDE:" -ForegroundColor Yellow
    Write-Host "=========================" -ForegroundColor Yellow
    
    if ($failedConnections | Where-Object { $_.Type -eq "SQL Server" }) {
        Write-Host "`n🗃️ SQL Server Issues:" -ForegroundColor Yellow
        Write-Host "   • Start SQL Server: docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=ToyStore123!' -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest" -ForegroundColor Blue
        Write-Host "   • Check if port 1433 is available: netstat -an | findstr 1433" -ForegroundColor Blue
        Write-Host "   • Verify password complexity requirements" -ForegroundColor Blue
    }
    
    if ($failedConnections | Where-Object { $_.Type -eq "PostgreSQL" }) {
        Write-Host "`n🐘 PostgreSQL Issues:" -ForegroundColor Yellow
        Write-Host "   • Start PostgreSQL: docker run -e POSTGRES_PASSWORD=ToyStore123! -p 5432:5432 -d postgres:15-alpine" -ForegroundColor Blue
        Write-Host "   • Check if port 5432 is available: netstat -an | findstr 5432" -ForegroundColor Blue
    }
    
    if ($failedConnections | Where-Object { $_.Type -eq "Redis" }) {
        Write-Host "`n📮 Redis Issues:" -ForegroundColor Yellow
        Write-Host "   • Start Redis: docker run -p 6379:6379 -d redis:7-alpine" -ForegroundColor Blue
        Write-Host "   • Check if port 6379 is available: netstat -an | findstr 6379" -ForegroundColor Blue
    }
    
    if ($failedConnections | Where-Object { $_.Type -eq "RabbitMQ" }) {
        Write-Host "`n🐰 RabbitMQ Issues:" -ForegroundColor Yellow
        Write-Host "   • Start RabbitMQ: docker run -p 5672:5672 -p 15672:15672 -d rabbitmq:3-management-alpine" -ForegroundColor Blue
        Write-Host "   • Check if port 5672 is available: netstat -an | findstr 5672" -ForegroundColor Blue
        Write-Host "   • Access management UI: http://localhost:15672 (guest/guest)" -ForegroundColor Blue
    }
} else {
    Write-Host "`n🎉 ALL CONNECTIONS SUCCESSFUL!" -ForegroundColor Green
    Write-Host "Infrastructure is ready for ToyStore services." -ForegroundColor Green
}

Write-Host "`n🚀 NEXT STEPS" -ForegroundColor Cyan
Write-Host "=============" -ForegroundColor Cyan

if ($successfulConnections -eq $totalConnections) {
    Write-Host "   1. ✅ All connections working - ready to start services" -ForegroundColor Green
    Write-Host "   2. 🚀 Start all services: docker-compose up --build" -ForegroundColor Green
    Write-Host "   3. 🧪 Test API endpoints: ./backend/test-backend-health.ps1" -ForegroundColor Green
} else {
    Write-Host "   1. 🔧 Fix failed connections using the troubleshooting guide above" -ForegroundColor Yellow
    Write-Host "   2. 🔁 Re-run this connection test" -ForegroundColor Yellow
    Write-Host "   3. 🚀 Start services once all connections are working" -ForegroundColor Yellow
}

Write-Host "`n💡 DOCKER QUICK START" -ForegroundColor Cyan
Write-Host "=====================" -ForegroundColor Cyan
Write-Host "Start all infrastructure at once:" -ForegroundColor Gray
Write-Host "docker-compose up -d sqlserver postgres redis rabbitmq" -ForegroundColor Blue

Write-Host "`nConnection testing completed! 🏁" -ForegroundColor Cyan

# Return exit code based on connection results
if ($successfulConnections -eq $totalConnections) {
    exit 0
} else {
    exit 1
}
