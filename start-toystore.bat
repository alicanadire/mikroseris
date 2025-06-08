@echo off
setlocal EnableDelayedExpansion

title ToyStore Microservices Starter

echo.
echo ================================================
echo 🎮 ToyStore Microservices Starter
echo ================================================
echo.

echo Checking Docker...
docker info >nul 2>&1
if errorlevel 1 (
    echo ❌ Docker is not running!
    echo.
    echo Please start Docker Desktop first:
    echo 1. Open Docker Desktop
    echo 2. Wait for "Engine running" message
    echo 3. Run this script again
    echo.
    pause
    exit /b 1
)
echo ✅ Docker is running

echo.
echo Checking files...
if not exist "backend" (
    echo ❌ Backend folder not found!
    pause
    exit /b 1
)

if not exist "backend\docker-compose-full.yml" (
    echo ❌ Docker Compose file not found!
    pause
    exit /b 1
)
echo ✅ All files found

echo.
echo 🚀 Starting ToyStore backend services...
echo.

cd backend

echo Starting infrastructure services...
docker-compose -f docker-compose-full.yml up -d sqlserver postgresql mongodb redis rabbitmq

echo Waiting for databases to initialize...
timeout /t 20 /nobreak >nul

echo Starting application services...
docker-compose -f docker-compose-full.yml up -d identityservice productservice orderservice userservice inventoryservice notificationservice

echo Waiting for services to start...
timeout /t 15 /nobreak >nul

echo Starting API Gateway and admin tools...
docker-compose -f docker-compose-full.yml up -d apigateway adminer redis-commander

if errorlevel 1 (
    echo.
    echo ❌ Failed to start some services!
    echo Check the error messages above.
    echo.
    pause
    exit /b 1
)

echo.
echo ⏳ Final initialization (30 seconds)...
timeout /t 30 /nobreak >nul

echo.
echo 📊 Service Status:
docker-compose -f docker-compose-full.yml ps

echo.
echo ================================================
echo 🎉 ToyStore is ready!
echo ================================================
echo.
echo 📱 Available Services:
echo   🌐 API Gateway:       http://localhost:5000
echo   📝 Swagger Docs:      http://localhost:5001/swagger
echo   🐰 RabbitMQ Admin:    http://localhost:15672
echo   🗄️  Database Admin:    http://localhost:8080
echo   ⚡ Redis Admin:       http://localhost:8081
echo.
echo 🔑 Login Credentials:
echo   Admin:     admin@toystore.com / Admin123!
echo   Customer:  customer@toystore.com / Customer123!
echo   RabbitMQ:  admin / ToyStore123!
echo.
echo 💡 To start frontend:
echo   cd ..
echo   npm install
echo   npm run dev
echo.
echo 🔧 Management:
echo   Stop:      docker-compose -f docker-compose-full.yml down
echo   Restart:   docker-compose -f docker-compose-full.yml restart
echo   Logs:      docker-compose -f docker-compose-full.yml logs -f
echo.

cd ..

pause
