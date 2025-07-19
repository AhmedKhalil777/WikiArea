#!/bin/bash

# WikiArea Startup Script
# This script builds and starts the entire WikiArea application stack

set -e

echo "🚀 Starting WikiArea Application Stack..."
echo "=================================="

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker is not running. Please start Docker and try again."
    exit 1
fi

# Check if Docker Compose is available
if ! command -v docker-compose &> /dev/null; then
    echo "❌ Docker Compose is not installed. Please install Docker Compose and try again."
    exit 1
fi

echo "✅ Docker is running"
echo "✅ Docker Compose is available"

# Stop any existing containers
echo ""
echo "🛑 Stopping existing containers..."
docker-compose down

# Remove old images (optional - uncomment if you want to force rebuild)
# echo "🗑️  Removing old images..."
# docker-compose down --rmi all

# Build and start all services
echo ""
echo "🔨 Building and starting services..."
echo "This may take a few minutes on first run..."
docker-compose up --build -d

# Wait for services to be healthy
echo ""
echo "⏳ Waiting for services to start..."
sleep 10

# Check service health
echo ""
echo "🏥 Checking service health..."

# Check MongoDB
if docker-compose exec -T mongodb mongosh --eval "db.adminCommand('ping')" > /dev/null 2>&1; then
    echo "✅ MongoDB is healthy"
else
    echo "⚠️  MongoDB is starting up..."
fi

# Check Backend
if curl -f http://localhost:5000/health > /dev/null 2>&1; then
    echo "✅ Backend API is healthy"
else
    echo "⚠️  Backend API is starting up..."
fi

# Check Frontend
if curl -f http://localhost:4200/health > /dev/null 2>&1; then
    echo "✅ Frontend is healthy"
else
    echo "⚠️  Frontend is starting up..."
fi

# Check Nginx
if curl -f http://localhost:3000/health > /dev/null 2>&1; then
    echo "✅ Nginx proxy is healthy"
else
    echo "⚠️  Nginx proxy is starting up..."
fi

echo ""
echo "🎉 WikiArea is starting up!"
echo "=================================="
echo ""
echo "📱 Access URLs:"
echo "   Frontend:  http://localhost:3000"
echo "   Backend:   http://localhost:5000"
echo "   MongoDB:   mongodb://localhost:27017"
echo ""
echo "📊 Management:"
echo "   View logs:     docker-compose logs -f"
echo "   Stop app:      docker-compose down"
echo "   Restart:       docker-compose restart"
echo ""
echo "⏳ Services may take a few minutes to fully start."
echo "   Check status: docker-compose ps"
echo ""

# Optional: Open browser
if command -v xdg-open &> /dev/null; then
    echo "🌐 Opening application in browser..."
    sleep 5
    xdg-open http://localhost:3000
elif command -v open &> /dev/null; then
    echo "🌐 Opening application in browser..."
    sleep 5
    open http://localhost:3000
fi

echo "✨ Startup complete! Check the URLs above to access WikiArea." 