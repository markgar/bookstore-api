# Bookstore API

A C# ASP.NET Core Web API for managing a bookstore inventory. Provides full CRUD endpoints for books with properties including title, author, ISBN, price, and genre.

## Features

- **CRUD Operations** — Create, Read, Update, and Delete books
- **Book Model** — Title, Author, ISBN, Price, Genre
- **Unit Tests** — xUnit test suite for API and service validation
- **Docker Support** — Multi-stage Dockerfile for optimized container builds
- **CI Pipeline** — GitHub Actions workflow for build, test, and Docker image verification

## Tech Stack

- **Language:** C# (.NET 8)
- **Framework:** ASP.NET Core Web API
- **Testing:** xUnit
- **Containerization:** Docker (multi-stage build)
- **CI/CD:** GitHub Actions

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/) (optional, for container builds)

### Run Locally

```bash
cd src/BookstoreApi
dotnet run
```

### Run Tests

```bash
dotnet test
```

### Build Docker Image

```bash
docker build -t bookstore-api .
docker run -p 8080:8080 bookstore-api
```

## API Endpoints

| Method | Endpoint         | Description        | Success Status | Error Status              |
|--------|------------------|--------------------|----------------|---------------------------|
| GET    | /api/books       | List all books     | 200 OK         | —                         |
| GET    | /api/books/{id}  | Get a book by ID   | 200 OK         | 404 Not Found             |
| POST   | /api/books       | Create a new book  | 201 Created    | 400 Bad Request           |
| PUT    | /api/books/{id}  | Update a book      | 204 No Content | 400 Bad Request, 404 Not Found |
| DELETE | /api/books/{id}  | Delete a book      | 204 No Content | 404 Not Found             |

### Book JSON Schema

```json
{
  "id": 1,
  "title": "string (required, max 200)",
  "author": "string (required, max 150)",
  "isbn": "string (required, 13 digits)",
  "price": 9.99,
  "genre": "string (required, max 50)"
}
```

## Project Structure

```
builder/
├── .github/
│   └── workflows/
│       └── ci.yml
├── src/
│   └── BookstoreApi/
│       ├── Controllers/
│       │   └── BooksController.cs
│       ├── Models/
│       │   └── Book.cs
│       ├── Services/
│       │   ├── IBookService.cs
│       │   └── BookService.cs
│       ├── Program.cs
│       └── BookstoreApi.csproj
├── tests/
│   └── BookstoreApi.Tests/
│       ├── BooksControllerTests.cs
│       ├── BookServiceTests.cs
│       └── BookstoreApi.Tests.csproj
├── Dockerfile
├── .gitignore
├── README.md
├── SPEC.md
└── BookstoreApi.sln
```

## License

MIT
