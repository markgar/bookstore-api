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

| Method | Endpoint         | Description        |
|--------|------------------|--------------------|
| GET    | /api/books       | List all books     |
| GET    | /api/books/{id}  | Get a book by ID   |
| POST   | /api/books       | Create a new book  |
| PUT    | /api/books/{id}  | Update a book      |
| DELETE | /api/books/{id}  | Delete a book      |

## License

MIT
