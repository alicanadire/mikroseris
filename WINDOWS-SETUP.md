# 🪟 Windows Setup Guide for ToyStore

Complete guide for setting up ToyStore on Windows systems.

## 🚀 Quick Start (Recommended)

### Option 1: PowerShell (Easiest)

```powershell
# Open PowerShell as Administrator
.\start-toystore.ps1
```

### Option 2: Batch File

```cmd
# Open Command Prompt
start-toystore.bat
```

### Option 3: Simple Start

```powershell
# For minimal setup
.\start-simple.ps1
```

## 📋 Prerequisites

### Required Software

1. **Docker Desktop**

   - Download: https://www.docker.com/products/docker-desktop
   - Make sure it's running before starting ToyStore
   - Minimum 4GB RAM allocated to Docker

2. **Node.js 18+**

   - Download: https://nodejs.org/
   - Verify: `node --version` should show v18 or higher

3. **Git** (Optional, for cloning)
   - Download: https://git-scm.com/download/win

### Optional (for development)

- **.NET 8 SDK**: https://dotnet.microsoft.com/download
- **Visual Studio 2022**: https://visualstudio.microsoft.com/
- **SQL Server Management Studio**: https://docs.microsoft.com/en-us/sql/ssms/

## 🔧 Detailed Setup Steps

### Step 1: Prepare Environment

1. **Install Docker Desktop**

   ```cmd
   # After installation, make sure Docker is running
   docker --version
   docker-compose --version
   ```

2. **Install Node.js**

   ```cmd
   # Verify installation
   node --version
   npm --version
   ```

3. **Clone or Download Project**

   ```cmd
   # If using Git
   git clone <repository-url>
   cd toystore

   # Or download and extract ZIP file
   ```

### Step 2: Configure Environment

1. **Copy Environment File**

   ```cmd
   copy .env.example .env
   ```

2. **Edit `.env` if needed** (optional)
   - Default settings work for local development
   - Only change if you need custom ports

### Step 3: Start Services

#### Option A: Automated Start (Recommended)

```powershell
# PowerShell - starts everything automatically
.\start-toystore.ps1

# Show logs while starting
.\start-toystore.ps1 -ShowLogs

# Include frontend in Docker
.\start-toystore.ps1 -IncludeFrontend
```

#### Option B: Manual Start

```cmd
# 1. Start backend services
cd backend
docker-compose -f docker-compose-full.yml up -d

# 2. Wait for services (about 2 minutes)
timeout /t 120

# 3. Start frontend
cd ..
npm install
npm run dev
```

## 🌐 Access Points

After successful startup:

- **🏠 Main Website**: http://localhost:3000
- **👨‍💼 Admin Panel**: http://localhost:3000/admin
- **🔗 API Gateway**: http://localhost:5000
- **📚 API Documentation**: http://localhost:5001/swagger
- **🐰 RabbitMQ Management**: http://localhost:15672
  - Username: `admin`
  - Password: `ToyStore123!`
- **🗄️ Database Admin**: http://localhost:8080
- **⚡ Redis Admin**: http://localhost:8081

## 🛠️ Management Commands

### PowerShell Commands

```powershell
# View service status
cd backend
docker-compose -f docker-compose-full.yml ps

# View logs
docker-compose -f docker-compose-full.yml logs -f

# Stop all services
docker-compose -f docker-compose-full.yml down

# Restart services
docker-compose -f docker-compose-full.yml restart

# Start specific service
docker-compose -f docker-compose-full.yml up -d sqlserver
```

### Frontend Commands

```cmd
# Install dependencies
npm install

# Start development server
npm run dev

# Build for production
npm run build

# Run tests
npm test
```

## 🚨 Troubleshooting

### Common Issues

#### 1. Docker Not Running

**Error**: `docker: command not found`
**Solution**:

- Start Docker Desktop
- Wait for Docker to fully initialize
- Check Docker icon in system tray

#### 2. Port Already in Use

**Error**: `Port 3000 is already in use`
**Solution**:

```cmd
# Find process using port
netstat -ano | findstr :3000

# Kill process (replace PID)
taskkill /PID <PID> /F

# Or change port in package.json
```

#### 3. Services Not Starting

**Error**: Services fail to start
**Solution**:

```powershell
# Check Docker logs
cd backend
docker-compose -f docker-compose-full.yml logs

# Restart Docker Desktop
# Try again with clean start
docker-compose -f docker-compose-full.yml down -v
docker-compose -f docker-compose-full.yml up -d
```

#### 4. Database Connection Issues

**Error**: Cannot connect to database
**Solution**:

```cmd
# Wait longer for SQL Server to start
timeout /t 60

# Check SQL Server health
docker-compose -f docker-compose-full.yml logs sqlserver

# Restart database services
docker-compose -f docker-compose-full.yml restart sqlserver
```

#### 5. Memory Issues

**Error**: Docker runs out of memory
**Solution**:

- Increase Docker memory in Docker Desktop settings
- Close other applications
- Restart Docker Desktop

### Performance Tips

1. **Allocate Enough Memory**

   - Docker Desktop → Settings → Resources
   - Allocate at least 4GB RAM to Docker

2. **Use SSD Storage**

   - Docker volumes work better on SSD
   - Consider moving Docker data directory to SSD

3. **Close Unnecessary Applications**

   - Free up RAM for Docker containers
   - Close browser tabs and other development tools

4. **Enable WSL 2 (Windows 10/11)**
   - Better performance than Hyper-V
   - Docker Desktop → Settings → General → Use WSL 2

## 🔒 Firewall and Security

### Windows Defender

If Windows blocks Docker:

1. Open Windows Defender Firewall
2. Allow Docker Desktop through firewall
3. Allow Node.js through firewall

### Antivirus Software

Some antivirus may block Docker:

- Add Docker installation folder to exclusions
- Add project folder to exclusions
- Temporarily disable real-time protection

## 🎓 Development Environment

### Visual Studio Code Setup

1. **Install Extensions**:

   - ES7+ React/Redux/React-Native snippets
   - Prettier - Code formatter
   - Auto Rename Tag
   - Bracket Pair Colorizer
   - GitLens

2. **Workspace Settings** (`.vscode/settings.json`):
   ```json
   {
     "editor.formatOnSave": true,
     "editor.defaultFormatter": "esbenp.prettier-vscode",
     "typescript.preferences.importModuleSpecifier": "relative"
   }
   ```

### Database Management

1. **SQL Server Management Studio**

   - Server: `localhost,1433`
   - Username: `sa`
   - Password: `ToyStore123!`

2. **pgAdmin** (for PostgreSQL)
   - Available at: http://localhost:8080
   - Login with credentials from docker-compose

## 📞 Getting Help

### Check Service Status

```powershell
# Quick health check
.\start-toystore.ps1 -ShowLogs

# Detailed status
cd backend
docker-compose -f docker-compose-full.yml ps
```

### Log Collection

```powershell
# Collect all logs for troubleshooting
cd backend
docker-compose -f docker-compose-full.yml logs > logs.txt
```

### System Information

```cmd
# System specs
systeminfo | findstr /C:"Total Physical Memory"
wmic cpu get name

# Docker info
docker info
docker version
```

## 🎯 Production Deployment on Windows

### IIS Deployment

1. Build frontend: `npm run build`
2. Copy `dist` folder to IIS wwwroot
3. Configure reverse proxy for API calls

### Windows Service

1. Use PM2 or Windows Service wrapper
2. Configure automatic startup
3. Set up monitoring and logging

---

**💡 Tip**: Keep Docker Desktop running in the background for best performance. The startup scripts handle everything else automatically!
