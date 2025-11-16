# 📚 API Endpoints Documentation - Portfolio Backend

**Base URL**: `https://portfolio-be-k3b0.onrender.com`  
**API Version**: `v1`  
**Base Route**: `/api/v1`

---

## 📋 Mục lục

1. [Health Check](#health-check)
2. [Authentication](#authentication)
3. [Account Management](#account-management)
4. [User Profile](#user-profile)
5. [Skills](#skills)
6. [Projects](#projects)
7. [Work Experience](#work-experience)
8. [Certificates](#certificates)
9. [My Tasks](#my-tasks)
10. [Public API](#public-api)
11. [Send Mail](#send-mail)
12. [Error Responses](#error-responses)

---

## 1. Health Check

### 1.1. Health Check

**Endpoint**: `GET /healthcheck`

**Authentication**: Không cần

**Response** (200 OK):
```
Server is alive!
```

---

## 🔐 Authentication

Tất cả các API (trừ Public API và Account endpoints) yêu cầu **Bearer Token** trong header:

```
Authorization: Bearer YOUR_ACCESS_TOKEN
```

---

## 2. Account Management

### 2.1. Đăng ký (Register)

**Endpoint**: `POST /api/v1/account/dev-register`

**Authentication**: Không cần

**Request Body**:
```json
{
  "username": "string",      // Required, 3-30 chars, a-z, 0-9, -, _
  "password": "string",       // Required
  "email": "string",          // Optional
  "fullName": "string"        // Optional
}
```

**Response** (201 Created):
```json
{
  "user_id": "string",
  "username": "string",
  "email": "string",
  "token": {
    "accessToken": "string",
    "refreshToken": "string",
    "expiresIn": 144,
    "createdAt": "2024-01-01T00:00:00Z"
  },
  "user_name": "string"
}
```

**Error Codes**:
- `INVALID_REQUEST`: Username hoặc password thiếu
- `INVALID_USERNAME`: Username không đúng format
- `USERNAME_TAKEN`: Username đã tồn tại
- `EMAIL_TAKEN`: Email đã tồn tại
- `REGISTRATION_FAILED`: Lỗi khi tạo user

---

### 2.2. Đăng nhập (Login)

**Endpoint**: `POST /api/v1/account/log-in`

**Authentication**: Không cần

**Request Body**:
```json
{
  "username": "string",  // Username hoặc Email
  "password": "string"
}
```

**Response** (200 OK):
```json
{
  "token": {
    "accessToken": "string",
    "refreshToken": "string",
    "expiresIn": 144,
    "createdAt": "2024-01-01T00:00:00Z"
  },
  "user_name": "string",
  "user_id": "string",
  "email": "string"
}
```

**Error Codes**:
- `401`: Username không tồn tại hoặc password sai
- `423`: Account bị lockout
- `428`: Yêu cầu 2FA

---

### 2.3. Refresh Token

**Endpoint**: `POST /api/v1/token/refresh`

**Authentication**: Không cần

**Request Body**:
```json
{
  "refreshToken": "string"
}
```

**Response** (200 OK):
```json
{
  "accessToken": "string",
  "refreshToken": "string",
  "expiresIn": 144,
  "createdAt": "2024-01-01T00:00:00Z"
}
```

---

### 2.4. Get Claims

**Endpoint**: `GET /api/v1/token/claims`

**Authentication**: Required (Bearer Token)

**Response** (200 OK):
```json
{
  "sub": "user_id",
  "name": "username",
  "email": "user@example.com",
  ...
}
```

---

## 3. User Profile

### 3.1. Get Profile

**Endpoint**: `GET /api/v1/profile`

**Authentication**: Required

**Response** (200 OK):
```json
{
  "user_id": "string",
  "fullname": "string",
  "email": "string",
  "username": "string",
  "avatar": "string",
  "phone_number": "string",
  "address": "string",
  "university_name": "string",
  "is_public": true,
  ...
}
```

---

### 3.2. Update Profile

**Endpoint**: `PATCH /api/v1/profile/{user_id}`

**Authentication**: Required

**Content-Type**: `multipart/form-data`

**Request Body** (FormData):
```
fullname: string (optional)
email: string (optional)
phone_number: string (optional)
address: string (optional)
university_name: string (optional)
avatar: File (optional, image file)
```

**Response** (200 OK):
```json
{
  "user_id": "string",
  "fullname": "string",
  "email": "string",
  "avatar": "string",
  ...
}
```

**Error Codes**:
- `404`: User không tồn tại
- `401`: Không có quyền

---

### 3.3. Update Username

**Endpoint**: `PATCH /api/v1/profile/username`

**Authentication**: Required

**Request Body**:
```json
{
  "username": "string"  // 3-30 chars, a-z, 0-9, -, _
}
```

**Response** (200 OK):
```json
{
  "message": "Username updated successfully",
  "username": "new_username"
}
```

**Error Codes**:
- `USERNAME_TAKEN`: Username đã tồn tại
- `USERNAME_CHANGE_LIMIT`: Đã đổi username 3 lần trong năm hoặc chưa đủ 30 ngày
- `INVALID_USERNAME`: Username không đúng format
- `USER_NOT_FOUND`: User không tồn tại

**Lưu ý**:
- Chỉ được đổi username tối đa 3 lần/năm
- Phải cách lần đổi trước ít nhất 30 ngày

---

### 3.4. Update Visibility

**Endpoint**: `PATCH /api/v1/profile/visibility`

**Authentication**: Required

**Request Body**:
```json
{
  "is_public": true  // true = public, false = private
}
```

**Response** (200 OK):
```json
{
  "message": "Visibility updated successfully",
  "is_public": true
}
```

---

## 4. Skills

### 4.1. Create Skill

**Endpoint**: `POST /api/v1/skill` hoặc `POST /api/v1/skill/add-skill`

**Authentication**: Required

**Request Body**:
```json
{
  "skill_name": "string",    // Required
  "position": "string"        // Required
}
```

**Response** (200 OK):
```json
{
  "skill_id": "string",
  "skill_name": "string",
  "position": "string",
  "user_id": "string"
}
```

---

### 4.2. Get All Skills

**Endpoint**: `GET /api/v1/skill`

**Authentication**: Required

**Response** (200 OK):
```json
[
  {
    "skill_id": "string",
    "skill_name": "string",
    "position": "string"
  },
  ...
]
```

---

### 4.3. Update Skill

**Endpoint**: `PATCH /api/v1/skill/{skill_id}`

**Authentication**: Required

**Request Body**:
```json
{
  "skill_name": "string",
  "position": "string"
}
```

**Response** (200 OK):
```json
{
  "skill_id": "string",
  "skill_name": "string",
  "position": "string"
}
```

---

### 4.4. Delete All Skills

**Endpoint**: `DELETE /api/v1/skill`

**Authentication**: Required

**Response** (200 OK):
```json
{
  "message": "All skills deleted successfully"
}
```

---

### 4.5. Delete Specific Skill

**Endpoint**: `DELETE /api/v1/skill/remove/{skill_id}`

**Authentication**: Required

**Response** (200 OK):
```json
{
  "message": "Skill deleted successfully"
}
```

---

## 5. Projects

### 5.1. Get Projects

**Endpoint**: `GET /api/v1/project?page_index=1&page_size=10`

**Authentication**: Required

**Query Parameters**:
- `page_index` (int, default: 1)
- `page_size` (int, default: 10)

**Response** (200 OK):
```json
{
  "data": [
    {
      "project_id": "string",
      "project_name": "string",
      "description": "string",
      "img_url": "string",
      "tech": [
        {
          "tech_name": "string"
        }
      ],
      "link": "string"
    }
  ],
  "total": 10,
  "page_index": 1,
  "page_size": 10
}
```

---

### 5.2. Create Project

**Endpoint**: `POST /api/v1/project/create-project`

**Authentication**: Required

**Content-Type**: `multipart/form-data`

**Request Body** (FormData):
```
project_name: string (required)
description: string (optional)
img_url: File (optional, image file)
tech: string (JSON array string, e.g., '[{"tech_name":"React"},{"tech_name":"Node.js"}]')
link: string (optional)
```

**Response** (200 OK):
```json
{
  "project_id": "string",
  "project_name": "string",
  "description": "string",
  "img_url": "string",
  "tech": [...],
  "link": "string"
}
```

---

### 5.3. Update Project

**Endpoint**: `PATCH /api/v1/project/{project_id}`

**Authentication**: Required

**Content-Type**: `multipart/form-data`

**Request Body** (FormData):
```
project_name: string (optional)
description: string (optional)
img_url: File (optional, image file)
tech: string (JSON array string, optional)
link: string (optional)
```

**Response** (200 OK):
```json
{
  "project_id": "string",
  "project_name": "string",
  ...
}
```

**Error Codes**:
- `404`: Project không tồn tại
- `401`: Không có quyền

---

### 5.4. Delete Project

**Endpoint**: `DELETE /api/v1/project/{project_id}`

**Authentication**: Required

**Response** (200 OK):
```json
{
  "message": "Project deleted successfully"
}
```

**Error Codes**:
- `404`: Project không tồn tại
- `401`: Không có quyền

---

## 6. Work Experience

### 6.1. Add Work Experience

**Endpoint**: `POST /api/v1/workexp`

**Authentication**: Required

**Request Body**:
```json
{
  "company_name": "string",
  "position": "string",
  "start_date": "2024-01-01",
  "end_date": "2024-12-31",  // null nếu đang làm
  "description": "string"
}
```

**Response** (200 OK):
```json
{
  "we_id": "string",
  "company_name": "string",
  "position": "string",
  "start_date": "2024-01-01",
  "end_date": "2024-12-31",
  "description": "string"
}
```

---

### 6.2. Get All Work Experience

**Endpoint**: `GET /api/v1/workexp?page_index=1&page_size=10`

**Authentication**: Required

**Query Parameters**:
- `page_index` (int, default: 1)
- `page_size` (int, default: 10)

**Response** (200 OK):
```json
{
  "data": [
    {
      "we_id": "string",
      "company_name": "string",
      "position": "string",
      "start_date": "2024-01-01",
      "end_date": "2024-12-31",
      "description": "string",
      "tasks": [...]
    }
  ],
  "total": 10,
  "page_index": 1,
  "page_size": 10
}
```

---

### 6.3. Update Work Experience

**Endpoint**: `PATCH /api/v1/workexp/{we_id}`

**Authentication**: Required

**Request Body**:
```json
{
  "company_name": "string",
  "position": "string",
  "start_date": "2024-01-01",
  "end_date": "2024-12-31",
  "description": "string"
}
```

**Response** (200 OK):
```json
{
  "we_id": "string",
  "company_name": "string",
  ...
}
```

---

### 6.4. Delete Work Experience

**Endpoint**: `DELETE /api/v1/workexp/{we_id}`

**Authentication**: Required

**Response** (200 OK):
```json
{
  "message": "Work experience deleted successfully"
}
```

---

## 7. Certificates

### 7.1. Add Certificate

**Endpoint**: `POST /api/v1/certificate`

**Authentication**: Required

**Request Body**:
```json
{
  "certificate_name": "string",
  "issuing_organization": "string",
  "issue_date": "2024-01-01",
  "expiry_date": "2025-01-01",  // null nếu không hết hạn
  "credential_id": "string",
  "credential_url": "string"
}
```

**Response** (200 OK):
```json
{
  "certificate_id": "string",
  "certificate_name": "string",
  "issuing_organization": "string",
  "issue_date": "2024-01-01",
  "expiry_date": "2025-01-01",
  "credential_id": "string",
  "credential_url": "string"
}
```

---

### 7.2. Update Certificate

**Endpoint**: `PATCH /api/v1/certificate/{certificate_id}`

**Authentication**: Required

**Request Body**:
```json
{
  "certificate_name": "string",
  "issuing_organization": "string",
  "issue_date": "2024-01-01",
  "expiry_date": "2025-01-01",
  "credential_id": "string",
  "credential_url": "string"
}
```

**Response** (200 OK):
```json
{
  "certificate_id": "string",
  ...
}
```

---

### 7.3. Delete Certificate

**Endpoint**: `DELETE /api/v1/certificate/{certificate_id}`

**Authentication**: Required

**Response** (200 OK):
```json
{
  "message": "Certificate deleted successfully"
}
```

---

## 8. My Tasks

### 8.1. Add Task to Work Experience

**Endpoint**: `POST /api/v1/mytask/{we_id}`

**Authentication**: Required

**Request Body**:
```json
{
  "task_description": "string"
}
```

**Response** (200 OK):
```json
{
  "mt_id": "string",
  "task_description": "string",
  "we_id": "string"
}
```

---

### 8.2. Update Task

**Endpoint**: `PATCH /api/v1/mytask/update/{we_id}`

**Authentication**: Required

**Request Body**:
```json
{
  "task_description": "string"
}
```

**Response** (200 OK):
```json
{
  "mt_id": "string",
  "task_description": "string"
}
```

---

### 8.3. Delete Task

**Endpoint**: `DELETE /api/v1/mytask/{mt_id}`

**Authentication**: Required

**Response** (200 OK):
```json
{
  "message": "Task deleted successfully"
}
```

---

## 9. Public API

Tất cả Public API **KHÔNG CẦN** authentication.

### 9.1. Get Public Profile

**Endpoint**: `GET /api/v1/public/profile/{username}`

**Authentication**: Không cần

**Response** (200 OK):
```json
{
  "username": "string",
  "fullname": "string",
  "avatar": "string",
  "email": "string",  // Chỉ hiển thị nếu user cho phép
  "phone_number": "string",
  "address": "string",
  "university_name": "string"
}
```

**Error Codes**:
- `USER_NOT_FOUND` (404): User không tồn tại
- `PORTFOLIO_PRIVATE` (403): Portfolio đang ở chế độ private

---

### 9.2. Get Public Work Experience

**Endpoint**: `GET /api/v1/public/profile/{username}/workexp?page_index=1&page_size=10`

**Authentication**: Không cần

**Query Parameters**:
- `page_index` (int, default: 1)
- `page_size` (int, default: 10)

**Response** (200 OK):
```json
{
  "data": [
    {
      "company_name": "string",
      "position": "string",
      "start_date": "2024-01-01",
      "end_date": "2024-12-31",
      "description": "string",
      "tasks": [...]
    }
  ],
  "total": 10,
  "page_index": 1,
  "page_size": 10
}
```

---

### 9.3. Get Public Projects

**Endpoint**: `GET /api/v1/public/profile/{username}/projects?page_index=1&page_size=10`

**Authentication**: Không cần

**Query Parameters**:
- `page_index` (int, default: 1)
- `page_size` (int, default: 10)

**Response** (200 OK):
```json
{
  "data": [
    {
      "project_name": "string",
      "description": "string",
      "img_url": "string",
      "tech": [...],
      "link": "string"
    }
  ],
  "total": 10,
  "page_index": 1,
  "page_size": 10
}
```

---

### 9.4. Get Public Skills

**Endpoint**: `GET /api/v1/public/profile/{username}/skills`

**Authentication**: Không cần

**Response** (200 OK):
```json
[
  {
    "skill_name": "string",
    "position": "string"
  },
  ...
]
```

---

### 9.5. Get Public Certificates

**Endpoint**: `GET /api/v1/public/profile/{username}/certificates`

**Authentication**: Không cần

**Response** (200 OK):
```json
[
  {
    "certificate_name": "string",
    "issuing_organization": "string",
    "issue_date": "2024-01-01",
    "expiry_date": "2025-01-01",
    "credential_id": "string",
    "credential_url": "string"
  },
  ...
]
```

---

### 9.6. Check Username Availability

**Endpoint**: `GET /api/v1/public/check-username/{username}`

**Authentication**: Không cần

**Response** (200 OK):
```json
{
  "username": "string",
  "available": true,
  "message": "Username is available"
}
```

**Response** (200 OK - Not Available):
```json
{
  "username": "string",
  "available": false,
  "message": "Username is already taken"
}
```

---

## 10. Send Mail

### 10.1. Send Contact Email

**Endpoint**: `POST /api/v1/email`

**Authentication**: Required

**Request Body**:
```json
{
  "hrName": "string",       // Required
  "hrEmail": "string",      // Required
  "hrCompany": "string",    // Required
  "hrNotes": "string",      // Required
  "hrPhone": "string"       // Optional
}
```

**Lưu ý**: `user_email` và `user_name` sẽ được tự động lấy từ user đang đăng nhập, không cần gửi trong request.

**Response** (200 OK):
```json
{
  "message": "Email sent successfully"
}
```

**Error Codes**:
- `400`: Request không hợp lệ
- `500`: Lỗi server khi gửi email

---

## 11. Error Responses

### Standard Error Format

```json
{
  "error": "ERROR_CODE",
  "message": "Error message description"
}
```

### Common Error Codes

| Code | Status | Mô tả |
|------|--------|-------|
| `INVALID_REQUEST` | 400 | Request không hợp lệ |
| `INVALID_USERNAME` | 400 | Username không đúng format |
| `USERNAME_TAKEN` | 409 | Username đã tồn tại |
| `EMAIL_TAKEN` | 409 | Email đã tồn tại |
| `USER_NOT_FOUND` | 404 | User không tồn tại |
| `PORTFOLIO_PRIVATE` | 403 | Portfolio đang private |
| `USERNAME_CHANGE_LIMIT` | 400 | Đã đổi username quá 3 lần/năm |
| `UNAUTHORIZED` | 401 | Chưa đăng nhập hoặc token hết hạn |
| `FORBIDDEN` | 403 | Không có quyền truy cập |

---

## 12. Notes

### Authentication
- Tất cả API (trừ Public API) yêu cầu Bearer Token
- Token có thể lấy từ Login hoặc Register
- Token hết hạn thì dùng Refresh Token để lấy token mới

### File Upload
- Chỉ hỗ trợ image files cho avatar và project images
- Images sẽ được upload lên Cloudinary
- Images sẽ được compress và resize tự động

### Pagination
- Các API có pagination: Projects, Work Experience
- `page_index`: Bắt đầu từ 1
- `page_size`: Số items mỗi trang (default: 10)

### Username Rules
- Độ dài: 3-30 ký tự
- Chỉ cho phép: a-z, 0-9, -, _
- Case-insensitive
- Chỉ được đổi tối đa 3 lần/năm
- Phải cách lần đổi trước ít nhất 30 ngày

### Date Format
- Tất cả dates dùng format: `YYYY-MM-DD`
- Timezone: UTC

---

## 13. Examples

### Example: Register và Login

```bash
# 1. Register
curl -X POST https://portfolio-be-k3b0.onrender.com/api/v1/account/dev-register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "johndoe",
    "password": "password123",
    "email": "john@example.com",
    "fullName": "John Doe"
  }'

# 2. Login
curl -X POST https://portfolio-be-k3b0.onrender.com/api/v1/account/log-in \
  -H "Content-Type: application/json" \
  -d '{
    "username": "johndoe",
    "password": "password123"
  }'
```

### Example: Create Skill

```bash
curl -X POST https://portfolio-be-k3b0.onrender.com/api/v1/skill \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "skill_name": "React",
    "position": "Frontend Developer"
  }'
```

### Example: Get Public Profile

```bash
curl -X GET https://portfolio-be-k3b0.onrender.com/api/v1/public/profile/johndoe
```

---

**Last Updated**: 2024-11-15  
**API Version**: v1

