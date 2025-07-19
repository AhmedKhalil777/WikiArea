@echo off
REM WikiArea Startup Script for Windows
REM This script builds and starts the entire WikiArea application stack

echo 🚀 Starting WikiArea Application Stack...
echo ==================================

REM Check if Docker is running
docker info >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Docker is not running. Please start Docker and try again.
    pause
    exit /b 1
)

REM Check if Docker Compose is available
docker-compose --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Docker Compose is not installed. Please install Docker Compose and try again.
    pause
    exit /b 1
)

echo ✅ Docker is running
echo ✅ Docker Compose is available

REM Stop any existing containers
echo.
echo 🛑 Stopping existing containers...
docker-compose down

REM Build and start all services
echo.
echo 🔨 Building and starting services...
echo This may take a few minutes on first run...
docker-compose up --build -d

REM Wait for services to be healthy
echo.
echo ⏳ Waiting for services to start...
timeout /t 10 /nobreak >nul

REM Check service health
echo.
echo 🏥 Checking service health...

REM Check MongoDB
docker-compose exec -T mongodb mongosh --eval "db.adminCommand('ping')" >nul 2>&1
if %errorlevel% equ 0 (
    echo ✅ MongoDB is healthy
) else (
    echo ⚠️  MongoDB is starting up...
)

REM Check Backend
curl -f http://localhost:5000/health >nul 2>&1
if %errorlevel% equ 0 (
    echo ✅ Backend API is healthy
) else (
    echo ⚠️  Backend API is starting up...
)

REM Check Frontend
curl -f http://localhost:4200/health >nul 2>&1
if %errorlevel% equ 0 (
    echo ✅ Frontend is healthy
) else (
    echo ⚠️  Frontend is starting up...
)

REM Check Nginx
curl -f http://localhost:3000/health >nul 2>&1
if %errorlevel% equ 0 (
    echo ✅ Nginx proxy is healthy
) else (
    echo ⚠️  Nginx proxy is starting up...
)

echo.
echo 🎉 WikiArea is starting up!
echo ==================================
echo.
echo 📱 Access URLs:
echo    Frontend:  http://localhost:3000
echo    Backend:   http://localhost:5000
echo    MongoDB:   mongodb://localhost:27017
echo.
echo 📊 Management:
echo    View logs:     docker-compose logs -f
echo    Stop app:      docker-compose down
echo    Restart:       docker-compose restart
echo.
echo ⏳ Services may take a few minutes to fully start.
echo    Check status: docker-compose ps
echo.

REM Open browser
echo 🌐 Opening application in browser...
timeout /t 3 /nobreak >nul
start http://localhost:3000

echo ✨ Startup complete! Check the URLs above to access WikiArea.
pause 