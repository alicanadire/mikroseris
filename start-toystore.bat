@echo off
echo.
echo =========================================
echo   🎮 ToyStore Microservices Quick Start
echo =========================================
echo.

REM Docker kontrolü
docker info >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo ✗ Docker is not running. Please start Docker Desktop first!
    pause
    exit /b 1
)

echo ✓ Docker is running

REM Docker Compose dosya kontrolü
if not exist "backend\docker-compose-full.yml" (
    echo ✗ Docker Compose file not found: backend\docker-compose-full.yml
    pause
    exit /b 1
)

echo ✓ Docker Compose file found
echo.
echo 🚀 Starting all backend services...

cd backend

REM Backend servislerini başlat
docker-compose -f docker-compose-full.yml up -d --scale frontend=0

if %ERRORLEVEL% neq 0 (
    echo ✗ Error starting services
    pause
    cd ..
    exit /b 1
)

echo ✓ Backend services started
echo.
echo ⏳ Waiting for services to be ready (this may take a few minutes)...
timeout /t 45 /nobreak >nul

echo.
echo 📊 Service Status:
docker-compose -f docker-compose-full.yml ps

echo.
echo 🎉 ToyStore backend is ready!
echo.
echo 📱 Access Points:
echo   • API Gateway:        http://localhost:5000
echo   • Swagger UI:         http://localhost:5001/swagger
echo   • RabbitMQ Admin:     http://localhost:15672 (admin/ToyStore123!)
echo   • Database Admin:     http://localhost:8080
echo   • Redis Admin:        http://localhost:8081
echo.
echo 💡 To start frontend separately:
echo    cd ..
echo    npm install ^&^& npm run dev
echo.
echo 🔧 Management Commands:
echo   • View logs:          docker-compose -f docker-compose-full.yml logs -f
echo   • Stop services:      docker-compose -f docker-compose-full.yml down
echo   • Restart services:   docker-compose -f docker-compose-full.yml restart
echo.

cd ..
pause
