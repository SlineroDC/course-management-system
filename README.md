# Course Management System

A full-stack course management application built with .NET 8 (Backend) and Vue 3 (Frontend), featuring role-based authorization, Docker support, and comprehensive CRUD operations.

## 🚀 Features

- **Authentication**: Identity + JWT with role-based authorization (Admin/User)
- **Course Management**: Create, Read, Update, Delete (Soft Delete), Publish/Unpublish
- **Lesson Management**: Full CRUD with reordering capabilities
- **Metrics Dashboard**: Real-time statistics and analytics
- **Business Rules**:
  - Courses cannot be published without lessons
  - Soft Delete for courses and lessons
  - Unique lesson order validation per course
- **Architecture**: Clean Architecture (Domain, Application, Infrastructure, API)

## 🛠 Tech Stack

### Backend

- .NET 8
- Entity Framework Core
- PostgreSQL
- ASP.NET Core Identity
- JWT Authentication
- xUnit for testing

### Frontend

- Vue 3 with Composition API
- Vite
- Tailwind CSS
- Pinia (State Management)
- Axios
- Vue Router

## 📦 Quick Start with Docker

The easiest way to run the entire application:

```bash
docker-compose up --build -d
```

This will start:

- PostgreSQL database on port 5433 (host) → 5432 (container)
- Backend API on http://localhost:7260 → http://localhost:5070
- Frontend on http://localhost:5173

**Default Credentials:**

- Email: `admin@example.com`
- Password: `Password123!`
- Role: Admin

## 🔧 Manual Setup

### Prerequisites

- .NET 8 SDK
- Node.js 20+ & npm
- PostgreSQL 16+

### Backend Setup

1. **Database Configuration**:

   Update the connection string in `backend/API/appsettings.json`:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=CourseManagementDb;Username=postgres;Password=postgres"
   }
   ```

2. **Apply Migrations**:

   ```bash
   dotnet ef database update --project backend/Infrastructure --startup-project backend/API
   ```

3. **Run the API**:
   ```bash
   cd backend/API
   dotnet run
   ```
   - API: `http://localhost:5070`
   - Swagger: `http://localhost:5070/swagger/index.html`

### Frontend Setup

1. **Install Dependencies**:

   ```bash
   cd client
   npm install
   ```

2. **Run Development Server**:
   ```bash
   npm run dev
   ```
   - App: `http://localhost:5173`

## 📖 Usage Guide

### Dashboard

1. **Login** with default credentials
2. **Create Course**: Enter title and click "Create"
3. **Manage Lessons**: Click "Manage Lessons" on any course
4. **Add Lessons**: Specify title and order number
5. **Reorder**: Use 🔼/🔽 buttons to change lesson order
6. **Edit**: Click "Edit" on courses or lessons
7. **Publish**: Courses must have at least one lesson
8. **View Metrics**: Navigate to `/metrics` for dashboard

## 🐳 Docker Quick Commands

### Using the Management Script (Recomendado)

```bash
# Iniciar todo
./docker-manage.sh start

# Ver estado
./docker-manage.sh status

# Ver logs
./docker-manage.sh logs api

# Probar login
./docker-manage.sh test-login test@example.com Test@123456

# Limpiar todo
./docker-manage.sh clean
```

### Using docker-compose Directly

```bash
# Build and start
docker-compose up -d --build

# View logs
docker-compose logs -f api

# Stop services
docker-compose down

# Clean everything
docker-compose down -v
```

**Docker Ports:**
- Frontend: http://localhost:5173
- Backend API: http://localhost:7260
- Backend Swagger: http://localhost:7260/swagger/index.html
- Database: localhost:5433 (external), 5432 (internal)

---

## 📚 Complete Documentation

- **[ROLE_AUTHORIZATION_TESTS.md](./ROLE_AUTHORIZATION_TESTS.md)** - Comprehensive role authorization test suite (NEW)
- **[DOCKER_COMPLETE_GUIDE.md](./DOCKER_COMPLETE_GUIDE.md)** - Guía completa de Docker con troubleshooting
- **[EXECUTIVE_SUMMARY.md](./EXECUTIVE_SUMMARY.md)** - Resumen ejecutivo de mejoras
- **[QUICK_GUIDE.md](./QUICK_GUIDE.md)** - Guía rápida de uso
- **[IMPLEMENTATION_SUMMARY.md](./IMPLEMENTATION_SUMMARY.md)** - Detalles técnicos
- **[CREDENTIALS_AND_SETUP.md](./CREDENTIALS_AND_SETUP.md)** - Setup e instalación
- **[BEFORE_AFTER_COMPARISON.md](./BEFORE_AFTER_COMPARISON.md)** - Comparación de mejoras

### Roles & Permissions

- **Admin**: Full access including hard delete (future feature)
- **User**: Standard CRUD operations

## 🧪 Testing

Run backend unit tests:

```bash
cd backend
dotnet test Tests/Tests.csproj
```

Run role authorization tests only:

```bash
dotnet test Tests/Tests.csproj --filter "RoleAuthorizationTests"
```

**Test Coverage:**

### Authentication & Authorization (AuthServiceTests)
- ✅ Login with valid credentials
- ✅ Reject invalid credentials
- ✅ User registration
- ✅ Password validation

### Course Management (CourseServiceTests)
- ✅ Course creation with Draft status
- ✅ Publishing with/without lessons
- ✅ Soft delete functionality
- ✅ Lesson order uniqueness

### Role-Based Authorization (RoleAuthorizationTests) - **NEW**
- ✅ Regular user can create courses
- ✅ Regular user can edit own courses
- ✅ Regular user can publish (with lessons)
- ✅ Regular user cannot publish (without lessons)
- ✅ Both roles can soft delete
- ✅ Admin has unlimited access
- ✅ Regular user limited to soft delete
- ✅ Multiple users can create courses independently
- ✅ Users can manage lessons in own courses

**Total**: 17 tests | **Status**: All passing ✅

## 🏗 Architecture

```
backend/
├── Domain/          # Entities, Enums, Core Logic
├── Application/     # DTOs, Interfaces, Services
├── Infrastructure/  # EF Core, Repositories, Data Access
├── API/             # Controllers, Startup Configuration
└── Tests/           # Unit Tests

client/
├── src/
│   ├── views/       # Page Components
│   ├── stores/      # Pinia Stores
│   ├── router/      # Vue Router Configuration
│   └── axios.js     # API Client Setup
```

**Clean Architecture Benefits:**

- Separation of concerns
- Testability
- Maintainability
- Independent of frameworks

## 🔗 API Endpoints

### Authentication

```
POST   /api/auth/register
POST   /api/auth/login
```

### Courses

```
GET    /api/courses?pageNumber=1&pageSize=10&status=Draft
GET    /api/courses/{id}
GET    /api/courses/metrics
POST   /api/courses
PUT    /api/courses/{id}
DELETE /api/courses/{id}
POST   /api/courses/{id}/publish
POST   /api/courses/{id}/unpublish
```

### Lessons

```
POST   /api/lessons
PUT    /api/lessons/{id}
DELETE /api/lessons/{id}
PUT    /api/lessons/{id}/move-up
PUT    /api/lessons/{id}/move-down
```

## ✨ Latest Features (v2.0)

✅ **Role-Based Authorization**: Admin and User roles with JWT claims  
✅ **Metrics Dashboard**: Real-time statistics with beautiful cards  
✅ **Docker Support**: One-command deployment with docker-compose  
✅ **Lesson Reordering**: Safe swap algorithm avoiding unique constraints  
✅ **Edit Functionality**: Full edit support for courses and lessons  
✅ **Pagination & Filters**: Server-side pagination with status filtering  
✅ **Soft Delete**: Cascading soft delete for courses and lessons  
✅ **Error Handling**: Comprehensive error messages and validation  
✅ **Swagger UI**: Complete API documentation available in Docker

## 🐳 Docker Details

### Services

**postgres**: PostgreSQL 16 Alpine

- Host Port: 5433
- Container Port: 5432
- Database: course_db
- Persistent volume for data

**api**: .NET 10 Backend

- Host Port: 7260
- Container Port: 5070
- Auto-migrates database on startup
- Includes default admin user seeding
- Swagger UI: http://localhost:7260/swagger/index.html

**frontend**: Vue 3 + Nginx

- Host Port: 5173
- Container Port: 5173
- Production build served by Nginx
- API proxy configured to http://api:5070/api

### Environment Variables

Backend (`docker-compose.yml`):

```yaml
- ConnectionStrings__DefaultConnection=Host=db;Database=course_db;Username=postgres;Password=Qwe.123*
- Jwt__Key=ThisIsASuperSecureKeyForJwtTokenGeneration2024!ThisMustBeAtLeast32Characters!
- Jwt__Issuer=CourseManagementAPI
- Jwt__Audience=CourseManagementClient
- Jwt__ExpirationMinutes=60
```

Frontend:

```yaml
- VITE_API_URL=http://api:5070/api
```

## 🔍 Troubleshooting

### Database Connection Issues

- Ensure PostgreSQL is running
- Check connection string in `appsettings.json`
- Verify database exists: `psql -U postgres -l`

### Frontend API Errors

- Confirm backend is running on port 5070
- Check CORS configuration in `Program.cs`
- Verify JWT token in browser DevTools

### Docker Issues

- Clear volumes: `docker-compose down -v`
- Rebuild images: `docker-compose build --no-cache`
- Check logs: `docker-compose logs api`

## 📝 License

This project is licensed under the Apache License 2.0.

## 👥 Contributing

Contributions are welcome! Please follow clean architecture principles and include tests for new features.

---

**Built with ❤️ using Clean Architecture principles**
