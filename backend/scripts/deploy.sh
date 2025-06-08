#!/bin/bash

# ToyStore Microservices Deployment Script
echo "🚀 Starting ToyStore Microservices Deployment..."

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    print_error "Docker is not running. Please start Docker and try again."
    exit 1
fi

# Check if Docker Compose is available
if ! command -v docker-compose &> /dev/null; then
    print_error "Docker Compose is not installed. Please install Docker Compose and try again."
    exit 1
fi

print_status "Docker environment validated ✓"

# Create necessary directories
print_status "Creating required directories..."
mkdir -p logs
mkdir -p data/sqlserver
mkdir -p data/postgresql
mkdir -p data/mongodb
mkdir -p data/redis
mkdir -p data/rabbitmq

# Function to wait for service to be ready
wait_for_service() {
    local service_name=$1
    local health_check_url=$2
    local max_attempts=30
    local attempt=0

    print_status "Waiting for $service_name to be ready..."
    
    while [ $attempt -lt $max_attempts ]; do
        if curl -f -s $health_check_url > /dev/null 2>&1; then
            print_success "$service_name is ready!"
            return 0
        fi
        
        attempt=$((attempt + 1))
        echo -n "."
        sleep 2
    done
    
    print_error "$service_name failed to start within expected time"
    return 1
}

# Parse command line arguments
ENVIRONMENT=${1:-development}
ACTION=${2:-up}

case $ACTION in
    "up")
        print_status "Starting ToyStore microservices in $ENVIRONMENT mode..."
        
        if [ "$ENVIRONMENT" = "development" ]; then
            # Development mode - start infrastructure first
            print_status "Starting infrastructure services..."
            docker-compose up -d sqlserver postgresql mongodb redis rabbitmq
            
            # Wait for databases to be ready
            print_status "Waiting for databases to initialize..."
            sleep 30
            
            # Start Identity Service first
            print_status "Starting Identity Service..."
            docker-compose up -d identityservice
            
            # Wait for Identity Service
            wait_for_service "Identity Service" "http://localhost:5004/health"
            
            # Start other services
            print_status "Starting application services..."
            docker-compose up -d productservice orderservice userservice inventoryservice notificationservice
            
            # Wait for services to be ready
            sleep 15
            
            # Start API Gateway last
            print_status "Starting API Gateway..."
            docker-compose up -d apigateway
            
        else
            # Production mode - start all services
            print_status "Starting all services in production mode..."
            docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
        fi
        
        # Wait for API Gateway
        wait_for_service "API Gateway" "http://localhost:5000/health"
        
        print_success "🎉 ToyStore microservices deployment completed!"
        print_status "Services are available at:"
        echo "  📱 API Gateway: http://localhost:5000"
        echo "  🔐 Identity Server: http://localhost:5004"
        echo "  🛍️  Product Service: http://localhost:5001"
        echo "  📦 Order Service: http://localhost:5002"
        echo "  👤 User Service: http://localhost:5003"
        echo "  📊 Inventory Service: http://localhost:5005"
        echo "  📧 Notification Service: http://localhost:5006"
        echo "  🐰 RabbitMQ Management: http://localhost:15672 (admin/ToyStore123!)"
        echo ""
        print_status "Health checks:"
        echo "  curl http://localhost:5000/health"
        echo "  curl http://localhost:5001/health"
        echo "  curl http://localhost:5002/health"
        ;;
        
    "down")
        print_status "Stopping ToyStore microservices..."
        docker-compose down
        print_success "All services stopped."
        ;;
        
    "restart")
        print_status "Restarting ToyStore microservices..."
        docker-compose restart
        print_success "All services restarted."
        ;;
        
    "logs")
        SERVICE=${3:-}
        if [ -z "$SERVICE" ]; then
            docker-compose logs -f
        else
            docker-compose logs -f $SERVICE
        fi
        ;;
        
    "status")
        print_status "Service status:"
        docker-compose ps
        ;;
        
    "clean")
        print_warning "This will remove all containers, volumes, and images. Are you sure? (y/N)"
        read -r response
        if [[ "$response" =~ ^([yY][eE][sS]|[yY])$ ]]; then
            print_status "Cleaning up Docker environment..."
            docker-compose down -v --remove-orphans
            docker system prune -f
            print_success "Cleanup completed."
        else
            print_status "Cleanup cancelled."
        fi
        ;;
        
    "seed")
        print_status "Seeding databases with sample data..."
        
        # Seed Identity Service
        docker-compose exec identityservice dotnet run --seed
        
        # Seed Product Service (migrations will handle seed data)
        docker-compose exec productservice dotnet ef database update
        
        print_success "Database seeding completed."
        ;;
        
    "backup")
        print_status "Creating backup of all databases..."
        BACKUP_DIR="backups/$(date +%Y%m%d_%H%M%S)"
        mkdir -p $BACKUP_DIR
        
        # Backup SQL Server
        docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P ToyStore123! -Q "BACKUP DATABASE ToyStoreProducts TO DISK='/tmp/products.bak'"
        docker cp $(docker-compose ps -q sqlserver):/tmp/products.bak $BACKUP_DIR/
        
        # Backup MongoDB
        docker-compose exec mongodb mongodump --uri="mongodb://admin:ToyStore123!@localhost:27017/toystore_notifications?authSource=admin" --out /tmp/mongo_backup
        docker cp $(docker-compose ps -q mongodb):/tmp/mongo_backup $BACKUP_DIR/
        
        print_success "Backup created in $BACKUP_DIR"
        ;;
        
    "help"|*)
        echo "ToyStore Microservices Deployment Script"
        echo ""
        echo "Usage: $0 [environment] [action] [options]"
        echo ""
        echo "Environments:"
        echo "  development    Development mode (default)"
        echo "  production     Production mode"
        echo ""
        echo "Actions:"
        echo "  up             Start all services (default)"
        echo "  down           Stop all services"
        echo "  restart        Restart all services"
        echo "  logs [service] View logs (optionally for specific service)"
        echo "  status         Show service status"
        echo "  clean          Remove all containers and volumes"
        echo "  seed           Seed databases with sample data"
        echo "  backup         Create database backups"
        echo "  help           Show this help message"
        echo ""
        echo "Examples:"
        echo "  $0                           # Start in development mode"
        echo "  $0 production up             # Start in production mode"
        echo "  $0 development logs          # View all logs"
        echo "  $0 development logs apigateway # View API gateway logs"
        echo "  $0 development down          # Stop all services"
        ;;
esac
