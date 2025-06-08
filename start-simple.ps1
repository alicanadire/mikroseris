# Simple ToyStore Starter
Write-Host "🎮 Starting ToyStore..." -ForegroundColor Cyan

# Docker kontrolü
if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Host "❌ Docker not found!" -ForegroundColor Red
    exit 1
}

# Backend başlat
Write-Host "🚀 Starting backend..." -ForegroundColor Yellow
cd backend
docker-compose up -d
cd ..

# Frontend başlat
Write-Host "🎨 Starting frontend..." -ForegroundColor Yellow
if (-not (Test-Path "node_modules")) {
    npm install
}
Start-Process "npm" -ArgumentList "run", "dev"

Write-Host "✅ ToyStore started!" -ForegroundColor Green
Write-Host "🌐 Frontend: http://localhost:3000" -ForegroundColor White
Write-Host "🔗 API: http://localhost:5000" -ForegroundColor White
