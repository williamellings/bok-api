# BokApi

A .NET 9 Web API backend for the BokApp project, built as a technical test for an internship application. Provides JWT-secured CRUD endpoints for books and personal quotes.

## Features

- 🔐 JWT-based authentication (register/login)
- 📚 Full CRUD for books
- 💬 CRUD for personal quotes, scoped to the logged-in user
- 🗄️ Entity Framework Core with SQL Server
- 🔒 Passwords hashed with BCrypt
- 📄 Swagger/OpenAPI documentation with built-in Authorize support
- 🌐 CORS configured for the Angular frontend

## Tech stack

- .NET 9 / ASP.NET Core Web API
- Entity Framework Core + SQL Server
- JWT Bearer authentication
- BCrypt.Net
- Swashbuckle (Swagger)

## Getting started locally

```bash
cd BokApi
dotnet ef database update
dotnet run --launch-profile https
```

The API runs on `https://localhost:7095`. Swagger UI is available at `https://localhost:7095/swagger`.

## Main endpoints

| Method | Endpoint | Description | Auth required |
|--------|----------|-------------|----------------|
| POST | `/api/auth/register` | Register a new user | No |
| POST | `/api/auth/login` | Log in and receive a JWT | No |
| GET | `/api/books` | Get all books | Yes |
| POST | `/api/books` | Create a book | Yes |
| PUT | `/api/books/{id}` | Update a book | Yes |
| DELETE | `/api/books/{id}` | Delete a book | Yes |
| GET | `/api/quotes` | Get the logged-in user's quotes | Yes |
| POST | `/api/quotes` | Create a quote | Yes |
| PUT | `/api/quotes/{id}` | Update a quote | Yes |
| DELETE | `/api/quotes/{id}` | Delete a quote | Yes |

## Related repo

- 🎨 Frontend (Angular): [bok-app](https://github.com/williamellings/bok-app)
