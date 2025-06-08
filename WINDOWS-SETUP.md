# 🪟 ToyStore Windows PowerShell Kurulum Rehberi

Bu rehber Windows bilgisayarınızda PowerShell kullanarak ToyStore microservices uygulamasını çalıştırmanız için hazırlanmıştır.

## 📋 Gereksinimler

### 1. Docker Desktop

- **İndir**: [Docker Desktop for Windows](https://www.docker.com/products/docker-desktop/)
- **Kurulum**: Normal kurulum yapın ve Docker Desktop'ı başlatın
- **Kontrol**: PowerShell'de `docker --version` çalıştırın

### 2. PowerShell (Zaten Windows'ta var)

- Windows PowerShell 5.1+ veya PowerShell 7+
- **Kontrol**: `$PSVersionTable.PSVersion`

## 🚀 Hızlı Başlatma (1-Click)

### Adım 1: Projeyi İndirin

```powershell
# GitHub'dan projeyi indirin veya ZIP olarak çıkarın
cd C:\ToyStore  # Projenin bulunduğu klasör
```

### Adım 2: Docker Desktop'ı Başlatın

- Docker Desktop uygulamasını açın
- "Engine running" yazısını bekleyin

### Adım 3: Tek Komutla Başlatın

```powershell
# Backend + Frontend dahil her şeyi başlat
.\start-toystore.ps1 -IncludeFrontend

# Veya sadece backend
.\start-toystore.ps1
```

**🎉 Bu kadar! 2-3 dakika sonra her şey hazır olacak.**

## 📊 Servis Erişim Noktaları

Başlatma tamamlandıktan sonra:

### 🌐 Web Arayüzleri

```
Frontend (React):        http://localhost:3000
API Gateway:              http://localhost:5000
RabbitMQ Yönetim:         http://localhost:15672
Veritabanı Yönetimi:      http://localhost:8080
Redis Yönetimi:           http://localhost:8081
```

### 🔑 Giriş Bilgileri

```
RabbitMQ:     admin / ToyStore123!
Veritabanı:   sa / ToyStore123!
Redis:        (şifre: ToyStore123!)
```

### 🎮 Test Kullanıcıları

```
Admin:        admin@toystore.com / Admin123!
Müşteri:      customer@toystore.com / Customer123!
```

## 🛠️ Yönetim Komutları

### PowerShell Komutları

```powershell
# Durumu kontrol et
cd backend
docker-compose -f docker-compose-full.yml ps

# Logları görüntüle
docker-compose -f docker-compose-full.yml logs -f

# Belirli bir servisin logları
docker-compose -f docker-compose-full.yml logs -f productservice

# Servisleri durdur
docker-compose -f docker-compose-full.yml down

# Servisleri yeniden başlat
docker-compose -f docker-compose-full.yml restart

# Tamamen temizle
docker-compose -f docker-compose-full.yml down -v
docker system prune -f
```

### Gelişmiş Deployment Script

```powershell
# Geliştirme modu
.\scripts\deploy.ps1 -Environment development -Action up

# Production modu
.\scripts\deploy.ps1 -Environment production -Action up

# Sadece veritabanlarını başlat
.\scripts\deploy.ps1 -Action up
docker-compose -f docker-compose-full.yml stop identityservice productservice orderservice userservice inventoryservice notificationservice apigateway

# Temizlik
.\scripts\deploy.ps1 -Action clean
```

## 🔧 Sorun Giderme

### Docker Desktop Sorunları

```powershell
# Docker'ın çalışıp çalışmadığını kontrol et
docker info

# Docker Desktop'ı yeniden başlat
# Docker Desktop uygulamasında: Settings > Reset > Restart Docker Desktop
```

### Port Çakışması

```powershell
# Hangi uygulamanın portu kullandığını bul
netstat -ano | findstr :5000
netstat -ano | findstr :5001

# Process'i sonlandır (PID ile)
taskkill /PID <PID_NUMBER> /F
```

### Bellek Sorunları

```powershell
# Docker Desktop bellek ayarları
# Docker Desktop > Settings > Resources > Advanced
# Memory: En az 4GB, önerilen 8GB
# CPUs: En az 2, önerilen 4+
```

### Servis Başlatma Sorunları

```powershell
# Servisleri tek tek başlat
docker-compose -f docker-compose-full.yml up -d sqlserver
Start-Sleep 30
docker-compose -f docker-compose-full.yml up -d postgresql mongodb redis rabbitmq
Start-Sleep 30
docker-compose -f docker-compose-full.yml up -d identityservice
Start-Sleep 15
docker-compose -f docker-compose-full.yml up -d productservice orderservice userservice inventoryservice notificationservice
Start-Sleep 15
docker-compose -f docker-compose-full.yml up -d apigateway
```

## 📁 Proje Yapısı

```
ToyStore/
├── backend/                    # .NET 8 Microservices
│   ├── src/                   # Kaynak kodları
│   ├── scripts/               # PowerShell scriptleri
│   ├── docker-compose-full.yml # Ana Docker Compose
│   └── logs/                  # Servis logları
├── src/                       # React Frontend
├── start-toystore.ps1         # Hızlı başlatma
├── Dockerfile.frontend        # Frontend Docker
└── nginx.conf                 # Nginx konfigürasyonu
```

## 🔄 Geliştirme Ortamı

### Frontend Geliştirme

```powershell
# Backend Docker'da, Frontend yerel
.\start-toystore.ps1  # Backend'i başlat

# Yeni PowerShell penceresi aç
npm install
npm run dev  # Frontend http://localhost:5173'te
```

### Backend Geliştirme

```powershell
# Sadece veritabanlarını Docker'da başlat
docker-compose -f docker-compose-full.yml up -d sqlserver postgresql mongodb redis rabbitmq

# Visual Studio veya VS Code'da backend servislerini debug et
```

## 🧪 Test Senaryoları

### API Testleri

```powershell
# Health check
Invoke-WebRequest http://localhost:5000/health
Invoke-WebRequest http://localhost:5001/health

# Ürünleri getir
Invoke-WebRequest http://localhost:5000/api/products

# Kategorileri getir
Invoke-WebRequest http://localhost:5000/api/categories
```

### Database Bağlantı Testleri

```powershell
# SQL Server
docker exec -it toystore-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P ToyStore123! -Q "SELECT @@VERSION"

# PostgreSQL
docker exec -it toystore-postgresql psql -U postgres -d toystore_inventory -c "SELECT version();"

# MongoDB
docker exec -it toystore-mongodb mongosh --eval "db.runCommand({ping: 1})"
```

## 📊 Monitoring

### Real-time Monitoring

```powershell
# Tüm servislerin durumu
docker-compose -f docker-compose-full.yml ps

# Resource kullanımı
docker stats

# Log takibi
docker-compose -f docker-compose-full.yml logs -f --tail=100
```

### Web Dashboards

- **RabbitMQ**: http://localhost:15672 - Message queue monitoring
- **Adminer**: http://localhost:8080 - Database management
- **Redis Commander**: http://localhost:8081 - Redis monitoring

## 🆘 Destek

### Yaygın Sorunlar ve Çözümleri

1. **"Docker is not running" Hatası**

   - Docker Desktop'ı başlatın
   - Windows'u yeniden başlatın
   - Docker Desktop'ı yeniden kurun

2. **Port Kullanımda Hatası**

   - `netstat -ano | findstr :5000` ile port kontrolü yapın
   - Çakışan uygulamayı kapatın

3. **Yavaş Başlatma**

   - Docker Desktop'a daha fazla RAM verin (8GB+)
   - SSD kullanın
   - Windows Defender'ı Docker klasörü için hariç tutun

4. **"Service Unhealthy" Hatası**
   - Logları kontrol edin: `docker-compose logs [service-name]`
   - Servisi yeniden başlatın: `docker-compose restart [service-name]`

### İletişim

Sorun yaşarsanız:

- Logları kontrol edin: `docker-compose -f docker-compose-full.yml logs`
- GitHub Issues'da sorun bildirin
- Proje dokümantasyonunu inceleyin

---

**🎮 ToyStore'u Windows'ta başarıyla çalıştırdınız! Eğlenceli geliştirmeler! 🚀**
