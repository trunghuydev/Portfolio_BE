# HƯỚNG DẪN CHẠY DỰ ÁN PORTFOLIO BACKEND

## 📋 Mô tả dự án

Dự án Portfolio Backend là một API RESTful được xây dựng bằng .NET 9.0, sử dụng kiến trúc Clean Architecture với các layer:
- **ZEN.Startup**: Entry point của ứng dụng
- **ZEN.Controller**: API endpoints và middleware
- **ZEN.Application**: Business logic và use cases
- **ZEN.Domain**: Domain entities và interfaces
- **ZEN.Infrastructure**: Implementations của các services
- **ZEN.Infrastructure.Mysql**: Database context và migrations (PostgreSQL)
- **ZEN.Contract**: DTOs và contracts
- **ZEN.CoreLib**: Shared libraries

## 🔧 Yêu cầu hệ thống

### Phần mềm cần cài đặt:
1. **.NET SDK 9.0** hoặc cao hơn
   - Tải tại: https://dotnet.microsoft.com/download
   - Kiểm tra phiên bản: `dotnet --version`

2. **PostgreSQL Database**
   - Cài đặt PostgreSQL hoặc sử dụng cloud database (Neon, Supabase, etc.)
   - Hoặc sử dụng Docker: `docker run -d -p 5432:5432 -e POSTGRES_PASSWORD=yourpassword postgres`

3. **Redis** (Tùy chọn - nếu sử dụng caching)
   - Cài đặt Redis hoặc sử dụng Docker: `docker run -d -p 6379:6379 redis`

4. **Git** (để clone repository)

## 📦 Cài đặt và cấu hình

### Bước 1: Clone repository
```bash
git clone <repository-url>
cd Portfolio_BE
```

### Bước 2: Cấu hình Environment Variables

Tạo file `.env` trong thư mục gốc của dự án hoặc cấu hình các biến môi trường:

```env
# Database Configuration
DB_CONNECTION_STRING=Host=localhost;Port=5432;Database=portfolio_db;Username=postgres;Password=yourpassword

# JWT Configuration
JWT_SECRET=your-super-secret-jwt-key-minimum-32-characters-long

# Server Configuration
PORT=5005
DB_LOGGING=True

# Redis Configuration (Optional)
REDIS_MODE=false
REDIS_CONNECTION_STRING=localhost:6379
REDIS_USER=default

# Storage Configuration (S3-compatible)
SIMPLE_STORAGE_SERVICE_URL=your-storage-service-url
SIMPLE_STORAGE_ACCESS_KEY=your-access-key
SIMPLE_STORAGE_SECRET_KEY=your-secret-key
```

**Lưu ý quan trọng:**
- `DB_CONNECTION_STRING`: Connection string cho PostgreSQL database
- `JWT_SECRET`: Secret key để tạo JWT token (nên có độ dài tối thiểu 32 ký tự)
- `PORT`: Port mà server sẽ chạy (mặc định: 5005)

### Bước 3: Restore dependencies

```bash
cd src
dotnet restore
```

### Bước 4: Tạo và cập nhật Database

Dự án sử dụng Entity Framework Core với PostgreSQL. Database sẽ tự động được tạo và migrate khi chạy ứng dụng lần đầu (trong môi trường Development).

Hoặc chạy migration thủ công:

```bash
cd src/ZEN.Startup
dotnet ef database update --project ../ZEN.Infrastructure.Mysql
```

**QUAN TRỌNG - Migration cho Dynamic Routing:**

Sau khi chạy migration, bạn cần chạy script SQL để thêm các fields mới cho username:

```bash
# Chạy script migration
psql -h your_host -U your_user -d your_database -f MIGRATION_ADD_USERNAME_FIELDS.sql
```

Hoặc chạy trực tiếp trong database:

```sql
-- Xem file MIGRATION_ADD_USERNAME_FIELDS.sql để biết chi tiết
```

Script này sẽ:
- Thêm các columns: `username`, `slug`, `is_public`, `username_changed_count`, `last_username_change_date`
- Tạo unique index trên `username`
- Tạo composite index trên `(username, is_public)`
- Migrate dữ liệu cho users hiện tại (tạo username từ email hoặc UserName)

### Bước 5: Chạy ứng dụng

#### Cách 1: Chạy trực tiếp
```bash
cd src/ZEN.Startup
dotnet run
```

#### Cách 2: Chạy với cấu hình cụ thể
```bash
cd src/ZEN.Startup
dotnet run --environment Development
```

#### Cách 3: Build và chạy
```bash
cd src/ZEN.Startup
dotnet build
dotnet run
```

Ứng dụng sẽ chạy tại: `http://localhost:5005` (hoặc port bạn đã cấu hình)

### Bước 6: Kiểm tra ứng dụng

1. **Health Check**: Mở trình duyệt và truy cập:
   ```
   http://localhost:5005/healthcheck
   ```
   Kết quả mong đợi: `Server is alive!`

2. **Swagger UI**: Truy cập:
   ```
   http://localhost:5005/swagger
   ```
   Đây là nơi bạn có thể xem và test tất cả các API endpoints.

## 🐳 Chạy với Docker

### Build Docker image:
```bash
cd src
docker build -t portfolio-backend .
```

### Chạy Docker container:
```bash
docker run -d \
  -p 5005:5005 \
  -e DB_CONNECTION_STRING="Host=host.docker.internal;Port=5432;Database=portfolio_db;Username=postgres;Password=yourpassword" \
  -e JWT_SECRET="your-super-secret-jwt-key-minimum-32-characters-long" \
  -e PORT=5005 \
  --name portfolio-api \
  portfolio-backend
```

## 🔐 Tài khoản mặc định

Dự án có 2 tài khoản được seed sẵn trong database:

1. **Username**: `trungthanh`
   - **Email**: buithanh10112000@gmail.com
   - **Password**: (cần reset hoặc kiểm tra trong code)

2. **Username**: `trunghuy`
   - **Email**: trunghuy832@gmail.com
   - **Password**: (cần reset hoặc kiểm tra trong code)

**Lưu ý**: Bạn cần reset password hoặc tạo tài khoản mới thông qua API `/api/v1/account/dev-register`

## 🛠️ Troubleshooting

### Lỗi kết nối Database
- Kiểm tra PostgreSQL đã chạy chưa
- Kiểm tra connection string trong `.env` file
- Đảm bảo database đã được tạo

### Lỗi JWT_SECRET
- Đảm bảo biến môi trường `JWT_SECRET` đã được set
- JWT_SECRET phải có độ dài tối thiểu 32 ký tự

### Lỗi Port đã được sử dụng
- Thay đổi `PORT` trong `.env` file
- Hoặc kill process đang sử dụng port đó

### Lỗi Migration
- Xóa database và tạo lại
- Hoặc chạy: `dotnet ef database drop` sau đó `dotnet ef database update`

## 📝 Cấu trúc thư mục quan trọng

```
src/
├── ZEN.Startup/          # Entry point, Program.cs, appsettings.json
├── ZEN.Controller/       # API endpoints, middleware, configurations
├── ZEN.Application/      # Use cases, business logic
├── ZEN.Domain/           # Entities, interfaces, domain logic
├── ZEN.Infrastructure/   # External services implementations
├── ZEN.Infrastructure.Mysql/  # Database context, migrations
└── ZEN.Contract/         # DTOs, request/response models
```

## 🚀 Deploy lên Production

### Deploy lên Render.com
Dự án đã được cấu hình sẵn với `render.yaml`. Chỉ cần:
1. Kết nối GitHub repository với Render
2. Render sẽ tự động build và deploy từ Dockerfile

### Deploy lên các platform khác
1. Build project: `dotnet publish -c Release`
2. Copy các file trong `bin/Release/net9.0/publish` lên server
3. Cấu hình environment variables trên server
4. Chạy: `dotnet ZEN.Startup.dll`

## 📚 Tài liệu tham khảo

- API Documentation: Xem file `API_ENDPOINTS.md`
- Database Structure: Xem file `CAU_TRUC_DB_VA_DU_AN.md`
- .NET Documentation: https://docs.microsoft.com/dotnet
- Entity Framework Core: https://docs.microsoft.com/ef/core

## 💡 Tips

1. Sử dụng Swagger UI để test API dễ dàng
2. Kiểm tra logs trong console để debug
3. Sử dụng Postman hoặc Insomnia để test API
4. Đảm bảo database connection string đúng format PostgreSQL

## ⚠️ Lưu ý bảo mật

- **KHÔNG** commit file `.env` lên Git
- Sử dụng strong JWT_SECRET trong production
- Cấu hình CORS đúng cách cho production
- Sử dụng HTTPS trong production
- Bảo vệ database connection string



