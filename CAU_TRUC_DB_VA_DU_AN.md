# CẤU TRÚC DATABASE VÀ DỰ ÁN

##  CẤU TRÚC DATABASE

### Tổng quan

Dự án sử dụng **PostgreSQL** làm database chính, quản lý bằng **Entity Framework Core** với code-first approach.

### Sơ đồ quan hệ (ERD)

```
┌─────────────────┐
│    AspUser      │ (Identity User - kế thừa từ IdentityUser)
├─────────────────┤
│ Id (PK)         │
│ UserName        │
│ Email           │
│ fullname        │
│ university_name │
│ address         │
│ phone_number    │
│ github          │
│ dob             │
│ avatar          │
│ position_career │
│ expOfYear       │
│ background      │
│ mindset         │
│ linkedin_url    │
│ facebook_url    │
│ GPA             │
└────────┬────────┘
         │
         │ 1:N
         ├─────────────────┬──────────────────┬──────────────────┬─────────────────┐
         │                 │                  │                  │                 │
         ▼                 ▼                  ▼                  ▼                 ▼
┌──────────────┐  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│RefreshToken  │  │ UserSkill    │  │UserProject   │  │WorkExperience│  │ Certificate  │
├──────────────┤  ├──────────────┤  ├──────────────┤  ├──────────────┤  ├──────────────┤
│ Id (PK)      │  │ Id (PK)      │  │ Id (PK)      │  │ Id (PK)      │  │ Id (PK)      │
│ Token        │  │ user_id (FK) │  │ user_id (FK) │  │ user_id (FK) │  │ user_id (FK) │
│ ExpireAt     │  │ skill_id (FK)│  │ project_id   │  │ company_name │  │certificate_ │
│ CreatedAt    │  └──────┬───────┘  │   (FK)       │  │ position     │  │   name      │
│ CreatedByIp  │         │          └──────┬───────┘  │ duration     │  └─────────────┘
│ Revoked      │         │                 │          │ description  │
│ AspUserId    │         │                 │          │ project_id   │
└──────────────┘         │                 │          └──────┬───────┘
                         │                 │                 │
                         │                 │                 │ 1:N
                         │                 │                 │
                         ▼                 ▼                 ▼
                  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐
                  │    Skill     │  │   Project    │  │    MyTask    │
                  ├──────────────┤  ├──────────────┤  ├──────────────┤
                  │ Id (PK)      │  │ Id (PK)      │  │ Id (PK)      │
                  │ skill_name   │  │ project_name │  │ we_id (FK)   │
                  │ position     │  │ description  │  │task_descrip  │
                  └──────────────┘  │ project_type │  │   tion       │
                                    │ is_Reality   │  └──────────────┘
                                    │ url_project  │
                                    │ url_demo     │
                                    │ url_github   │
                                    │ duration     │
                                    │ from         │
                                    │ to           │
                                    │ img_url      │
                                    │ url_contract │
                                    │ url_excel    │
                                    └──────┬───────┘
                                           │
                                           │ 1:N
                                           │
                                           ▼
                                    ┌──────────────┐
                                    │     Tech     │
                                    ├──────────────┤
                                    │ Id (PK)      │
                                    │ project_id   │
                                    │   (FK)       │
                                    │ tech_name    │
                                    └──────────────┘
```

### Chi tiết các bảng

#### 1. **AspUser** (Bảng người dùng chính)

- **Mô tả**: Bảng lưu thông tin người dùng, kế thừa từ `IdentityUser` của ASP.NET Core Identity
- **Các trường chính**:
  - `Id` (string, PK): ID người dùng (GUID)
  - `UserName`, `Email`: Từ IdentityUser
  - `fullname` (string, required): Họ và tên
  - `university_name`: Tên trường đại học
  - `address`: Địa chỉ
  - `phone_number`: Số điện thoại
  - `github`: Link GitHub
  - `dob`: Ngày sinh
  - `avatar`: URL ảnh đại diện
  - `position_career`: Vị trí công việc
  - `expOfYear`: Số năm kinh nghiệm
  - `background`: Tiểu sử
  - `mindset`: Tư duy/philosophy
  - `linkedin_url`, `facebook_url`: Social links
  - `GPA`: Điểm trung bình
  - **Dynamic Routing Fields (MỚI)**:
    - `username` (string, unique, nullable): Username cho public profile (3-30 ký tự, a-z, 0-9, -, \_)
    - `slug` (string, nullable): URL-friendly slug
    - `is_public` (bool, default: true): Portfolio visibility
    - `username_changed_count` (int, default: 0): Số lần đổi username
    - `last_username_change_date` (DateTime?, nullable): Ngày đổi username lần cuối
- **Quan hệ**:
  - 1:N với `RefreshToken`
  - 1:N với `UserSkill`
  - 1:N với `UserProject`
  - 1:N với `WorkExperience`
  - 1:N với `Certificate`

#### 2. **RefreshToken** (Bảng refresh token)

- **Mô tả**: Lưu refresh tokens cho JWT authentication
- **Các trường**:
  - `Id` (long, PK): ID token
  - `Token` (string): Refresh token string
  - `ExpireAt` (DateTime): Thời gian hết hạn
  - `CreatedAt` (DateTime): Thời gian tạo
  - `CreatedByIp` (string): IP tạo token
  - `Revoked` (DateTime?): Thời gian revoke
  - `RevokedByIp` (string): IP revoke
  - `ReplacedByToken` (string): Token thay thế
  - `ReasonRevoked` (string): Lý do revoke
  - `AspUserId` (string, FK): ID người dùng
- **Quan hệ**: N:1 với `AspUser`

#### 3. **Skill** (Bảng kỹ năng)

- **Mô tả**: Danh sách các kỹ năng có thể có
- **Các trường**:
  - `Id` (string, PK): ID kỹ năng
  - `skill_name` (string, required): Tên kỹ năng
  - `position` (string): Vị trí áp dụng (Frontend, Backend, Fullstack, etc.)
- **Quan hệ**: 1:N với `UserSkill`

#### 4. **UserSkill** (Bảng liên kết User-Skill)

- **Mô tả**: Bảng trung gian liên kết User và Skill (Many-to-Many)
- **Các trường**:
  - `Id` (string, PK): ID
  - `user_id` (string, FK): ID người dùng
  - `skill_id` (string, FK): ID kỹ năng
- **Quan hệ**: N:1 với `AspUser`, N:1 với `Skill`

#### 5. **Project** (Bảng dự án)

- **Mô tả**: Thông tin các dự án
- **Các trường**:
  - `Id` (string, PK): ID dự án
  - `project_name` (string, required): Tên dự án
  - `description`: Mô tả dự án
  - `project_type`: Loại dự án
  - `is_Reality` (bool): Dự án thực tế hay không
  - `url_project`: URL dự án
  - `url_demo`: URL demo
  - `url_github`: URL GitHub
  - `duration`: Thời gian thực hiện
  - `from`, `to`: Thời gian bắt đầu/kết thúc
  - `img_url`: URL ảnh dự án
  - `url_contract`: URL hợp đồng
  - `url_excel`: URL file Excel
- **Quan hệ**:
  - 1:N với `UserProject`
  - 1:N với `Tech`

#### 6. **UserProject** (Bảng liên kết User-Project)

- **Mô tả**: Bảng trung gian liên kết User và Project
- **Các trường**:
  - `Id` (string, PK): ID
  - `user_id` (string, FK): ID người dùng
  - `project_id` (string, FK): ID dự án
- **Quan hệ**: N:1 với `AspUser`, N:1 với `Project`

#### 7. **Tech** (Bảng công nghệ)

- **Mô tả**: Các công nghệ sử dụng trong dự án
- **Các trường**:
  - `Id` (string, PK): ID
  - `project_id` (string, FK): ID dự án
  - `tech_name` (string, required): Tên công nghệ
- **Quan hệ**: N:1 với `Project`

#### 8. **WorkExperience** (Bảng kinh nghiệm làm việc)

- **Mô tả**: Kinh nghiệm làm việc của người dùng
- **Các trường**:
  - `Id` (string, PK): ID
  - `user_id` (string, FK): ID người dùng
  - `company_name` (string, required): Tên công ty
  - `position`: Vị trí công việc
  - `duration`: Thời gian làm việc
  - `description`: Mô tả công việc
  - `project_id`: ID dự án liên quan
- **Quan hệ**:
  - N:1 với `AspUser`
  - 1:N với `MyTask`

#### 9. **MyTask** (Bảng nhiệm vụ)

- **Mô tả**: Các nhiệm vụ trong kinh nghiệm làm việc
- **Các trường**:
  - `Id` (string, PK): ID
  - `we_id` (string, FK): ID work experience
  - `task_description`: Mô tả nhiệm vụ
- **Quan hệ**: N:1 với `WorkExperience`

#### 10. **Certificate** (Bảng chứng chỉ)

- **Mô tả**: Các chứng chỉ của người dùng
- **Các trường**:
  - `Id` (string, PK): ID
  - `user_id` (string, FK): ID người dùng
  - `certificate_name`: Tên chứng chỉ
- **Quan hệ**: N:1 với `AspUser`

### Indexes và Constraints

- `RefreshToken.CreatedByIp` có index
- Foreign keys có cascade delete cho một số quan hệ
- Unique constraints trên `AspUser.UserName` và `AspUser.Email` (từ Identity)
- **MỚI**: Unique index trên `AspUser.username` (chỉ cho non-null values)
- **MỚI**: Composite index trên `(username, is_public)` để tối ưu query public profiles

---

## 🏗️ CẤU TRÚC DỰ ÁN

### Kiến trúc Clean Architecture

Dự án được tổ chức theo kiến trúc Clean Architecture với các layer độc lập:

```
┌─────────────────────────────────────────────────────────┐
│                    ZEN.Startup                          │
│              (Entry Point, Program.cs)                  │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                  ZEN.Controller                         │
│  (API Endpoints, Middleware, Configurations)            │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                 ZEN.Application                         │
│        (Use Cases, Business Logic, MediatR)             │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                  ZEN.Domain                            │
│    (Entities, Interfaces, Domain Logic, DTOs)          │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│              ZEN.Infrastructure                         │
│  (External Services, Redis, Storage, Email, etc.)       │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│          ZEN.Infrastructure.Mysql                      │
│    (DbContext, Repositories, Migrations)               │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│                 ZEN.Contract                          │
│              (DTOs, Request/Response)                   │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│                 ZEN.CoreLib                            │
│              (Shared Libraries)                         │
└─────────────────────────────────────────────────────────┘
```

### Chi tiết các layer

#### 1. **ZEN.Startup** (Presentation Layer)

- **Vai trò**: Entry point của ứng dụng
- **Nội dung**:
  - `Program.cs`: Cấu hình và khởi động ứng dụng
  - `appsettings.json`: Cấu hình ứng dụng
  - Dependency injection setup
  - Middleware pipeline configuration
- **Phụ thuộc**: ZEN.Controller

#### 2. **ZEN.Controller** (API Layer)

- **Vai trò**: Xử lý HTTP requests/responses
- **Cấu trúc**:
  ```
  ZEN.Controller/
  ├── Endpoints/V1/          # API endpoints (Minimal APIs)
  │   ├── AccountEndpoint.cs
  │   ├── ProjectEndpoint.cs
  │   ├── SkillEndpoint.cs
  │   ├── UserEndpoint.cs
  │   ├── WorkExpEndpoint.cs
  │   ├── CertificateEndpoint.cs
  │   ├── MyTaskEndpoint.cs
  │   └── SendMailEndpoint.cs
  ├── Configurations/        # Cấu hình (CORS, JWT, Swagger, etc.)
  ├── Middlewares/           # Custom middlewares
  ├── Extensions/            # Extension methods
  └── Types/                 # Response types
  ```
- **Phụ thuộc**: ZEN.Application, ZEN.Contract

#### 3. **ZEN.Application** (Application Layer)

- **Vai trò**: Business logic và use cases
- **Cấu trúc**:
  ```
  ZEN.Application/
  ├── Usecases/
  │   ├── CertificateUC/     # Certificate use cases
  │   ├── MyTaskUC/          # MyTask use cases
  │   ├── ProjectUC/         # Project use cases
  │   ├── SendMailUC/        # Email use cases
  │   ├── SkillUC/           # Skill use cases
  │   ├── UserUC/           # User use cases
  │   └── WorkExperienceUC/  # WorkExperience use cases
  ├── Services/              # Application services
  ├── Core/                  # Core behaviors (MediatR, Validation)
  └── Application.cs         # Application setup
  ```
- **Pattern**: CQRS với MediatR
- **Phụ thuộc**: ZEN.Domain, ZEN.Contract

#### 4. **ZEN.Domain** (Domain Layer)

- **Vai trò**: Domain entities và business rules
- **Cấu trúc**:
  ```
  ZEN.Domain/
  ├── Entities/Identities/   # Domain entities
  │   ├── AspUser.cs
  │   ├── Project.cs
  │   ├── Skill.cs
  │   ├── WorkExperience.cs
  │   ├── Certificate.cs
  │   ├── MyTask.cs
  │   ├── Tech.cs
  │   ├── UserSkill.cs
  │   ├── UserProject.cs
  │   └── RefreshToken.cs
  ├── Interfaces/            # Domain interfaces
  ├── Services/              # Domain service interfaces
  ├── Common/               # Common abstractions
  ├── Definition/           # Constants, enums
  └── DTO/                  # Domain DTOs
  ```
- **Phụ thuộc**: Không phụ thuộc layer nào (Pure Domain)

#### 5. **ZEN.Infrastructure** (Infrastructure Layer)

- **Vai trò**: Implementations của external services
- **Cấu trúc**:
  ```
  ZEN.Infrastructure/
  ├── Integrations/
  │   ├── CloudStorage/      # S3-compatible storage
  │   ├── Redis/             # Redis cache
  │   ├── SendMail/          # Email service
  │   ├── SimpleStorage/     # Simple storage service
  │   └── ProvinceOpenAPI/   # External API integration
  ├── Persistence/           # Unit of Work pattern
  └── Common/               # Common infrastructure code
  ```
- **Phụ thuộc**: ZEN.Domain

#### 6. **ZEN.Infrastructure.Mysql** (Data Layer)

- **Vai trò**: Database access
- **Cấu trúc**:
  ```
  ZEN.Infrastructure.Mysql/
  ├── Persistence/
  │   ├── AppDbContext.cs    # EF Core DbContext
  │   └── Repositories/      # Repository implementations
  └── Migrations/            # EF Core migrations
  ```
- **Phụ thuộc**: ZEN.Domain, ZEN.Infrastructure

#### 7. **ZEN.Contract** (Contract Layer)

- **Vai trò**: DTOs và contracts cho API
- **Cấu trúc**:
  ```
  ZEN.Contract/
  ├── AspAccountDto/        # Authentication DTOs
  ├── ProjectDto/           # Project DTOs
  ├── SkillDto/             # Skill DTOs
  ├── CertificateDto/       # Certificate DTOs
  ├── MyTaskDto/            # MyTask DTOs
  ├── WEDTO/                # WorkExperience DTOs
  ├── HRSendMailDto/        # Email DTOs
  └── ResponsePagination/   # Pagination response
  ```
- **Phụ thuộc**: Không có

#### 8. **ZEN.CoreLib** (Shared Library)

- **Vai trò**: Shared libraries và utilities
- **Phụ thuộc**: Không có

### Design Patterns sử dụng

1. **Repository Pattern**: Abstract data access
2. **Unit of Work Pattern**: Transaction management
3. **CQRS**: Command Query Responsibility Segregation với MediatR
4. **Dependency Injection**: IoC container
5. **Factory Pattern**: Entity creation
6. **Strategy Pattern**: Service implementations

### Công nghệ và thư viện chính

- **.NET 9.0**: Framework
- **Entity Framework Core**: ORM
- **PostgreSQL**: Database
- **ASP.NET Core Identity**: Authentication
- **JWT**: Token-based authentication
- **MediatR**: CQRS pattern
- **Swagger/OpenAPI**: API documentation
- **Quartz.NET**: Background jobs
- **HybridCache**: Caching
- **Redis**: Distributed cache (optional)

### Flow xử lý request

```
1. HTTP Request
   ↓
2. ZEN.Controller (Endpoint)
   ↓
3. MediatR (Mediator Pattern)
   ↓
4. ZEN.Application (Use Case Handler)
   ↓
5. ZEN.Domain (Domain Logic)
   ↓
6. ZEN.Infrastructure.Mysql (Repository)
   ↓
7. Database (PostgreSQL)
   ↓
8. Response (ngược lại)
```

### Migration và Database Management

- Migrations được quản lý bằng EF Core
- Tự động apply migrations khi chạy trong Development mode
- Migration files nằm trong `ZEN.Infrastructure.Mysql/Migrations/`
- Sử dụng code-first approach
