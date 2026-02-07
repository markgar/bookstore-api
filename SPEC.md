# Bookstore API — Project Specification

## High-Level Summary

This project is a RESTful Web API for a bookstore, built with C# and ASP.NET Core. It exposes CRUD endpoints for managing a collection of books. The project includes a comprehensive xUnit test suite, a multi-stage Dockerfile for production-ready container images, and a GitHub Actions CI workflow that builds, tests, and verifies the Docker image on every push and pull request.

The API uses an in-memory data store (no external database dependency) to keep the scope focused on API design, testing, containerization, and CI.

---

## Tech Stack & Language

| Component         | Technology                      |
|-------------------|---------------------------------|
| Language          | C# 12                           |
| Runtime           | .NET 8 (LTS)                    |
| Framework         | ASP.NET Core Web API (Minimal APIs or Controllers) |
| Testing Framework | xUnit with FluentAssertions     |
| HTTP Testing      | Microsoft.AspNetCore.Mvc.Testing (WebApplicationFactory) |
| Containerization  | Docker (multi-stage build)      |
| CI/CD             | GitHub Actions                  |
| Data Store        | In-memory (e.g., ConcurrentDictionary or EF Core InMemory) |

---

## Features & Requirements

### Book Model

The `Book` entity must have the following properties:

| Property | Type     | Constraints                                      |
|----------|----------|--------------------------------------------------|
| Id       | int      | Auto-generated, unique identifier                |
| Title    | string   | Required, max length 200                         |
| Author   | string   | Required, max length 150                         |
| Isbn     | string   | Required, must be a valid ISBN-13 format (13 digits) |
| Price    | decimal  | Required, must be > 0                            |
| Genre    | string   | Required, max length 50                          |

### API Endpoints

All endpoints are under the `/api/books` route prefix and return JSON.

| Method | Route            | Description              | Success Status | Error Status           |
|--------|------------------|--------------------------|----------------|------------------------|
| GET    | /api/books       | Retrieve all books       | 200 OK         | —                      |
| GET    | /api/books/{id}  | Retrieve a single book   | 200 OK         | 404 Not Found          |
| POST   | /api/books       | Create a new book        | 201 Created    | 400 Bad Request        |
| PUT    | /api/books/{id}  | Update an existing book  | 204 No Content | 400 Bad Request, 404 Not Found |
| DELETE | /api/books/{id}  | Delete a book            | 204 No Content | 404 Not Found          |

#### Behavior Details

- **GET /api/books**: Returns an array of all books. Returns an empty array `[]` if no books exist.
- **GET /api/books/{id}**: Returns the book with the given ID. Returns 404 if the book does not exist.
- **POST /api/books**: Accepts a JSON body with Title, Author, Isbn, Price, and Genre. Returns the created book with its generated Id and a `Location` header pointing to the new resource. Returns 400 if validation fails.
- **PUT /api/books/{id}**: Accepts a JSON body with updated fields. The Id in the URL must match. Returns 204 on success. Returns 404 if the book does not exist. Returns 400 if validation fails.
- **DELETE /api/books/{id}**: Deletes the book with the given ID. Returns 204 on success. Returns 404 if the book does not exist.

### Input Validation

- All required fields must be present and non-empty on POST and PUT.
- ISBN must be exactly 13 digits (numeric characters only).
- Price must be greater than zero.
- Validation errors return 400 with a structured problem details response body.

### Project Structure

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

### Service Layer

- A `BookService` class (implementing `IBookService`) encapsulates all data access logic using an in-memory store.
- The controller delegates to the service; the controller itself should contain no business logic.
- The service is registered via dependency injection as a singleton.

### Testing Requirements

Tests must be written using **xUnit** and should cover:

1. **Unit Tests (BookServiceTests)**
   - Adding a book returns the book with a generated Id.
   - Getting all books returns all added books.
   - Getting a book by Id returns the correct book.
   - Getting a book by non-existent Id returns null.
   - Updating an existing book succeeds and persists changes.
   - Updating a non-existent book returns false / fails gracefully.
   - Deleting an existing book succeeds.
   - Deleting a non-existent book returns false / fails gracefully.

2. **Integration Tests (BooksControllerTests)**
   - GET /api/books returns 200 and an empty list initially.
   - POST /api/books with valid data returns 201 and the created book.
   - POST /api/books with invalid data (missing fields, bad ISBN, negative price) returns 400.
   - GET /api/books/{id} with a valid Id returns 200 and the book.
   - GET /api/books/{id} with an invalid Id returns 404.
   - PUT /api/books/{id} with valid data returns 204.
   - PUT /api/books/{id} with non-existent Id returns 404.
   - DELETE /api/books/{id} with a valid Id returns 204.
   - DELETE /api/books/{id} with a non-existent Id returns 404.

### Dockerfile

- **Multi-stage build** with at least two stages:
  1. **Build stage**: Use the .NET SDK image to restore, build, and publish the application.
  2. **Runtime stage**: Use the ASP.NET Core runtime image to run the published output.
- Expose port 8080.
- The container should start the API on launch with no additional configuration required.
- Use `.dockerignore` to exclude unnecessary files (bin, obj, .git, etc.).

### GitHub Actions CI Workflow

The workflow file (`.github/workflows/ci.yml`) must:

- Trigger on `push` to `main` and on all `pull_request` events.
- Use a matrix or single job with the following steps:
  1. Checkout the repository.
  2. Set up the .NET 8 SDK.
  3. Restore NuGet packages.
  4. Build the solution in Release configuration.
  5. Run all xUnit tests.
  6. Build the Docker image (verify it builds successfully).
- Use `ubuntu-latest` as the runner OS.

---

## Constraints & Guidelines

1. **No external database** — use an in-memory store only. This keeps the project self-contained and simplifies testing.
2. **No authentication/authorization** — out of scope for this project.
3. **Controllers pattern** — use the traditional `[ApiController]` controller pattern (not Minimal APIs).
4. **Follow standard .NET conventions** — PascalCase for public members, `I`-prefix for interfaces, XML doc comments on public API surface.
5. **Use Data Annotations** for model validation where possible.
6. **Solution file** — a `BookstoreApi.sln` at the repository root that references both the API project and the test project.
7. **Thread safety** — the in-memory store must be thread-safe (use `ConcurrentDictionary` or equivalent).
8. **No Swagger/OpenAPI** — not required, but acceptable if included. Do not spend effort on it.
9. **Target .NET 8** — use `net8.0` target framework in all `.csproj` files.

---

## Acceptance Criteria

The project is considered **done** when all of the following are true:

- [ ] The solution builds without errors: `dotnet build` succeeds.
- [ ] All xUnit tests pass: `dotnet test` reports zero failures.
- [ ] The Docker image builds successfully: `docker build -t bookstore-api .` completes without errors.
- [ ] The API starts in the Docker container and responds to requests on port 8080.
- [ ] The GitHub Actions CI workflow runs on push to `main` and completes all steps (build, test, Docker build) successfully.
- [ ] GET /api/books returns 200 with an empty array when no books have been added.
- [ ] A full CRUD cycle works: create a book (POST), read it (GET by ID), update it (PUT), verify the update (GET by ID), delete it (DELETE), and confirm deletion (GET returns 404).
- [ ] Invalid input to POST and PUT returns 400 with appropriate error details.
- [ ] The project structure matches the layout defined in this spec.
- [ ] The README accurately describes the project, setup instructions, and API endpoints.
