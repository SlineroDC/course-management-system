# Course Management System

A full-stack course management application built with .NET 10 (Backend) and Vue 3 (Frontend).

## Features

- **Authentication**: Identity + JWT (Login/Register).
- **Course Management**: Create, Read, Delete (Soft Delete), Publish.
- **Lesson Management**: Add lessons to courses.
- **Business Rules**:
  - Courses cannot be published without lessons.
  - Soft Delete for courses.
- **Architecture**: Clean Architecture "Lite" (Domain, Application, Infrastructure, API).

## Tech Stack

- **Backend**: .NET 10, EF Core, PostgreSQL (configured for development with SQLite/InMemory for simplicity if needed, current setup uses Postgres logic but check connection string), xUnit.
- **Frontend**: Vue 3, Vite, Tailwind CSS, Pinia, Axios.

## Getting Started

### Prerequisites

- .NET 10 SDK
- Node.js & npm

### Backend Setup

1.  **Database Configuration**:

    - The project uses **PostgreSQL** by default.
    - Update the connection string in `backend/API/appsettings.json` if needed:
      ```json
      "ConnectionStrings": {
        "DefaultConnection": "Host=localhost;Database=CourseDb;Username=postgres;Password=yourpassword"
      }
      ```

2.  **Migrations**:

    - Apply migrations to create the database schema:
      ```bash
      dotnet ef database update --project backend/Infrastructure --startup-project backend/API
      ```

3.  **Run the Application**:
    ```bash
    cd backend/API
    dotnet run
    ```
    - The API will start at `http://localhost:5070`.
    - **Default User**: `admin@example.com` / `Password123!`

### Frontend Setup

1.  Navigate to the client directory:
    ```bash
    cd client
    ```
2.  Install dependencies:
    ```bash
    npm install
    ```
3.  Run the development server:
    ```bash
    npm run dev
    ```
    - The application will be available at `http://localhost:5173`.

## Usage Guide

1.  **Login**: Use the default credentials or register a new user.
2.  **Dashboard**:
    - **Create Course**: Enter a title and click "Create".
    - **Add Lesson**: Click "Add Lesson" on a course card.
    - **Publish**: Click "Publish". **Note**: This will fail with an error if the course has no lessons (Business Rule).
    - **Delete**: Click "Delete" to soft-delete the course (removes it from the list).

## Testing

Run the backend unit tests:

```bash
dotnet test backend/Tests/Tests.csproj
```

## 📚 Documentation

- **[Quick Guide](./QUICK_GUIDE.md)** - Guía rápida de uso
- **[Credentials & Setup](./CREDENTIALS_AND_SETUP.md)** - Credenciales de prueba y configuración
- **[Implementation Summary](./IMPLEMENTATION_SUMMARY.md)** - Resumen técnico completo

## ✨ Últimas Mejoras (v1.0)

✅ **Validación de Order**: Min 1, No duplicados
✅ **Reordenamiento**: Botones 🔼/🔽 para mover lecciones
✅ **Edición sin Duplicados**: PUT /lessons/{id}
✅ **Soft Delete**: Las lecciones eliminadas no aparecen
✅ **Mensajes de Error**: Captura y muestra de errores 400 BAD REQUEST
✅ **Sincronización Automática**: UI actualiza después de cada acción
✅ **Botón Despublicar**: Cambiar de Published a Draft

## 🔗 Endpoints API

```
Cursos:
  GET    /api/courses?pageNumber=1&pageSize=10&status=Draft
  POST   /api/courses
  PUT    /api/courses/{id}
  DELETE /api/courses/{id}
  POST   /api/courses/{id}/publish
  POST   /api/courses/{id}/unpublish

Lecciones:
  POST   /api/lessons
  PUT    /api/lessons/{id}
  DELETE /api/lessons/{id}
  PUT    /api/lessons/{id}/move-up
  PUT    /api/lessons/{id}/move-down
```

---

