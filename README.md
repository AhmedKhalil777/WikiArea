# WikiArea - Knowledge Management System

A modern wiki application built with Angular 18 frontend and .NET 8 backend, designed for corporate knowledge management with ADFS authentication.

## 🏗️ Architecture

- **Frontend**: Angular 18 with Angular Material
- **Backend**: .NET 8 Web API with Clean Architecture
- **Database**: MongoDB 7.0
- **Authentication**: ADFS/OAuth2
- **Deployment**: Docker Compose

## 🚀 Quick Start with Docker

### Prerequisites
- Docker Engine 20.10+
- Docker Compose 2.0+
- 8GB RAM recommended
- 10GB free disk space

### 1. Clone and Setup
```bash
git clone <repository-url>
cd WikiArea
```

### 2. Environment Configuration
Update the environment files:

**Frontend Environment** (`src/wikiarea-frontend/src/environments/environment.ts`):
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:3000',
  adfsUrl: 'https://your-adfs-server.com/adfs',
  clientId: 'your-client-id',
  redirectUri: 'http://localhost:3000/auth/callback'
};
```

**Backend Configuration**: Update `src/WikiArea.Web/appsettings.json` if needed.

### 3. Start the Application
```bash
# Build and start all services
docker-compose up --build

# Or run in detached mode
docker-compose up -d --build
```

### 4. Access the Application
- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5000
- **MongoDB**: mongodb://localhost:27017

## 🔧 Development Setup

### Local Development (without Docker)

#### Frontend Setup
```bash
cd src/wikiarea-frontend
npm install
npm start
# App runs on http://localhost:4200
```

#### Backend Setup
```bash
cd src/WikiArea.Web
dotnet restore
dotnet run
# API runs on http://localhost:5000
```

#### MongoDB Setup
```bash
# Using Docker for MongoDB only
docker run -d --name wikiarea-mongodb \
  -p 27017:27017 \
  -e MONGO_INITDB_ROOT_USERNAME=admin \
  -e MONGO_INITDB_ROOT_PASSWORD=wikiarea123 \
  mongo:7.0
```

## 📦 Docker Services

| Service | Container | Port | Description |
|---------|-----------|------|-------------|
| nginx | wikiarea-nginx | 3000 | Reverse proxy |
| frontend | wikiarea-frontend | 4200 | Angular app |
| backend | wikiarea-backend | 5000 | .NET API |
| mongodb | wikiarea-mongodb | 27017 | Database |

## 🛠️ Management Commands

```bash
# View logs
docker-compose logs -f [service-name]

# Restart a service
docker-compose restart [service-name]

# Stop all services
docker-compose down

# Remove all containers and volumes
docker-compose down -v

# Rebuild a specific service
docker-compose build [service-name]

# Scale a service
docker-compose up -d --scale frontend=2
```

## 🔐 Authentication Setup

1. **Configure ADFS**:
   - Add WikiArea as a relying party
   - Set redirect URI: `http://localhost:3000/auth/callback`
   - Note the client ID and authority URL

2. **Update Environment**:
   - Set `adfsUrl` to your ADFS server
   - Set `clientId` to your application ID
   - Update `redirectUri` if needed

## 📁 Project Structure

```
WikiArea/
├── src/
│   ├── wikiarea-frontend/          # Angular 18 app
│   ├── WikiArea.Core/              # Domain entities
│   ├── WikiArea.Application/       # Business logic
│   ├── WikiArea.Infrastructure/    # Data access
│   └── WikiArea.Web/               # Web API
├── tests/
├── nginx/                          # Nginx configuration
├── scripts/                        # Database scripts
├── docker-compose.yml             # Docker orchestration
├── Dockerfile.backend             # Backend container
└── README.md
```

## 🔧 Configuration

### Environment Variables

#### Backend (.NET)
```env
ASPNETCORE_ENVIRONMENT=Production
MongoDbSettings__ConnectionString=mongodb://admin:wikiarea123@mongodb:27017/wikiarea?authSource=admin
MongoDbSettings__DatabaseName=wikiarea
```

#### Frontend (Angular)
- Environment files in `src/wikiarea-frontend/src/environments/`
- Configure API URL, ADFS settings

### MongoDB Configuration
- Username: `admin`
- Password: `wikiarea123`
- Database: `wikiarea`
- Initialization script: `scripts/mongodb-init.js`

## 🚀 Production Deployment

1. **Update Environment Files**:
   - Set production URLs
   - Configure proper ADFS settings
   - Update database credentials

2. **SSL/HTTPS Setup**:
   - Add SSL certificates to nginx
   - Update environment URLs to HTTPS

3. **Security Considerations**:
   - Change default passwords
   - Configure proper CORS settings
   - Set up proper firewall rules

## 🐛 Troubleshooting

### Common Issues

1. **Frontend not loading**:
   ```bash
   docker-compose logs frontend
   # Check if Angular build succeeded
   ```

2. **Backend API errors**:
   ```bash
   docker-compose logs backend
   # Check MongoDB connection
   ```

3. **Database connection issues**:
   ```bash
   docker-compose logs mongodb
   # Verify MongoDB is running
   docker-compose exec mongodb mongosh --eval "db.adminCommand('ping')"
   ```

4. **Permission errors**:
   ```bash
   # Reset Docker volumes
   docker-compose down -v
   docker-compose up --build
   ```

### Health Checks

- **Application**: http://localhost:3000/health
- **API**: http://localhost:5000/health
- **MongoDB**: Check with MongoDB client

## 📊 Monitoring

### Logs
```bash
# View all logs
docker-compose logs -f

# View specific service logs
docker-compose logs -f backend
docker-compose logs -f frontend
```

### Resource Usage
```bash
# View container stats
docker stats

# View container processes
docker-compose top
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test with Docker
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Support

For issues and questions:
- Check the troubleshooting section
- Review Docker logs
- Create an issue in the repository
