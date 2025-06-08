# 🛠️ ToyStore Troubleshooting Guide

## 🚨 Common Issues and Solutions

### 1. PowerShell Script Error (Try/Catch Missing)

**Error**: `The Try statement is missing its Catch or Finally block`

**Solution**: Use the updated scripts:

```powershell
# Use the simple backend starter
.\start-simple-backend.ps1

# Or the fixed main script
.\start-toystore.ps1
```

### 2. Docker MongoDB Extraction Error

**Error**: `failed to extract layer... input/output error`

**Solutions**:

#### Option A: Use Simple Backend (Recommended)

```powershell
# Start only essential services (no MongoDB)
.\start-simple-backend.ps1
```

#### Option B: Clean Docker and Retry

```powershell
# Clean Docker system
docker system prune -f
docker volume prune -f

# Clear Docker cache
docker builder prune -f

# Restart Docker Desktop, then try again
.\start-toystore.ps1
```

#### Option C: Manual Service Start

```powershell
cd backend

# Start services one by one
docker-compose up -d sqlserver
docker-compose up -d redis
docker-compose up -d rabbitmq
docker-compose up -d postgresql

# Check status
docker-compose ps
```

### 3. Frontend Loading Forever

**Issue**: Frontend shows "Generating your app..." or loading spinner

**Solutions**:

#### Check if Dev Server is Running

- Frontend should be accessible at http://localhost:8080
- If not, the dev server might have crashed

#### Restart Frontend

```bash
# Stop and restart frontend
# (Dev server will restart automatically)
```

#### Clear Browser Cache

- Press `Ctrl+F5` to hard refresh
- Or open Developer Tools → Network → Disable cache

### 4. Port Conflicts

**Error**: `Port already in use`

**Solutions**:

#### Check Which Process is Using the Port

```cmd
# Check port 3000
netstat -ano | findstr :3000

# Check port 5000
netstat -ano | findstr :5000

# Check port 1433 (SQL Server)
netstat -ano | findstr :1433
```

#### Kill Conflicting Processes

```cmd
# Kill process by PID (replace <PID> with actual number)
taskkill /PID <PID> /F
```

#### Use Alternative Ports

Edit `.env` file:

```env
VITE_API_GATEWAY_URL=http://localhost:5001/api
VITE_IDENTITY_SERVER_URL=http://localhost:5005
```

### 5. Docker Desktop Issues

**Common Problems**:

- Docker not starting
- Out of memory
- WSL issues (Windows)

**Solutions**:

#### Increase Docker Memory

1. Open Docker Desktop
2. Settings → Resources → Memory
3. Allocate at least 4GB
4. Apply & Restart

#### Enable WSL 2 (Windows 10/11)

1. Docker Desktop → Settings → General
2. Check "Use WSL 2 based engine"
3. Apply & Restart

#### Restart Docker Desktop

1. Quit Docker Desktop completely
2. Wait 30 seconds
3. Start Docker Desktop
4. Wait for it to fully initialize

### 6. Database Connection Issues

**Error**: Cannot connect to SQL Server

**Solutions**:

#### Wait for SQL Server to Start

SQL Server takes 2-3 minutes to fully initialize:

```powershell
# Check SQL Server logs
docker logs toystore-sqlserver

# Wait for this message: "SQL Server is now ready for client connections"
```

#### Test Connection

```cmd
# Using SQL Server tools (if installed)
sqlcmd -S localhost,1433 -U sa -P ToyStore123!

# Or check from Docker
docker exec -it toystore-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P ToyStore123! -Q "SELECT 1"
```

### 7. Services Not Responding

**Issue**: Backend services return 500 errors

**Diagnosis**:

```powershell
# Check service status
cd backend
docker-compose ps

# Check logs for errors
docker-compose logs api-gateway
docker-compose logs identity-service
```

**Solutions**:

1. **Wait longer**: Services need 3-5 minutes to fully start
2. **Check dependencies**: Ensure databases are running first
3. **Restart services**: `docker-compose restart`

## 📋 Quick Diagnostic Commands

### Check System Status

```powershell
# Docker version and status
docker --version
docker info

# Docker Compose version
docker-compose --version

# Available memory
systeminfo | findstr "Total Physical Memory"
```

### Check ToyStore Services

```powershell
cd backend

# Service status
docker-compose ps

# Quick health check
curl http://localhost:5000/health
curl http://localhost:15672  # RabbitMQ UI
```

### Log Collection

```powershell
# Collect all logs for troubleshooting
cd backend
docker-compose logs > toystore-logs.txt
```

## 🎯 Minimal Working Setup

If everything fails, use this minimal setup:

1. **Start only databases**:

```powershell
.\start-simple-backend.ps1
```

2. **Access frontend**: http://localhost:8080

   - Frontend works with mock data
   - No backend needed for basic testing

3. **Gradually add services**:

```powershell
# Add services one by one
cd backend
docker-compose up -d api-gateway
docker-compose up -d identity-service
```

## 📞 Getting Help

### Before Asking for Help:

1. ✅ Check this troubleshooting guide
2. ✅ Try the simple backend starter
3. ✅ Collect logs: `docker-compose logs > logs.txt`
4. ✅ Note your system specs and Docker version

### System Information Template:

```
OS: Windows 10/11
Docker Desktop Version:
RAM:
Issue:
Steps Tried:
Error Messages:
```

## 🚀 Success Indicators

Your ToyStore is working correctly when:

- ✅ Frontend loads at http://localhost:8080
- ✅ No loading spinner (shows actual toy store)
- ✅ Docker containers are running: `docker ps`
- ✅ Databases are accessible on their ports
- ✅ No errors in browser console

---

💡 **Pro Tip**: Start with `.\start-simple-backend.ps1` for the most reliable experience!
