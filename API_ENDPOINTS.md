# TÀI LIỆU API ENDPOINTS

##  Tổng quan

API Base URL: `/api/v{version}` (mặc định version = 1)

Tất cả các endpoints (trừ authentication) đều yêu cầu **JWT Bearer Token** trong header:

```
Authorization: Bearer {your_access_token}
```

---

##  AUTHENTICATION ENDPOINTS

### 1. Đăng ký tài khoản

**Endpoint**: `POST /api/v1/account/dev-register`

**Mô tả**: Tạo tài khoản mới với username

**Authentication**: Không cần

**Request Body**:

```json
{
  "username": "string (required, 3-30 chars, a-z, 0-9, -, _)",
  "email": "string (optional)",
  "password": "string (required, min 6 chars)",
  "fullName": "string (optional)"
}
```

**Ví dụ Request**:

```json
{
  "username": "newuser",
  "email": "newuser@example.com",
  "password": "SecurePassword123!",
  "fullName": "New User"
}
```

**Response 201 Created**:

```json
{
  "user_id": "d726c4b1-5a4e-4b89-84af-92c36d3e28aa",
  "username": "newuser",
  "email": "newuser@example.com",
  "token": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "abc123def456...",
    "expiresIn": 31536000,
    "createdAt": "2024-01-01T00:00:00Z"
  },
  "user_name": "newuser"
}
```

**Response 400 Bad Request** (Invalid username):

```json
{
  "error": "INVALID_USERNAME",
  "message": "Username must be 3-30 characters, only a-z, 0-9, -, _ allowed"
}
```

**Response 409 Conflict** (Username taken):

```json
{
  "error": "USERNAME_TAKEN",
  "message": "Username already taken"
}
```

**Response 409 Conflict** (Email taken):

```json
{
  "error": "EMAIL_TAKEN",
  "message": "Email already exists"
}
```

**cURL Example**:

```bash
curl -X POST "http://localhost:5005/api/v1/account/dev-register" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "newuser",
    "email": "newuser@example.com",
    "password": "SecurePassword123!",
    "fullName": "New User"
  }'
```

**Lưu ý**:
- Username sẽ được normalize (lowercase) và validate
- Username phải unique
- Portfolio mặc định là public (`is_public: true`)

---

### 2. Đăng nhập

**Endpoint**: `POST /api/v1/account/log-in`

**Mô tả**: Đăng nhập và nhận JWT tokens

**Authentication**: Không cần

**Request Body**:

```json
{
  "username": "string (required)",
  "password": "string (required)"
}
```

**Ví dụ Request**:

```json
{
  "username": "trungthanh",
  "password": "yourpassword"
}
```

**Response 200 OK**:

```json
{
  "token": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "abc123def456...",
    "expiresIn": 31536000,
    "createdAt": "2024-01-01T00:00:00Z"
  },
  "user_name": "trungthanh",
  "user_id": "d726c4b1-5a4e-4b89-84af-92c36d3e28aa",
  "email": "buithanh10112000@gmail.com"
}
```

**Response 401 Unauthorized**:

```json
{
  "message": "Login failed: Invalid password"
}
```

**cURL Example**:

```bash
curl -X POST "http://localhost:5005/api/v1/account/log-in" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "trungthanh",
    "password": "yourpassword"
  }'
```

**Lưu ý**:

- Có thể đăng nhập bằng `username` hoặc `email`
- Access token có thời hạn (mặc định: 525600 phút = 1 năm)
- Lưu `refreshToken` để refresh access token khi hết hạn

---

### 3. Refresh Token

**Endpoint**: `POST /api/v1/token/refresh`

**Mô tả**: Lấy access token mới bằng refresh token

**Authentication**: Không cần

**Request Body**:

```json
{
  "refreshToken": "string (required)"
}
```

**Ví dụ Request**:

```json
{
  "refreshToken": "abc123def456..."
}
```

**Response 200 OK**:

```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "new_refresh_token...",
  "expiresIn": 31536000,
  "createdAt": "2024-01-01T00:00:00Z"
}
```

**cURL Example**:

```bash
curl -X POST "http://localhost:5005/api/v1/token/refresh" \
  -H "Content-Type: application/json" \
  -d '{
    "refreshToken": "abc123def456..."
  }'
```

---

### 4. Lấy Claims từ Token

**Endpoint**: `GET /api/v1/token/claims`

**Mô tả**: Lấy thông tin claims từ JWT token hiện tại

**Authentication**: **Cần** (Bearer Token)

**Response 200 OK**:

```json
{
  "sub": "d726c4b1-5a4e-4b89-84af-92c36d3e28aa",
  "name": "trungthanh",
  "email": "buithanh10112000@gmail.com",
  "jti": "token-id",
  "exp": "1234567890",
  "iat": "1234567890"
}
```

**cURL Example**:

```bash
curl -X GET "http://localhost:5005/api/v1/token/claims" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

---

## 👤 USER/PROFILE ENDPOINTS

### 5. Lấy thông tin profile

**Endpoint**: `GET /api/v1/profile/`

**Mô tả**: Lấy thông tin profile của user hiện tại

**Authentication**: **Cần** (Bearer Token)

**Response 200 OK**:

```json
{
  "id": "d726c4b1-5a4e-4b89-84af-92c36d3e28aa",
  "userName": "trungthanh",
  "email": "buithanh10112000@gmail.com",
  "fullname": "B",
  "university_name": "Trường Đại học Công nghệ TP.HCM - HUTECH",
  "address": "Thành phố Hồ Chí Minh, Vietnam",
  "phone_number": "0000",
  "github": "https://github.com/trungdev",
  "dob": "10/11/2003",
  "avatar": "https://resources-1.hcm.gdatas.vn/avatar.jpg",
  "position_career": "Full Stack Developer",
  "expOfYear": "3 years",
  "background": "Experienced developer...",
  "mindset": "Always learning...",
  "linkedin_url": "https://linkedin.com/in/...",
  "facebook_url": "https://facebook.com/...",
  "gpa": 3.5
}
```

**cURL Example**:

```bash
curl -X GET "http://localhost:5005/api/v1/profile/" \
  -H "Authorization: Bearer {your_token}"
```

---

### 6. Cập nhật profile

**Endpoint**: `PATCH /api/v1/profile/{user_id}`

**Mô tả**: Cập nhật thông tin profile

**Authentication**: **Cần** (Bearer Token)

**Request**: `multipart/form-data`

**Form Data**:

```
fullname: string (optional)
email: string (optional)
university_name: string (optional)
address: string (optional)
phone_number: string (optional)
github: string (optional)
dob: string (optional)
avatar: file (optional, image file)
position_career: string (optional)
expOfYear: string (optional)
background: string (optional)
mindset: string (optional)
linkedin_url: string (optional)
facebook_url: string (optional)
GPA: double (optional)
```

**Response 200 OK**:

```json
{
  "id": "d726c4b1-5a4e-4b89-84af-92c36d3e28aa",
  "fullname": "Updated Name",
  "email": "updated@example.com",
  ...
}
```

**cURL Example**:

```bash
curl -X PATCH "http://localhost:5005/api/v1/profile/d726c4b1-5a4e-4b89-84af-92c36d3e28aa" \
  -H "Authorization: Bearer {your_token}" \
  -F "fullname=Updated Name" \
  -F "email=updated@example.com" \
  -F "avatar=@/path/to/image.jpg"
```

**Lưu ý**:

- Chỉ user sở hữu profile mới có thể cập nhật
- Avatar sẽ được upload lên cloud storage và trả về URL

---

## PROJECT ENDPOINTS

### 7. Lấy danh sách projects

**Endpoint**: `GET /api/v1/project/?page_index={page}&page_size={size}`

**Mô tả**: Lấy danh sách projects có phân trang

**Authentication**: **Cần** (Bearer Token)

**Query Parameters**:

- `page_index` (int, required): Số trang (bắt đầu từ 1)
- `page_size` (int, required): Số items mỗi trang

**Response 200 OK**:

```json
{
  "data": [
    {
      "id": "project-id-1",
      "project_name": "E-Commerce Website",
      "description": "Full-stack e-commerce platform",
      "project_type": "Web Application",
      "is_Reality": true,
      "url_project": "https://example.com",
      "url_demo": "https://demo.example.com",
      "url_github": "https://github.com/...",
      "duration": "6 months",
      "from": "2024-01-01",
      "to": "2024-06-30",
      "img_url": "https://resources-1.hcm.gdatas3.vn/project.jpg",
      "url_contract": "https://...",
      "url_excel": "https://...",
      "teches": [
        {
          "id": "tech-id-1",
          "tech_name": "React",
          "project_id": "project-id-1"
        },
        {
          "id": "tech-id-2",
          "tech_name": "Node.js",
          "project_id": "project-id-1"
        }
      ]
    }
  ],
  "pageIndex": 1,
  "pageSize": 10,
  "totalCount": 25,
  "totalPages": 3
}
```

**cURL Example**:

```bash
curl -X GET "http://localhost:5005/api/v1/project/?page_index=1&page_size=10" \
  -H "Authorization: Bearer {your_token}"
```

---

### 8. Tạo project mới

**Endpoint**: `POST /api/v1/project/create-project`

**Mô tả**: Tạo project mới

**Authentication**: **Cần** (Bearer Token)

**Request**: `multipart/form-data`

**Form Data**:

```
project_name: string (required)
description: string (optional)
tech: array of objects (optional) - [{"tech_name": "React"}, {"tech_name": "Node.js"}]
project_type: string (optional)
is_Reality: boolean (optional, default: false)
url_project: string (optional)
url_demo: string (optional)
url_github: string (optional)
duration: string (optional)
from: string (optional)
to: string (optional)
img_url: file (optional, image file)
url_contract: string (optional)
url_excel: string (optional)
```

**Response 200 OK**:

```json
{
  "id": "new-project-id",
  "project_name": "New Project",
  "description": "Project description",
  ...
}
```

**cURL Example**:

```bash
curl -X POST "http://localhost:5005/api/v1/project/create-project" \
  -H "Authorization: Bearer {your_token}" \
  -F "project_name=New Project" \
  -F "description=Project description" \
  -F "is_Reality=true" \
  -F "tech=[{\"tech_name\":\"React\"},{\"tech_name\":\"Node.js\"}]" \
  -F "img_url=@/path/to/image.jpg"
```

---

### 9. Cập nhật project

**Endpoint**: `PATCH /api/v1/project/{project_id}`

**Mô tả**: Cập nhật thông tin project

**Authentication**: **Cần** (Bearer Token)

**Path Parameters**:

- `project_id` (string, required): ID của project

**Request**: `multipart/form-data` (tất cả fields đều optional)

**Form Data**:

```
project_name: string (optional)
description: string (optional)
tech: JSON string (optional) - Array of tech objects: [{"tech_name": "React"}, {"tech_name": "Node.js"}]
project_type: string (optional)
is_Reality: boolean (optional)
url_project: string (optional)
url_demo: string (optional)
url_github: string (optional)
duration: string (optional)
from: string (optional)
to: string (optional)
img_url: file (optional, image file)
url_contract: string (optional)
url_excel: string (optional)
```

**Lưu ý về trường `tech`**:

- Trường `tech` là một mảng các object `TechDto`
- **Cách 1 (Khuyến nghị)**: Gửi theo format form array:
  ```
  tech[0].tech_name: React
  tech[1].tech_name: Node.js
  tech[2].tech_name: PostgreSQL
  ```
- **Cách 2**: Gửi như JSON string (có thể cần custom model binder):
  ```
  tech: [{"tech_name":"React"},{"tech_name":"Node.js"}]
  ```
- Nếu không gửi trường này, các tech hiện tại sẽ không bị thay đổi

**Response 200 OK**:

```json
{
  "id": "project-id",
  "project_name": "Updated Project Name",
  "description": "Updated description",
  "project_type": "Web Application",
  "is_Reality": true,
  "url_project": "https://example.com",
  "url_demo": "https://demo.example.com",
  "url_github": "https://github.com/...",
  "duration": "6 months",
  "from": "2024-01-01",
  "to": "2024-06-30",
  "img_url": "https://resources-1.hcm.gdatas3.vn/project.jpg",
  "url_contract": "https://...",
  "url_excel": "https://...",
  "teches": [
    {
      "id": "tech-id-1",
      "tech_name": "React",
      "project_id": "project-id"
    }
  ]
}
```

**cURL Example** (cập nhật thông tin cơ bản):

```bash
curl -X PATCH "http://localhost:5005/api/v1/project/project-id" \
  -H "Authorization: Bearer {your_token}" \
  -F "project_name=Updated Name" \
  -F "description=Updated description"
```

**cURL Example** (cập nhật với tech array - Cách 1: Form array - Khuyến nghị):

```bash
curl -X PATCH "http://localhost:5005/api/v1/project/project-id" \
  -H "Authorization: Bearer {your_token}" \
  -F "project_name=Updated Name" \
  -F "description=Updated description" \
  -F "tech[0].tech_name=React" \
  -F "tech[1].tech_name=Node.js" \
  -F "tech[2].tech_name=PostgreSQL" \
  -F "is_Reality=true"
```

**cURL Example** (cập nhật với tech array - Cách 2: JSON string):

```bash
curl -X PATCH "http://localhost:5005/api/v1/project/project-id" \
  -H "Authorization: Bearer {your_token}" \
  -F "project_name=Updated Name" \
  -F "description=Updated description" \
  -F 'tech=[{"tech_name":"React"},{"tech_name":"Node.js"},{"tech_name":"PostgreSQL"}]' \
  -F "is_Reality=true"
```

**Lưu ý**: Cách 1 (form array) là cách chuẩn của ASP.NET Core. Cách 2 (JSON string) có thể không hoạt động nếu không có custom model binder.

**cURL Example** (cập nhật với file ảnh):

```bash
curl -X PATCH "http://localhost:5005/api/v1/project/project-id" \
  -H "Authorization: Bearer {your_token}" \
  -F "project_name=Updated Name" \
  -F "img_url=@/path/to/new-image.jpg"
```

**JavaScript/Fetch Example** (Cách 1: Form array - Khuyến nghị):

```javascript
const formData = new FormData();
formData.append("project_name", "Updated Project Name");
formData.append("description", "Updated description");

// Gửi tech theo format form array
const techs = [
  { tech_name: "React" },
  { tech_name: "Node.js" },
  { tech_name: "PostgreSQL" },
];
techs.forEach((tech, index) => {
  formData.append(`tech[${index}].tech_name`, tech.tech_name);
});

formData.append("is_Reality", "true");

// Nếu có file ảnh
const fileInput = document.querySelector('input[type="file"]');
if (fileInput.files[0]) {
  formData.append("img_url", fileInput.files[0]);
}

fetch("http://localhost:5005/api/v1/project/project-id", {
  method: "PATCH",
  headers: {
    Authorization: "Bearer " + yourToken,
  },
  body: formData,
});
```

**JavaScript/Fetch Example** (Cách 2: JSON string):

```javascript
const formData = new FormData();
formData.append("project_name", "Updated Project Name");
formData.append("description", "Updated description");
formData.append(
  "tech",
  JSON.stringify([{ tech_name: "React" }, { tech_name: "Node.js" }])
);
formData.append("is_Reality", "true");

// Nếu có file ảnh
const fileInput = document.querySelector('input[type="file"]');
if (fileInput.files[0]) {
  formData.append("img_url", fileInput.files[0]);
}

fetch("http://localhost:5005/api/v1/project/project-id", {
  method: "PATCH",
  headers: {
    Authorization: "Bearer " + yourToken,
  },
  body: formData,
});
```

**Postman Example** (Cách 1: Form array - Khuyến nghị):

1. Chọn method: `PATCH`
2. URL: `http://localhost:5005/api/v1/project/{project_id}`
3. Headers: `Authorization: Bearer {your_token}`
4. Body: chọn `form-data`
5. Thêm các key-value pairs:
   - `project_name`: `Updated Name` (Text)
   - `description`: `Updated description` (Text)
   - `tech[0].tech_name`: `React` (Text)
   - `tech[1].tech_name`: `Node.js` (Text)
   - `tech[2].tech_name`: `PostgreSQL` (Text)
   - `img_url`: chọn file (File)
   - `is_Reality`: `true` (Text)

**Postman Example** (Cách 2: JSON string):

1. Chọn method: `PATCH`
2. URL: `http://localhost:5005/api/v1/project/{project_id}`
3. Headers: `Authorization: Bearer {your_token}`
4. Body: chọn `form-data`
5. Thêm các key-value pairs:
   - `project_name`: `Updated Name` (Text)
   - `description`: `Updated description` (Text)
   - `tech`: `[{"tech_name":"React"},{"tech_name":"Node.js"}]` (Text)
   - `img_url`: chọn file (File)
   - `is_Reality`: `true` (Text)

---

### 10. Xóa project

**Endpoint**: `DELETE /api/v1/project/{project_id}`

**Mô tả**: Xóa project

**Authentication**: **Cần** (Bearer Token)

**Path Parameters**:

- `project_id` (string, required): ID của project

**Response 200 OK**:

```json
{
  "message": "Project deleted successfully"
}
```

**Response 404 Not Found**:

```json
{
  "message": "Project not found"
}
```

**cURL Example**:

```bash
curl -X DELETE "http://localhost:5005/api/v1/project/project-id" \
  -H "Authorization: Bearer {your_token}"
```

---

## 🛠️ SKILL ENDPOINTS

### 11. Thêm skill cho user

**Endpoint**: `POST /api/v1/skill/add-skill`

**Mô tả**: Thêm skill vào profile của user hiện tại

**Authentication**: **Cần** (Bearer Token)

**Request Body**:

```json
{
  "skill_name": "string (required)",
  "position": "string (optional)"
}
```

**Ví dụ Request**:

```json
{
  "skill_name": "React",
  "position": "Frontend"
}
```

**Response 200 OK**:

```json
{
  "id": "skill-id",
  "skill_name": "React",
  "position": "Frontend"
}
```

**cURL Example**:

```bash
curl -X POST "http://localhost:5005/api/v1/skill/add-skill" \
  -H "Authorization: Bearer {your_token}" \
  -H "Content-Type: application/json" \
  -d '{
    "skill_name": "React",
    "position": "Frontend"
  }'
```

---

### 12. Lấy tất cả skills

**Endpoint**: `GET /api/v1/skill/`

**Mô tả**: Lấy danh sách tất cả skills của user hiện tại

**Authentication**: **Cần** (Bearer Token)

**Response 200 OK**:

```json
[
  {
    "id": "skill-id-1",
    "skill_name": "React",
    "position": "Frontend"
  },
  {
    "id": "skill-id-2",
    "skill_name": "Node.js",
    "position": "Backend"
  }
]
```

**cURL Example**:

```bash
curl -X GET "http://localhost:5005/api/v1/skill/" \
  -H "Authorization: Bearer {your_token}"
```

---

### 13. Cập nhật skill

**Endpoint**: `PATCH /api/v1/skill/{skill_id}`

**Mô tả**: Cập nhật thông tin skill

**Authentication**: **Cần** (Bearer Token)

**Path Parameters**:

- `skill_id` (string, required): ID của skill

**Request Body**:

```json
{
  "skill_name": "string (optional)",
  "position": "string (optional)"
}
```

**Response 200 OK**:

```json
{
  "id": "skill-id",
  "skill_name": "Updated Skill",
  "position": "Updated Position"
}
```

**cURL Example**:

```bash
curl -X PATCH "http://localhost:5005/api/v1/skill/skill-id" \
  -H "Authorization: Bearer {your_token}" \
  -H "Content-Type: application/json" \
  -d '{
    "skill_name": "Updated Skill",
    "position": "Updated Position"
  }'
```

---

### 14. Xóa tất cả skills của user

**Endpoint**: `DELETE /api/v1/skill/`

**Mô tả**: Xóa tất cả skills của user hiện tại

**Authentication**: **Cần** (Bearer Token)

**Response 200 OK**:

```json
{
  "message": "All skills deleted successfully"
}
```

**cURL Example**:

```bash
curl -X DELETE "http://localhost:5005/api/v1/skill/" \
  -H "Authorization: Bearer {your_token}"
```

---

### 15. Xóa skill cụ thể

**Endpoint**: `DELETE /api/v1/skill/remove/{skill_id}`

**Mô tả**: Xóa một skill cụ thể khỏi profile

**Authentication**: **Cần** (Bearer Token)

**Path Parameters**:

- `skill_id` (string, required): ID của skill

**Response 200 OK**:

```json
{
  "message": "Skill deleted successfully"
}
```

**cURL Example**:

```bash
curl -X DELETE "http://localhost:5005/api/v1/skill/remove/skill-id" \
  -H "Authorization: Bearer {your_token}"
```

---

## 💼 WORK EXPERIENCE ENDPOINTS

### 16. Thêm work experience

**Endpoint**: `POST /api/v1/workexp/`

**Mô tả**: Thêm kinh nghiệm làm việc mới

**Authentication**: **Cần** (Bearer Token)

**Request Body**:

```json
{
  "company_name": "string (required)",
  "position": "string (optional)",
  "duration": "string (optional)",
  "description": "string (optional)",
  "project_id": "string (optional)"
}
```

**Ví dụ Request**:

```json
{
  "company_name": "ABC Company",
  "position": "Senior Developer",
  "duration": "2 years",
  "description": "Worked on various projects",
  "project_id": "project-id-1"
}
```

**Response 200 OK**:

```json
{
  "id": "we-id",
  "company_name": "ABC Company",
  "position": "Senior Developer",
  ...
}
```

**cURL Example**:

```bash
curl -X POST "http://localhost:5005/api/v1/workexp/" \
  -H "Authorization: Bearer {your_token}" \
  -H "Content-Type: application/json" \
  -d '{
    "company_name": "ABC Company",
    "position": "Senior Developer",
    "duration": "2 years"
  }'
```

---

### 17. Lấy danh sách work experiences

**Endpoint**: `GET /api/v1/workexp/?page_index={page}&page_size={size}`

**Mô tả**: Lấy danh sách work experiences có phân trang

**Authentication**: **Cần** (Bearer Token)

**Query Parameters**:

- `page_index` (int, required): Số trang
- `page_size` (int, required): Số items mỗi trang

**Response 200 OK**:

```json
{
  "data": [
    {
      "id": "we-id-1",
      "company_name": "ABC Company",
      "position": "Senior Developer",
      "duration": "2 years",
      "description": "Worked on...",
      "project_id": "project-id-1",
      "myTasks": [
        {
          "id": "task-id-1",
          "task_description": "Developed features"
        }
      ]
    }
  ],
  "pageIndex": 1,
  "pageSize": 10,
  "totalCount": 5,
  "totalPages": 1
}
```

**cURL Example**:

```bash
curl -X GET "http://localhost:5005/api/v1/workexp/?page_index=1&page_size=10" \
  -H "Authorization: Bearer {your_token}"
```

---

### 18. Cập nhật work experience

**Endpoint**: `PATCH /api/v1/workexp/{we_id}`

**Mô tả**: Cập nhật thông tin work experience

**Authentication**: **Cần** (Bearer Token)

**Path Parameters**:

- `we_id` (string, required): ID của work experience

**Request Body** (tất cả fields optional):

```json
{
  "company_name": "string (optional)",
  "position": "string (optional)",
  "duration": "string (optional)",
  "description": "string (optional)",
  "project_id": "string (optional)"
}
```

**Response 200 OK**:

```json
{
  "id": "we-id",
  "company_name": "Updated Company",
  ...
}
```

**cURL Example**:

```bash
curl -X PATCH "http://localhost:5005/api/v1/workexp/we-id" \
  -H "Authorization: Bearer {your_token}" \
  -H "Content-Type: application/json" \
  -d '{
    "company_name": "Updated Company",
    "position": "Updated Position"
  }'
```

---

### 19. Xóa work experience

**Endpoint**: `DELETE /api/v1/workexp/{we_id}`

**Mô tả**: Xóa work experience

**Authentication**: **Cần** (Bearer Token)

**Path Parameters**:

- `we_id` (string, required): ID của work experience

**Response 200 OK**:

```json
{
  "message": "Work experience deleted successfully"
}
```

**cURL Example**:

```bash
curl -X DELETE "http://localhost:5005/api/v1/workexp/we-id" \
  -H "Authorization: Bearer {your_token}"
```

---

## 📋 MY TASK ENDPOINTS

### 20. Thêm task vào work experience

**Endpoint**: `POST /api/v1/mytask/{we_id}`

**Mô tả**: Thêm task vào một work experience

**Authentication**: **Cần** (Bearer Token)

**Path Parameters**:

- `we_id` (string, required): ID của work experience

**Request Body**:

```json
{
  "task_description": "string (required)"
}
```

**Ví dụ Request**:

```json
{
  "task_description": "Developed RESTful APIs"
}
```

**Response 200 OK**:

```json
{
  "id": "task-id",
  "we_id": "we-id",
  "task_description": "Developed RESTful APIs"
}
```

**cURL Example**:

```bash
curl -X POST "http://localhost:5005/api/v1/mytask/we-id" \
  -H "Authorization: Bearer {your_token}" \
  -H "Content-Type: application/json" \
  -d '{
    "task_description": "Developed RESTful APIs"
  }'
```

---

### 21. Cập nhật task

**Endpoint**: `PATCH /api/v1/mytask/update/{we_id}`

**Mô tả**: Cập nhật task trong work experience

**Authentication**: **Cần** (Bearer Token)

**Path Parameters**:

- `we_id` (string, required): ID của work experience

**Request Body**:

```json
{
  "task_description": "string (required)"
}
```

**Response 200 OK**:

```json
{
  "id": "task-id",
  "task_description": "Updated task description"
}
```

**cURL Example**:

```bash
curl -X PATCH "http://localhost:5005/api/v1/mytask/update/we-id" \
  -H "Authorization: Bearer {your_token}" \
  -H "Content-Type: application/json" \
  -d '{
    "task_description": "Updated task description"
  }'
```

---

### 22. Xóa task

**Endpoint**: `DELETE /api/v1/mytask/{mt_id}`

**Mô tả**: Xóa một task

**Authentication**: **Cần** (Bearer Token)

**Path Parameters**:

- `mt_id` (string, required): ID của task

**Response 200 OK**:

```json
{
  "message": "Task deleted successfully"
}
```

**cURL Example**:

```bash
curl -X DELETE "http://localhost:5005/api/v1/mytask/task-id" \
  -H "Authorization: Bearer {your_token}"
```

---

## 🏆 CERTIFICATE ENDPOINTS

### 23. Thêm certificate

**Endpoint**: `POST /api/v1/certificate/`

**Mô tả**: Thêm certificate mới

**Authentication**: **Cần** (Bearer Token)

**Request Body**:

```json
{
  "certificate_name": "string (required)"
}
```

**Ví dụ Request**:

```json
{
  "certificate_name": "AWS Certified Solutions Architect"
}
```

**Response 200 OK**:

```json
{
  "id": "cert-id",
  "certificate_name": "AWS Certified Solutions Architect",
  "user_id": "user-id"
}
```

**cURL Example**:

```bash
curl -X POST "http://localhost:5005/api/v1/certificate/" \
  -H "Authorization: Bearer {your_token}" \
  -H "Content-Type: application/json" \
  -d '{
    "certificate_name": "AWS Certified Solutions Architect"
  }'
```

---

### 24. Cập nhật certificate

**Endpoint**: `PATCH /api/v1/certificate/{certificate_id}`

**Mô tả**: Cập nhật thông tin certificate

**Authentication**: **Cần** (Bearer Token)

**Path Parameters**:

- `certificate_id` (string, required): ID của certificate

**Request Body**:

```json
{
  "certificate_name": "string (required)"
}
```

**Response 200 OK**:

```json
{
  "id": "cert-id",
  "certificate_name": "Updated Certificate Name"
}
```

**cURL Example**:

```bash
curl -X PATCH "http://localhost:5005/api/v1/certificate/cert-id" \
  -H "Authorization: Bearer {your_token}" \
  -H "Content-Type: application/json" \
  -d '{
    "certificate_name": "Updated Certificate Name"
  }'
```

---

### 25. Xóa certificate

**Endpoint**: `DELETE /api/v1/certificate/{certificate_id}`

**Mô tả**: Xóa certificate

**Authentication**: **Cần** (Bearer Token)

**Path Parameters**:

- `certificate_id` (string, required): ID của certificate

**Response 200 OK**:

```json
{
  "message": "Certificate deleted successfully"
}
```

**Response 401 Unauthorized**:

```json
{
  "message": "You have no permission"
}
```

**cURL Example**:

```bash
curl -X DELETE "http://localhost:5005/api/v1/certificate/cert-id" \
  -H "Authorization: Bearer {your_token}"
```

---

## 📧 EMAIL ENDPOINT

### 26. Gửi email

**Endpoint**: `POST /api/v1/email/`

**Mô tả**: Gửi email liên hệ (HR contact)

**Authentication**: **Cần** (Bearer Token)

**Request Body**:

```json
{
  "name": "string (required)",
  "email": "string (required)",
  "message": "string (required)",
  "subject": "string (optional)"
}
```

**Ví dụ Request**:

```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "message": "Interested in your profile",
  "subject": "Job Opportunity"
}
```

**Response 200 OK**:

```json
{
  "message": "Email sent successfully"
}
```

**cURL Example**:

```bash
curl -X POST "http://localhost:5005/api/v1/email/" \
  -H "Authorization: Bearer {your_token}" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "John Doe",
    "email": "john@example.com",
    "message": "Interested in your profile"
  }'
```

---

## 🔍 HEALTH CHECK

### 27. Health Check

**Endpoint**: `GET /healthcheck`

**Mô tả**: Kiểm tra trạng thái server

**Authentication**: Không cần

**Response 200 OK**:

```
Server is alive!
```

**cURL Example**:

```bash
curl -X GET "http://localhost:5005/healthcheck"
```

---

## 🌐 PUBLIC API ENDPOINTS (Không cần authentication)

### 28. Lấy public profile theo username

**Endpoint**: `GET /api/v1/public/profile/{username}`

**Mô tả**: Lấy thông tin profile công khai của user theo username

**Authentication**: Không cần

**Path Parameters**:
- `username` (string, required): Username của user

**Response 200 OK**:

```json
{
  "username": "trunghuy",
  "fullname": "Trung Huy",
  "email": "trunghuy@example.com",
  "phone_number": "0123456789",
  "address": "Ho Chi Minh City, Vietnam",
  "position_career": "Software Developer",
  "background": "Experienced developer...",
  "mindset": "Always learning...",
  "avatar": "https://...",
  "github": "https://github.com/...",
  "linkedin_url": "https://linkedin.com/...",
  "facebook_url": "https://facebook.com/...",
  "university_name": "University Name",
  "gpa": 3.5,
  "expOfYear": "5 years",
  "dob": "01/01/1990",
  "certificates": [
    {
      "id": "cert_123",
      "certificate_name": "AWS Certified"
    }
  ]
}
```

**Response 404 Not Found**:

```json
{
  "error": "USER_NOT_FOUND",
  "message": "Portfolio not found"
}
```

**Response 403 Forbidden**:

```json
{
  "error": "PORTFOLIO_PRIVATE",
  "message": "This portfolio is private"
}
```

**cURL Example**:

```bash
curl -X GET "http://localhost:5005/api/v1/public/profile/trunghuy"
```

---

### 29. Lấy work experience public

**Endpoint**: `GET /api/v1/public/profile/{username}/workexp`

**Mô tả**: Lấy danh sách work experience công khai

**Authentication**: Không cần

**Path Parameters**:
- `username` (string, required): Username của user

**Query Parameters**:
- `page_index` (int, default: 1): Số trang
- `page_size` (int, default: 10): Số items mỗi trang

**Response 200 OK**:

```json
{
  "total_item": 5,
  "data": [
    {
      "we_id": "we_123",
      "company_name": "Company Name",
      "position": "Senior Developer",
      "duration": "2020-2023",
      "description": "Work description...",
      "project_id": "proj_123",
      "tasks": [
        {
          "mt_id": "task_123",
          "task_description": "Task description"
        }
      ]
    }
  ]
}
```

**cURL Example**:

```bash
curl -X GET "http://localhost:5005/api/v1/public/profile/trunghuy/workexp?page_index=1&page_size=10"
```

---

### 30. Lấy projects public

**Endpoint**: `GET /api/v1/public/profile/{username}/projects`

**Mô tả**: Lấy danh sách projects công khai

**Authentication**: Không cần

**Path Parameters**:
- `username` (string, required): Username của user

**Query Parameters**:
- `page_index` (int, default: 1): Số trang
- `page_size` (int, default: 10): Số items mỗi trang

**Response 200 OK**:

```json
{
  "total_item": 10,
  "data": [
    {
      "project_id": "proj_123",
      "project_name": "Project Name",
      "description": "Project description...",
      "project_type": "Web Application",
      "is_Reality": true,
      "duration": "6 months",
      "from": "2023-01",
      "to": "2023-06",
      "url_project": "https://...",
      "url_demo": "https://...",
      "url_github": "https://...",
      "img_url": "https://...",
      "url_contract": "https://...",
      "url_excel": "https://...",
      "teches": [
        {
          "tech_name": "React"
        }
      ]
    }
  ]
}
```

**cURL Example**:

```bash
curl -X GET "http://localhost:5005/api/v1/public/profile/trunghuy/projects?page_index=1&page_size=10"
```

---

### 31. Lấy skills public

**Endpoint**: `GET /api/v1/public/profile/{username}/skills`

**Mô tả**: Lấy danh sách skills công khai

**Authentication**: Không cần

**Path Parameters**:
- `username` (string, required): Username của user

**Response 200 OK**:

```json
[
  {
    "skill_id": "skill_123",
    "skill_name": "React",
    "position": "Frontend"
  },
  {
    "skill_id": "skill_124",
    "skill_name": "Node.js",
    "position": "Backend"
  }
]
```

**cURL Example**:

```bash
curl -X GET "http://localhost:5005/api/v1/public/profile/trunghuy/skills"
```

---

### 32. Lấy certificates public

**Endpoint**: `GET /api/v1/public/profile/{username}/certificates`

**Mô tả**: Lấy danh sách certificates công khai

**Authentication**: Không cần

**Path Parameters**:
- `username` (string, required): Username của user

**Response 200 OK**:

```json
[
  {
    "id": "cert_123",
    "certificate_name": "AWS Certified Solutions Architect"
  }
]
```

**cURL Example**:

```bash
curl -X GET "http://localhost:5005/api/v1/public/profile/trunghuy/certificates"
```

---

### 33. Check username availability

**Endpoint**: `GET /api/v1/public/check-username/{username}`

**Mô tả**: Kiểm tra username có sẵn hay không

**Authentication**: Không cần

**Path Parameters**:
- `username` (string, required): Username cần kiểm tra

**Response 200 OK** (available):

```json
{
  "available": true,
  "message": "Username is available"
}
```

**Response 200 OK** (taken):

```json
{
  "available": false,
  "message": "Username already taken"
}
```

**cURL Example**:

```bash
curl -X GET "http://localhost:5005/api/v1/public/check-username/newusername"
```

---

## 🔧 USER MANAGEMENT ENDPOINTS (Cần authentication)

### 34. Đổi username

**Endpoint**: `PATCH /api/v1/profile/username`

**Mô tả**: Đổi username của user hiện tại

**Authentication**: **Cần** (Bearer Token)

**Request Body**:

```json
{
  "username": "newusername"
}
```

**Response 200 OK**:

```json
{
  "message": "Username updated successfully to newusername",
  "username": "newusername"
}
```

**Response 400 Bad Request** (Invalid username):

```json
{
  "error": "INVALID_USERNAME",
  "message": "Username must be 3-30 characters, only a-z, 0-9, -, _ allowed"
}
```

**Response 400 Bad Request** (Username taken):

```json
{
  "error": "USERNAME_TAKEN",
  "message": "Username already taken"
}
```

**Response 400 Bad Request** (Change limit):

```json
{
  "error": "USERNAME_CHANGE_LIMIT",
  "message": "You can only change username 3 times per year"
}
```

**cURL Example**:

```bash
curl -X PATCH "http://localhost:5005/api/v1/profile/username" \
  -H "Authorization: Bearer {your_token}" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "newusername"
  }'
```

**Lưu ý**:
- Tối đa 3 lần đổi username/năm
- Phải cách lần đổi trước ít nhất 30 ngày
- Username phải tuân theo validation rules

---

### 35. Bật/tắt public portfolio

**Endpoint**: `PATCH /api/v1/profile/visibility`

**Mô tả**: Bật hoặc tắt tính công khai của portfolio

**Authentication**: **Cần** (Bearer Token)

**Request Body**:

```json
{
  "is_public": true
}
```

**Response 200 OK**:

```json
{
  "message": "Visibility updated successfully. Portfolio is now public",
  "is_public": true
}
```

**cURL Example**:

```bash
curl -X PATCH "http://localhost:5005/api/v1/profile/visibility" \
  -H "Authorization: Bearer {your_token}" \
  -H "Content-Type: application/json" \
  -d '{
    "is_public": true
  }'
```

---

## ⚠️ ERROR RESPONSES

### Common Error Codes

**400 Bad Request**: Request không hợp lệ

```json
{
  "error": "ERROR_CODE",
  "message": "Error message"
}
```

**401 Unauthorized**: Chưa đăng nhập hoặc token không hợp lệ

```json
{
  "error": "UNAUTHORIZED",
  "message": "Unauthorized"
}
```

**403 Forbidden**: Không có quyền truy cập

```json
{
  "error": "PORTFOLIO_PRIVATE",
  "message": "This portfolio is private"
}
```

**404 Not Found**: Resource không tồn tại

```json
{
  "error": "USER_NOT_FOUND",
  "message": "Portfolio not found"
}
```

**409 Conflict**: Resource đã tồn tại

```json
{
  "error": "USERNAME_TAKEN",
  "message": "Username already taken"
}
```

**429 Too Many Requests**: Vượt quá giới hạn

```json
{
  "error": "USERNAME_CHANGE_LIMIT",
  "message": "You can only change username 3 times per year"
}
```

**500 Internal Server Error**: Lỗi server

```json
{
  "error": "INTERNAL_SERVER_ERROR",
  "message": "Internal Server Error"
}
```

### Error Codes Reference

- `USER_NOT_FOUND` (404): Username không tồn tại
- `PORTFOLIO_PRIVATE` (403): Portfolio bị private
- `USERNAME_TAKEN` (409): Username đã được sử dụng
- `EMAIL_TAKEN` (409): Email đã được sử dụng
- `INVALID_USERNAME` (400): Username không hợp lệ
- `USERNAME_CHANGE_LIMIT` (429): Đã đổi username quá nhiều lần
- `UNAUTHORIZED` (401): Chưa đăng nhập
- `FORBIDDEN` (403): Không có quyền

---

## 📝 NOTES

1. **Authentication**: 
   - Hầu hết các endpoints đều yêu cầu JWT Bearer Token
   - Public API endpoints (`/api/v1/public/*`) không cần authentication
   - Authentication endpoints (`/api/v1/account/*`) không cần token

2. **Username Rules**:
   - Độ dài: 3-30 ký tự
   - Chỉ cho phép: a-z, 0-9, -, _
   - Không được bắt đầu hoặc kết thúc bằng - hoặc _
   - Case-insensitive (tự động lowercase)
   - Reserved words: admin, api, www, public, private, register, login, logout, profile, a-dmin

3. **File Upload**: Khi upload file (avatar, project image), sử dụng `multipart/form-data`

4. **Pagination**: Các endpoints có phân trang sử dụng `page_index` (bắt đầu từ 1) và `page_size`

5. **Date Format**: Sử dụng ISO 8601 format cho dates

6. **Base URL**: Thay `localhost:5005` bằng domain thực tế khi deploy

7. **Swagger UI**: Truy cập `/swagger` để xem và test API trực tiếp trên browser

8. **Public Portfolio**: 
   - Mặc định portfolio là public (`is_public: true`)
   - Có thể bật/tắt qua API `/api/v1/profile/visibility`
   - Chỉ portfolio public mới có thể truy cập qua Public API

9. **Username Change**:
   - Tối đa 3 lần/năm
   - Phải cách lần đổi trước ít nhất 30 ngày

---

##  Testing với Postman

1. Import collection vào Postman
2. Set base URL: `http://localhost:5005`
3. Đăng nhập để lấy token
4. Set token vào Authorization header cho các requests tiếp theo
5. Test các endpoints

---

##  Tham khảo thêm

- Swagger UI: `http://localhost:5005/swagger`
- Health Check: `http://localhost:5005/healthcheck`
- Database Structure: Xem file `CAU_TRUC_DB_VA_DU_AN.md`
