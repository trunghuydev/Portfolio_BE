# API Endpoints Documentation - Portfolio Backend

**Base URL**: `https://portfolio-be-k3b0.onrender.com`  
**API Version**: `v1`  
**Base Route**: `/api/v1`

---

## Mục lục

1. [Health Check](#1-health-check)
2. [Authentication](#2-authentication)
3. [Account Management](#3-account-management)
4. [User Profile](#4-user-profile)
5. [Skills](#5-skills)
6. [Projects](#6-projects)
7. [Work Experience](#7-work-experience)
8. [Certificates](#8-certificates)
9. [My Tasks](#9-my-tasks)
10. [Public API](#10-public-api)
11. [Send Mail](#11-send-mail)
12. [Error Responses](#12-error-responses)
13. [Notes](#13-notes)
14. [Examples](#14-examples)

---

## 1. Health Check

### 1.1. Health Check

**Endpoint**: `GET /healthcheck`

**Tác dụng**: Kiểm tra trạng thái hoạt động của server. API này không yêu cầu authentication và có thể được sử dụng để monitor server health.

**Authentication**: Không cần

**Cách sử dụng**: Gửi GET request đến endpoint `/healthcheck` để kiểm tra server có đang hoạt động không.

**Response** (200 OK):

```
Server is alive!
```

---

## 2. Authentication

Tất cả các API (trừ Public API và Account endpoints) yêu cầu **Bearer Token** trong header:

```
Authorization: Bearer YOUR_ACCESS_TOKEN
```

Token có thể lấy được từ API Login hoặc Register. Khi token hết hạn, sử dụng Refresh Token API để lấy token mới.

---

## 3. Account Management

### 3.1. Đăng ký (Register)

**Endpoint**: `POST /api/v1/account/dev-register`

**Tác dụng**: Tạo tài khoản mới cho user. Sau khi đăng ký thành công, hệ thống sẽ tự động trả về access token và refresh token để user có thể sử dụng ngay.

**Authentication**: Không cần

**Cách sử dụng**: Gửi POST request với thông tin đăng ký. Username và password là bắt buộc, email và fullName là tùy chọn.

**Request Body**:

```json
{
  "username": "string", // Required, 3-30 chars, a-z, 0-9, -, _
  "password": "string", // Required
  "email": "string", // Optional
  "fullName": "string" // Optional
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
- `INVALID_USERNAME`: Username không đúng format (phải 3-30 ký tự, chỉ chứa a-z, 0-9, -, \_)
- `USERNAME_TAKEN`: Username đã tồn tại
- `EMAIL_TAKEN`: Email đã tồn tại
- `REGISTRATION_FAILED`: Lỗi khi tạo user

---

### 3.2. Đăng nhập (Login)

**Endpoint**: `POST /api/v1/account/log-in`

**Tác dụng**: Đăng nhập vào hệ thống bằng username/email và password. Sau khi đăng nhập thành công, hệ thống trả về access token và refresh token.

**Authentication**: Không cần

**Cách sử dụng**: Gửi POST request với username (hoặc email) và password. Sử dụng token trả về trong header Authorization cho các request tiếp theo.

**Request Body**:

```json
{
  "username": "string", // Username hoặc Email
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

### 3.3. Refresh Token

**Endpoint**: `POST /api/v1/token/refresh`

**Tác dụng**: Lấy access token mới khi token cũ hết hạn. Sử dụng refresh token từ lần đăng nhập/đăng ký trước đó.

**Authentication**: Không cần

**Cách sử dụng**: Khi access token hết hạn (nhận được lỗi 401), gửi POST request với refresh token để lấy access token mới.

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

### 3.4. Get Claims

**Endpoint**: `GET /api/v1/token/claims`

**Tác dụng**: Lấy thông tin claims từ JWT token hiện tại. Dùng để xác minh thông tin user từ token.

**Authentication**: Required (Bearer Token)

**Cách sử dụng**: Gửi GET request với Bearer token trong header để lấy thông tin claims.

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

## 4. User Profile

### 4.1. Get Profile

**Endpoint**: `GET /api/v1/profile`

**Tác dụng**: Lấy thông tin profile đầy đủ của user hiện tại. Sử dụng để hiển thị thông tin user trên trang admin hoặc profile page.

**Authentication**: Required

**Cách sử dụng**: Gửi GET request với Bearer token. API sẽ tự động lấy thông tin của user từ token.

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

### 4.2. Update Profile

**Endpoint**: `PATCH /api/v1/profile/{user_id}`

**Tác dụng**: Cập nhật thông tin profile của user. Hỗ trợ upload avatar mới, khi upload avatar mới thì avatar cũ trên Cloudinary sẽ tự động bị xóa.

**Authentication**: Required

**Cách sử dụng**: Gửi PATCH request với multipart/form-data. Chỉ gửi các field muốn cập nhật, các field không gửi sẽ giữ nguyên giá trị cũ.

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
- `401`: Không có quyền (chỉ có thể update profile của chính mình)

**Lưu ý**: Khi upload avatar mới, avatar cũ trên Cloudinary sẽ tự động bị xóa để tiết kiệm dung lượng.

---

### 4.3. Update Username

**Endpoint**: `PATCH /api/v1/profile/username`

**Tác dụng**: Cập nhật username của user. Có giới hạn số lần đổi username để bảo mật.

**Authentication**: Required

**Cách sử dụng**: Gửi PATCH request với username mới. Username phải đúng format và chưa được sử dụng.

**Request Body**:

```json
{
  "username": "string" // 3-30 chars, a-z, 0-9, -, _
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
- `USERNAME_CHANGE_LIMIT`: Đã đổi username 3 lần trong năm hoặc chưa đủ 30 ngày kể từ lần đổi trước
- `INVALID_USERNAME`: Username không đúng format
- `USER_NOT_FOUND`: User không tồn tại

**Lưu ý**:

- Chỉ được đổi username tối đa 3 lần/năm
- Phải cách lần đổi trước ít nhất 30 ngày

---

### 4.4. Update Visibility

**Endpoint**: `PATCH /api/v1/profile/visibility`

**Tác dụng**: Thay đổi chế độ hiển thị portfolio (public hoặc private). Khi set private, portfolio sẽ không hiển thị trong Public API.

**Authentication**: Required

**Cách sử dụng**: Gửi PATCH request với giá trị is_public (true = public, false = private).

**Request Body**:

```json
{
  "is_public": true // true = public, false = private
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

## 5. Skills

### 5.1. Create Skill

**Endpoint**: `POST /api/v1/skill` hoặc `POST /api/v1/skill/add-skill`

**Tác dụng**: Tạo skill mới cho user. Skill sẽ được liên kết với user hiện tại thông qua bảng UserSkill.

**Authentication**: Required

**Cách sử dụng**: Gửi POST request với skill_name và position. Skill sẽ được tạo và gán cho user hiện tại.

**Request Body**:

```json
{
  "skill_name": "string", // Required
  "position": "string" // Required
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

### 5.2. Get All Skills

**Endpoint**: `GET /api/v1/skill`

**Tác dụng**: Lấy danh sách tất cả skills của user hiện tại. Sử dụng để hiển thị danh sách skills trên trang admin.

**Authentication**: Required

**Cách sử dụng**: Gửi GET request với Bearer token. API sẽ trả về tất cả skills thuộc về user hiện tại.

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

### 5.3. Get Skill by ID

**Endpoint**: `GET /api/v1/skill/{skill_id}`

**Tác dụng**: Lấy thông tin chi tiết của một skill cụ thể. Sử dụng để load thông tin skill khi chỉnh sửa trên trang admin.

**Authentication**: Required

**Cách sử dụng**: Gửi GET request với skill_id trong URL path. Chỉ có thể lấy skill thuộc về user hiện tại.

**Response** (200 OK):

```json
{
  "skill_id": "string",
  "skill_name": "string",
  "position": "string"
}
```

**Error Codes**:

- `404`: Skill không tồn tại hoặc không thuộc về user hiện tại
- `401`: Không có quyền truy cập

---

### 5.4. Update Skill

**Endpoint**: `PATCH /api/v1/skill/{skill_id}`

**Tác dụng**: Cập nhật thông tin skill. Chỉ có thể cập nhật skill thuộc về user hiện tại.

**Authentication**: Required

**Cách sử dụng**: Gửi PATCH request với skill_id trong URL path và thông tin cần cập nhật trong body.

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

**Error Codes**:

- `404`: Skill không tồn tại
- `401`: Không có quyền (chỉ có thể update skill của chính mình)

---

### 5.5. Delete All Skills

**Endpoint**: `DELETE /api/v1/skill`

**Tác dụng**: Xóa tất cả skills của user hiện tại. Sử dụng cẩn thận vì hành động này không thể hoàn tác.

**Authentication**: Required

**Cách sử dụng**: Gửi DELETE request để xóa tất cả skills của user hiện tại.

**Response** (200 OK):

```json
{
  "message": "All skills deleted successfully"
}
```

---

### 5.6. Delete Specific Skill

**Endpoint**: `DELETE /api/v1/skill/remove/{skill_id}`

**Tác dụng**: Xóa một skill cụ thể. Chỉ có thể xóa skill thuộc về user hiện tại.

**Authentication**: Required

**Cách sử dụng**: Gửi DELETE request với skill_id trong URL path.

**Response** (200 OK):

```json
{
  "message": "Skill deleted successfully"
}
```

**Error Codes**:

- `404`: Skill không tồn tại
- `401`: Không có quyền

---

## 6. Projects

### 6.1. Get Projects

**Endpoint**: `GET /api/v1/project?page_index=1&page_size=10`

**Tác dụng**: Lấy danh sách projects của user hiện tại với phân trang. Sử dụng để hiển thị danh sách projects trên trang admin.

**Authentication**: Required

**Cách sử dụng**: Gửi GET request với query parameters page_index và page_size để phân trang.

**Query Parameters**:

- `page_index` (int, default: 1): Số trang, bắt đầu từ 1
- `page_size` (int, default: 10): Số items mỗi trang

**Response** (200 OK):

```json
{
  "data": [
    {
      "project_id": "string",
      "project_name": "string",
      "description": "string",
      "img_url": "string",
      "project_type": "string",
      "is_Reality": true,
      "url_project": "string",
      "url_demo": "string",
      "url_github": "string",
      "duration": "string",
      "from": "string",
      "to": "string",
      "url_contract": "string",
      "url_excel": "string",
      "teches": [
        {
          "tech_name": "string"
        }
      ]
    }
  ],
  "total": 10,
  "page_index": 1,
  "page_size": 10
}
```

---

### 6.2. Get Project by ID

**Endpoint**: `GET /api/v1/project/{project_id}`

**Tác dụng**: Lấy thông tin chi tiết của một project cụ thể. Sử dụng để load thông tin project khi chỉnh sửa trên trang admin.

**Authentication**: Required

**Cách sử dụng**: Gửi GET request với project_id trong URL path. Chỉ có thể lấy project thuộc về user hiện tại.

**Response** (200 OK):

```json
{
  "project_id": "string",
  "project_name": "string",
  "description": "string",
  "img_url": "string",
  "project_type": "string",
  "is_Reality": true,
  "url_project": "string",
  "url_demo": "string",
  "url_github": "string",
  "duration": "string",
  "from": "string",
  "to": "string",
  "url_contract": "string",
  "url_excel": "string",
  "teches": [
    {
      "tech_name": "string"
    }
  ]
}
```

**Error Codes**:

- `404`: Project không tồn tại
- `401`: Không có quyền truy cập

---

### 6.3. Create Project

**Endpoint**: `POST /api/v1/project/create-project`

**Tác dụng**: Tạo project mới. Hỗ trợ upload ảnh project, ảnh sẽ được upload lên Cloudinary và tự động compress/resize.

**Authentication**: Required

**Cách sử dụng**: Gửi POST request với multipart/form-data. project_name là bắt buộc, các field khác là tùy chọn.

**Content-Type**: `multipart/form-data`

**Request Body** (FormData):

```
project_name: string (required)
description: string (optional)
img_url: File (optional, image file)
tech: string (JSON array string, e.g., '[{"tech_name":"React"},{"tech_name":"Node.js"}]')
project_type: string (optional)
is_Reality: boolean (optional)
url_project: string (optional)
url_demo: string (optional)
url_github: string (optional)
duration: string (optional)
from: string (optional)
to: string (optional)
url_contract: string (optional)
url_excel: string (optional)
```

**Response** (200 OK):

```json
{
  "project_id": "string",
  "project_name": "string",
  "description": "string",
  "img_url": "string",
  "teches": [...],
  ...
}
```

**Lưu ý**: Ảnh sẽ được tự động compress và resize tối đa 1024x1024px với quality 75% để tối ưu dung lượng.

---

### 6.4. Update Project

**Endpoint**: `PATCH /api/v1/project/{project_id}`

**Tác dụng**: Cập nhật thông tin project. Khi upload ảnh mới, ảnh cũ trên Cloudinary sẽ tự động bị xóa. Chỉ có thể cập nhật project thuộc về user hiện tại.

**Authentication**: Required

**Cách sử dụng**: Gửi PATCH request với multipart/form-data. Chỉ gửi các field muốn cập nhật, các field không gửi sẽ giữ nguyên giá trị cũ.

**Content-Type**: `multipart/form-data`

**Request Body** (FormData):

```
project_name: string (optional)
description: string (optional)
img_url: File (optional, image file)
tech: string (JSON array string, optional)
project_type: string (optional)
is_Reality: boolean (optional)
url_project: string (optional)
url_demo: string (optional)
url_github: string (optional)
duration: string (optional)
from: string (optional)
to: string (optional)
url_contract: string (optional)
url_excel: string (optional)
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
- `401`: Không có quyền (chỉ có thể update project của chính mình)

**Lưu ý**: Khi upload ảnh mới, ảnh cũ trên Cloudinary sẽ tự động bị xóa sau khi upload thành công để tiết kiệm dung lượng.

---

### 6.5. Delete Project

**Endpoint**: `DELETE /api/v1/project/{project_id}`

**Tác dụng**: Xóa project. Khi xóa project, ảnh trên Cloudinary cũng sẽ tự động bị xóa. Chỉ có thể xóa project thuộc về user hiện tại.

**Authentication**: Required

**Cách sử dụng**: Gửi DELETE request với project_id trong URL path.

**Response** (200 OK):

```json
{
  "message": "Project deleted successfully"
}
```

**Error Codes**:

- `404`: Project không tồn tại
- `401`: Không có quyền

**Lưu ý**: Khi xóa project, ảnh trên Cloudinary sẽ tự động bị xóa.

---

## 7. Work Experience

### 7.1. Add Work Experience

**Endpoint**: `POST /api/v1/workexp`

**Tác dụng**: Thêm work experience mới cho user. Work experience bao gồm thông tin công ty, vị trí, thời gian làm việc và mô tả.

**Authentication**: Required

**Cách sử dụng**: Gửi POST request với thông tin work experience. company_name là bắt buộc.

**Request Body**:

```json
{
  "company_name": "string",
  "position": "string",
  "duration": "string",
  "description": "string",
  "project_id": "string"
}
```

**Response** (200 OK):

```json
{
  "we_id": "string",
  "user_id": "string",
  "company_name": "string",
  "position": "string",
  "duration": "string",
  "description": "string",
  "project_id": "string"
}
```

---

### 7.2. Get All Work Experience

**Endpoint**: `GET /api/v1/workexp?page_index=1&page_size=10`

**Tác dụng**: Lấy danh sách tất cả work experiences của user hiện tại với phân trang. Bao gồm cả danh sách tasks của mỗi work experience.

**Authentication**: Required

**Cách sử dụng**: Gửi GET request với query parameters page_index và page_size để phân trang.

**Query Parameters**:

- `page_index` (int, default: 1): Số trang, bắt đầu từ 1
- `page_size` (int, default: 10): Số items mỗi trang

**Response** (200 OK):

```json
{
  "data": [
    {
      "we_id": "string",
      "user_id": "string",
      "company_name": "string",
      "position": "string",
      "duration": "string",
      "description": "string",
      "project_id": "string",
      "tasks": [
        {
          "mt_id": "string",
          "task_description": "string"
        }
      ]
    }
  ],
  "total": 10,
  "page_index": 1,
  "page_size": 10
}
```

---

### 7.3. Get Work Experience by ID

**Endpoint**: `GET /api/v1/workexp/{we_id}`

**Tác dụng**: Lấy thông tin chi tiết của một work experience cụ thể. Sử dụng để load thông tin work experience khi chỉnh sửa trên trang admin. Bao gồm cả danh sách tasks.

**Authentication**: Required

**Cách sử dụng**: Gửi GET request với we_id trong URL path. Chỉ có thể lấy work experience thuộc về user hiện tại.

**Response** (200 OK):

```json
{
  "we_id": "string",
  "user_id": "string",
  "company_name": "string",
  "position": "string",
  "duration": "string",
  "description": "string",
  "project_id": "string",
  "tasks": [
    {
      "mt_id": "string",
      "task_description": "string"
    }
  ]
}
```

**Error Codes**:

- `404`: Work experience không tồn tại hoặc không thuộc về user hiện tại
- `401`: Không có quyền truy cập

---

### 7.4. Update Work Experience

**Endpoint**: `PATCH /api/v1/workexp/{we_id}`

**Tác dụng**: Cập nhật thông tin work experience. Chỉ có thể cập nhật work experience thuộc về user hiện tại.

**Authentication**: Required

**Cách sử dụng**: Gửi PATCH request với we_id trong URL path và thông tin cần cập nhật trong body.

**Request Body**:

```json
{
  "company_name": "string",
  "position": "string",
  "duration": "string",
  "description": "string",
  "project_id": "string"
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

**Error Codes**:

- `404`: Work experience không tồn tại
- `401`: Không có quyền

---

### 7.5. Delete Work Experience

**Endpoint**: `DELETE /api/v1/workexp/{we_id}`

**Tác dụng**: Xóa work experience. Khi xóa work experience, tất cả tasks liên quan cũng sẽ bị xóa. Chỉ có thể xóa work experience thuộc về user hiện tại.

**Authentication**: Required

**Cách sử dụng**: Gửi DELETE request với we_id trong URL path.

**Response** (200 OK):

```json
{
  "message": "Work experience deleted successfully"
}
```

**Error Codes**:

- `404`: Work experience không tồn tại
- `401`: Không có quyền

**Lưu ý**: Khi xóa work experience, tất cả tasks liên quan cũng sẽ bị xóa.

---

## 8. Certificates

### 8.1. Add Certificate

**Endpoint**: `POST /api/v1/certificate`

**Tác dụng**: Thêm certificate mới cho user. Certificate sẽ được liên kết với user hiện tại.

**Authentication**: Required

**Cách sử dụng**: Gửi POST request với certificate_name. Certificate sẽ được tạo và gán cho user hiện tại.

**Request Body**:

```json
{
  "certificate_name": "string"
}
```

**Response** (200 OK):

```json
{
  "message": "User {fullname} adds his/her certificate successfully!"
}
```

---

### 8.2. Get All Certificates

**Endpoint**: `GET /api/v1/certificate`

**Tác dụng**: Lấy danh sách tất cả certificates của user hiện tại. Sử dụng để hiển thị danh sách certificates trên trang admin.

**Authentication**: Required

**Cách sử dụng**: Gửi GET request với Bearer token. API sẽ trả về tất cả certificates thuộc về user hiện tại.

**Response** (200 OK):

```json
[
  {
    "certificate_id": "string",
    "certificate_name": "string",
    "user_id": "string"
  },
  ...
]
```

---

### 8.3. Get Certificate by ID

**Endpoint**: `GET /api/v1/certificate/{certificate_id}`

**Tác dụng**: Lấy thông tin chi tiết của một certificate cụ thể. Sử dụng để load thông tin certificate khi chỉnh sửa trên trang admin.

**Authentication**: Required

**Cách sử dụng**: Gửi GET request với certificate_id trong URL path. Chỉ có thể lấy certificate thuộc về user hiện tại.

**Response** (200 OK):

```json
{
  "certificate_id": "string",
  "certificate_name": "string",
  "user_id": "string"
}
```

**Error Codes**:

- `404`: Certificate không tồn tại hoặc không thuộc về user hiện tại
- `401`: Không có quyền truy cập

---

### 8.4. Update Certificate

**Endpoint**: `PATCH /api/v1/certificate/{certificate_id}`

**Tác dụng**: Cập nhật thông tin certificate. Chỉ có thể cập nhật certificate thuộc về user hiện tại.

**Authentication**: Required

**Cách sử dụng**: Gửi PATCH request với certificate_id trong URL path và thông tin cần cập nhật trong body.

**Request Body**:

```json
{
  "certificate_name": "string"
}
```

**Response** (200 OK):

```json
{
  "message": "Certificate updated successfully!"
}
```

**Error Codes**:

- `404`: Certificate không tồn tại
- `401`: Không có quyền

---

### 8.5. Delete Certificate

**Endpoint**: `DELETE /api/v1/certificate/{certificate_id}`

**Tác dụng**: Xóa certificate. Chỉ có thể xóa certificate thuộc về user hiện tại.

**Authentication**: Required

**Cách sử dụng**: Gửi DELETE request với certificate_id trong URL path.

**Response** (200 OK):

```json
{
  "message": "Certificate deleted successfully"
}
```

**Error Codes**:

- `404`: Certificate không tồn tại
- `401`: Không có quyền

---

## 9. My Tasks

### 9.1. Add Task to Work Experience

**Endpoint**: `POST /api/v1/mytask/{we_id}`

**Tác dụng**: Thêm task mới vào một work experience. Task sẽ được liên kết với work experience thông qua we_id.

**Authentication**: Required

**Cách sử dụng**: Gửi POST request với we_id trong URL path và task_description trong body. Chỉ có thể thêm task vào work experience thuộc về user hiện tại.

**Request Body**:

```json
{
  "task_description": "string"
}
```

**Response** (200 OK):

```json
{
  "message": "User {fullname} adds his/her experience task successfully!"
}
```

**Error Codes**:

- `404`: Work experience không tồn tại
- `401`: Không có quyền (chỉ có thể thêm task vào work experience của chính mình)

---

### 9.2. Get All My Tasks

**Endpoint**: `GET /api/v1/mytask`

**Tác dụng**: Lấy danh sách tất cả tasks của user hiện tại (từ tất cả work experiences). Sử dụng để hiển thị danh sách tasks trên trang admin.

**Authentication**: Required

**Cách sử dụng**: Gửi GET request với Bearer token. API sẽ trả về tất cả tasks thuộc về các work experiences của user hiện tại.

**Response** (200 OK):

```json
[
  {
    "mt_id": "string",
    "we_id": "string",
    "task_description": "string"
  },
  ...
]
```

---

### 9.3. Get My Task by ID

**Endpoint**: `GET /api/v1/mytask/{mt_id}`

**Tác dụng**: Lấy thông tin chi tiết của một task cụ thể. Sử dụng để load thông tin task khi chỉnh sửa trên trang admin.

**Authentication**: Required

**Cách sử dụng**: Gửi GET request với mt_id trong URL path. Chỉ có thể lấy task thuộc về work experience của user hiện tại.

**Response** (200 OK):

```json
{
  "mt_id": "string",
  "we_id": "string",
  "task_description": "string"
}
```

**Error Codes**:

- `404`: Task không tồn tại hoặc không thuộc về user hiện tại
- `401`: Không có quyền truy cập

---

### 9.4. Update Task

**Endpoint**: `PATCH /api/v1/mytask/update/{we_id}`

**Tác dụng**: Cập nhật task của một work experience. Chỉ có thể cập nhật task thuộc về work experience của user hiện tại.

**Authentication**: Required

**Cách sử dụng**: Gửi PATCH request với we_id trong URL path và task_description mới trong body.

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

**Error Codes**:

- `404`: Work experience hoặc task không tồn tại
- `401`: Không có quyền

---

### 9.5. Delete Task

**Endpoint**: `DELETE /api/v1/mytask/{mt_id}`

**Tác dụng**: Xóa task. Chỉ có thể xóa task thuộc về work experience của user hiện tại.

**Authentication**: Required

**Cách sử dụng**: Gửi DELETE request với mt_id trong URL path.

**Response** (200 OK):

```json
{
  "message": "Task deleted successfully"
}
```

**Error Codes**:

- `404`: Task không tồn tại
- `401`: Không có quyền

---

## 10. Public API

Tất cả Public API **KHÔNG CẦN** authentication. Chỉ hiển thị thông tin của user có portfolio ở chế độ public (is_public = true).

### 10.1. Get Public Profile

**Endpoint**: `GET /api/v1/public/profile/{username}`

**Tác dụng**: Lấy thông tin profile công khai của user theo username. Chỉ hiển thị nếu portfolio ở chế độ public.

**Authentication**: Không cần

**Cách sử dụng**: Gửi GET request với username trong URL path. Không cần authentication.

**Response** (200 OK):

```json
{
  "username": "string",
  "fullname": "string",
  "avatar": "string",
  "email": "string", // Chỉ hiển thị nếu user cho phép
  "phone_number": "string",
  "address": "string",
  "university_name": "string"
}
```

**Error Codes**:

- `USER_NOT_FOUND` (404): User không tồn tại
- `PORTFOLIO_PRIVATE` (403): Portfolio đang ở chế độ private

---

### 10.2. Get Public Work Experience

**Endpoint**: `GET /api/v1/public/profile/{username}/workexp?page_index=1&page_size=10`

**Tác dụng**: Lấy danh sách work experiences công khai của user với phân trang. Chỉ hiển thị nếu portfolio ở chế độ public.

**Authentication**: Không cần

**Cách sử dụng**: Gửi GET request với username trong URL path và query parameters để phân trang.

**Query Parameters**:

- `page_index` (int, default: 1): Số trang, bắt đầu từ 1
- `page_size` (int, default: 10): Số items mỗi trang

**Response** (200 OK):

```json
{
  "data": [
    {
      "company_name": "string",
      "position": "string",
      "duration": "string",
      "description": "string",
      "tasks": [
        {
          "mt_id": "string",
          "task_description": "string"
        }
      ]
    }
  ],
  "total": 10,
  "page_index": 1,
  "page_size": 10
}
```

**Error Codes**:

- `USER_NOT_FOUND` (404): User không tồn tại
- `PORTFOLIO_PRIVATE` (403): Portfolio đang ở chế độ private

---

### 10.3. Get Public Projects

**Endpoint**: `GET /api/v1/public/profile/{username}/projects?page_index=1&page_size=10`

**Tác dụng**: Lấy danh sách projects công khai của user với phân trang. Chỉ hiển thị nếu portfolio ở chế độ public.

**Authentication**: Không cần

**Cách sử dụng**: Gửi GET request với username trong URL path và query parameters để phân trang.

**Query Parameters**:

- `page_index` (int, default: 1): Số trang, bắt đầu từ 1
- `page_size` (int, default: 10): Số items mỗi trang

**Response** (200 OK):

```json
{
  "data": [
    {
      "project_name": "string",
      "description": "string",
      "img_url": "string",
      "tech": [
        {
          "tech_name": "string"
        }
      ],
      "url_project": "string",
      "url_demo": "string",
      "url_github": "string"
    }
  ],
  "total": 10,
  "page_index": 1,
  "page_size": 10
}
```

**Error Codes**:

- `USER_NOT_FOUND` (404): User không tồn tại
- `PORTFOLIO_PRIVATE` (403): Portfolio đang ở chế độ private

---

### 10.4. Get Public Skills

**Endpoint**: `GET /api/v1/public/profile/{username}/skills`

**Tác dụng**: Lấy danh sách skills công khai của user. Chỉ hiển thị nếu portfolio ở chế độ public.

**Authentication**: Không cần

**Cách sử dụng**: Gửi GET request với username trong URL path.

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

**Error Codes**:

- `USER_NOT_FOUND` (404): User không tồn tại
- `PORTFOLIO_PRIVATE` (403): Portfolio đang ở chế độ private

---

### 10.5. Get Public Certificates

**Endpoint**: `GET /api/v1/public/profile/{username}/certificates`

**Tác dụng**: Lấy danh sách certificates công khai của user. Chỉ hiển thị nếu portfolio ở chế độ public.

**Authentication**: Không cần

**Cách sử dụng**: Gửi GET request với username trong URL path.

**Response** (200 OK):

```json
[
  {
    "certificate_name": "string"
  },
  ...
]
```

**Error Codes**:

- `USER_NOT_FOUND` (404): User không tồn tại
- `PORTFOLIO_PRIVATE` (403): Portfolio đang ở chế độ private

---

### 10.6. Check Username Availability

**Endpoint**: `GET /api/v1/public/check-username/{username}`

**Tác dụng**: Kiểm tra username có sẵn để sử dụng hay không. Sử dụng để validate username trước khi đăng ký.

**Authentication**: Không cần

**Cách sử dụng**: Gửi GET request với username trong URL path.

**Response** (200 OK - Available):

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

## 11. Send Mail

### 11.1. Send Contact Email

**Endpoint**: `POST /api/v1/email`

**Tác dụng**: Gửi email liên hệ từ HR/recruiter đến user. Email sẽ được gửi với thông tin user tự động lấy từ token.

**Authentication**: Required

**Cách sử dụng**: Gửi POST request với thông tin HR. user_email và user_name sẽ được tự động lấy từ user đang đăng nhập.

**Request Body**:

```json
{
  "hrName": "string", // Required
  "hrEmail": "string", // Required
  "hrCompany": "string", // Required
  "hrNotes": "string", // Required
  "hrPhone": "string" // Optional
}
```

**Response** (200 OK):

```json
{
  "message": "Email sent successfully"
}
```

**Error Codes**:

- `400`: Request không hợp lệ
- `500`: Lỗi server khi gửi email

**Lưu ý**: `user_email` và `user_name` sẽ được tự động lấy từ user đang đăng nhập, không cần gửi trong request.

---

## 12. Error Responses

### Standard Error Format

Tất cả các lỗi đều trả về format chuẩn:

```json
{
  "error": "ERROR_CODE",
  "message": "Error message description"
}
```

### Common Error Codes

| Code                    | Status | Mô tả                             |
| ----------------------- | ------ | --------------------------------- |
| `INVALID_REQUEST`       | 400    | Request không hợp lệ              |
| `INVALID_USERNAME`      | 400    | Username không đúng format        |
| `USERNAME_TAKEN`        | 409    | Username đã tồn tại               |
| `EMAIL_TAKEN`           | 409    | Email đã tồn tại                  |
| `USER_NOT_FOUND`        | 404    | User không tồn tại                |
| `PORTFOLIO_PRIVATE`     | 403    | Portfolio đang private            |
| `USERNAME_CHANGE_LIMIT` | 400    | Đã đổi username quá 3 lần/năm     |
| `UNAUTHORIZED`          | 401    | Chưa đăng nhập hoặc token hết hạn |
| `FORBIDDEN`             | 403    | Không có quyền truy cập           |

---

## 13. Notes

### Authentication

- Tất cả API (trừ Public API và Account endpoints) yêu cầu Bearer Token trong header Authorization
- Token có thể lấy từ Login hoặc Register API
- Token hết hạn thì dùng Refresh Token API để lấy token mới
- Format: `Authorization: Bearer YOUR_ACCESS_TOKEN`

### File Upload

- Chỉ hỗ trợ image files cho avatar và project images
- Images sẽ được upload lên Cloudinary
- Images sẽ được tự động compress và resize tối đa 1024x1024px với quality 75%
- Khi upload ảnh mới, ảnh cũ trên Cloudinary sẽ tự động bị xóa để tiết kiệm dung lượng

### Pagination

- Các API có pagination: Projects, Work Experience
- `page_index`: Bắt đầu từ 1
- `page_size`: Số items mỗi trang (default: 10)

### Username Rules

- Độ dài: 3-30 ký tự
- Chỉ cho phép: a-z, 0-9, -, \_
- Case-insensitive
- Chỉ được đổi tối đa 3 lần/năm
- Phải cách lần đổi trước ít nhất 30 ngày

### Date Format

- Tất cả dates dùng format: `YYYY-MM-DD`
- Timezone: UTC

### Data Ownership

- Tất cả dữ liệu (Skills, Projects, Work Experience, Certificates, Tasks) đều được lưu theo user_id
- User chỉ có thể truy cập và thao tác dữ liệu của chính mình
- Khi gọi API GET by ID, hệ thống sẽ tự động kiểm tra quyền sở hữu

### Image Management

- Khi update project với ảnh mới: ảnh cũ trên Cloudinary sẽ tự động bị xóa
- Khi update profile với avatar mới: avatar cũ trên Cloudinary sẽ tự động bị xóa
- Khi delete project: ảnh trên Cloudinary sẽ tự động bị xóa

---

## 14. Examples

### Example 1: Register và Login

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

### Example 2: Create Skill

```bash
curl -X POST https://portfolio-be-k3b0.onrender.com/api/v1/skill \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "skill_name": "React",
    "position": "Frontend Developer"
  }'
```

### Example 3: Get Skill by ID (để chỉnh sửa)

```bash
curl -X GET https://portfolio-be-k3b0.onrender.com/api/v1/skill/{skill_id} \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Example 4: Update Project với ảnh mới

```bash
curl -X PATCH https://portfolio-be-k3b0.onrender.com/api/v1/project/{project_id} \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -F "project_name=My Updated Project" \
  -F "description=Updated description" \
  -F "img_url=@/path/to/new-image.jpg" \
  -F 'tech=[{"tech_name":"React"},{"tech_name":"TypeScript"}]'
```

### Example 5: Get Project by ID (để chỉnh sửa)

```bash
curl -X GET https://portfolio-be-k3b0.onrender.com/api/v1/project/{project_id} \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Example 6: Get All Certificates

```bash
curl -X GET https://portfolio-be-k3b0.onrender.com/api/v1/certificate \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Example 7: Get Public Profile

```bash
curl -X GET https://portfolio-be-k3b0.onrender.com/api/v1/public/profile/johndoe
```

---

**Last Updated**: 2024-12-19  
**API Version**: v1
