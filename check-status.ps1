# ToyStore Status Check Script
param(
    [switch]$Detailed = $false
)

Write-Host ""
Write-Host "🎮 ToyStore Status Check" -ForegroundColor Cyan
Write-Host "========================" -ForegroundColor Cyan

# Check if we're in the right directory
if (-not (Test-Path "package.json")) {
    Write-Host "❌ Not in ToyStore root directory!" -ForegroundColor Red
    Write-Host "   Please run this script from the ToyStore project root." -ForegroundColor Yellow
    exit 1
}

# Frontend Status
Write-Host ""
Write-Host "🎨 Frontend Status:" -ForegroundColor Yellow

try {
    $frontendResponse = Invoke-WebRequest -Uri "http://localhost:8080" -TimeoutSec 5 -UseBasicParsing
    if ($frontendResponse.StatusCode -eq 200) {
        Write-Host "✅ Frontend: Running at http://localhost:8080" -ForegroundColor Green
        
        # Check if it's showing the loading page or actual content
        if ($frontendResponse.Content -like "*Generating your app*") {
            Write-Host "⚠️  Still showing loading page - may need restart" -ForegroundColor Yellow
        } else {
            Write-Host "✅ Frontend: Showing ToyStore content" -ForegroundColor Green
        }
    }
} catch {
    Write-Host "❌ Frontend: Not accessible at http://localhost:8080" -ForegroundColor Red
    Write-Host "   Try restarting the dev server" -ForegroundColor Gray
}

# Docker Status
Write-Host ""
Write-Host "🐳 Docker Status:" -ForegroundColor Yellow

try {
    $dockerInfo = docker info 2>$null
    Write-Host "✅ Docker: Running" -ForegroundColor Green
    
    # Check Docker containers
    $containers = docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}" 2>$null
    if ($containers) {
        Write-Host "✅ Docker Containers:" -ForegroundColor Green
        $containers | ForEach-Object { 
            if ($_ -notlike "*NAMES*") {
                Write-Host "   $_" -ForegroundColor White
            }
        }
    } else {
        Write-Host "⚠️  No containers running" -ForegroundColor Yellow
        Write-Host "   Run: .\start-simple-backend.ps1" -ForegroundColor Gray
    }
} catch {
    Write-Host "❌ Docker: Not running" -ForegroundColor Red
    Write-Host "   Please start Docker Desktop" -ForegroundColor Gray
}

# Backend Services Status
Write-Host ""
Write-Host "⚙️ Backend Services:" -ForegroundColor Yellow

$services = @(
    @{ Name = "SQL Server"; Port = 1433; Container = "toystore-sqlserver" },
    @{ Name = "PostgreSQL"; Port = 5432; Container = "toystore-postgresql" },
    @{ Name = "Redis"; Port = 6379; Container = "toystore-redis" },
    @{ Name = "RabbitMQ"; Port = 15672; Container = "toystore-rabbitmq"; Url = "http://localhost:15672" },
    @{ Name = "Adminer"; Port = 8080; Container = "toystore-adminer"; Url = "http://localhost:8080" },
    @{ Name = "Redis Admin"; Port = 8081; Container = "toystore-redis-admin"; Url = "http://localhost:8081" }
)

foreach ($service in $services) {
    try {
        if ($service.Url) {
            $response = Invoke-WebRequest -Uri $service.Url -TimeoutSec 3 -UseBasicParsing
            Write-Host "✅ $($service.Name): Available at $($service.Url)" -ForegroundColor Green
        } else {
            $connection = Test-NetConnection -ComputerName localhost -Port $service.Port -WarningAction SilentlyContinue
            if ($connection.TcpTestSucceeded) {
                Write-Host "✅ $($service.Name): Port $($service.Port) open" -ForegroundColor Green
            } else {
                Write-Host "❌ $($service.Name): Port $($service.Port) not accessible" -ForegroundColor Red
            }
        }
    } catch {
        Write-Host "❌ $($service.Name): Not accessible" -ForegroundColor Red
        
        # Check if container is running but not responding
        $containerStatus = docker ps --filter "name=$($service.Container)" --format "{{.Status}}" 2>$null
        if ($containerStatus) {
            Write-Host "   Container running but not responding: $containerStatus" -ForegroundColor Yellow
        }
    }
}

# System Resources
if ($Detailed) {
    Write-Host ""
    Write-Host "🖥️ System Resources:" -ForegroundColor Yellow
    
    try {
        $memory = Get-WmiObject -Class Win32_OperatingSystem
        $totalMemory = [math]::Round($memory.TotalVisibleMemorySize / 1MB, 2)
        $freeMemory = [math]::Round($memory.FreePhysicalMemory / 1MB, 2)
        $usedMemory = $totalMemory - $freeMemory
        
        Write-Host "   Total Memory: $totalMemory GB" -ForegroundColor White
        Write-Host "   Used Memory:  $usedMemory GB" -ForegroundColor White
        Write-Host "   Free Memory:  $freeMemory GB" -ForegroundColor White
        
        if ($freeMemory -lt 2) {
            Write-Host "⚠️  Low memory - may affect Docker performance" -ForegroundColor Yellow
        }
    } catch {
        Write-Host "   Could not retrieve memory information" -ForegroundColor Gray
    }
    
    # Docker stats
    try {
        $dockerStats = docker stats --no-stream --format "table {{.Container}}\t{{.CPUPerc}}\t{{.MemUsage}}" 2>$null
        if ($dockerStats) {
            Write-Host ""
            Write-Host "📊 Docker Container Stats:" -ForegroundColor Yellow
            $dockerStats | ForEach-Object { 
                if ($_ -notlike "*CONTAINER*") {
                    Write-Host "   $_" -ForegroundColor White
                }
            }
        }
    } catch {
        Write-Host "   Could not retrieve Docker stats" -ForegroundColor Gray
    }
}

# Quick Actions
Write-Host ""
Write-Host "🚀 Quick Actions:" -ForegroundColor Yellow
Write-Host "   Frontend only:    Navigate to http://localhost:8080" -ForegroundColor White
Write-Host "   Start backend:    .\start-simple-backend.ps1" -ForegroundColor White
Write-Host "   Full setup:       .\start-toystore.ps1" -ForegroundColor White
Write-Host "   Detailed status:  .\check-status.ps1 -Detailed" -ForegroundColor White
Write-Host "   Troubleshooting:  See TROUBLESHOOTING.md" -ForegroundColor White

# Summary
Write-Host ""
$frontendOk = $false
$backendOk = $false

try {
    $frontendTest = Invoke-WebRequest -Uri "http://localhost:8080" -TimeoutSec 3 -UseBasicParsing
    $frontendOk = $frontendTest.StatusCode -eq 200
} catch { }

try {
    $backendTest = docker ps --filter "name=toystore" --format "{{.Names}}" 2>$null
    $backendOk = $backendTest.Count -gt 0
} catch { }

if ($frontendOk -and $backendOk) {
    Write-Host "🎉 ToyStore Status: FULLY OPERATIONAL" -ForegroundColor Green
} elseif ($frontendOk) {
    Write-Host "⚠️  ToyStore Status: FRONTEND ONLY" -ForegroundColor Yellow
    Write-Host "   Backend services not running - using mock data" -ForegroundColor Gray
} elseif ($backendOk) {
    Write-Host "⚠️  ToyStore Status: BACKEND ONLY" -ForegroundColor Yellow
    Write-Host "   Frontend not accessible" -ForegroundColor Gray
} else {
    Write-Host "❌ ToyStore Status: NOT RUNNING" -ForegroundColor Red
    Write-Host "   Start with: .\start-simple-backend.ps1" -ForegroundColor Gray
}

Write-Host ""
