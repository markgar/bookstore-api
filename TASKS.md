# Bookstore API — Task Plan

## Phase 1: Project Scaffolding

- [x] 1. Create the solution file `BookstoreApi.sln` at the repository root.
- [ ] 2. Create the API project at `src/BookstoreApi/BookstoreApi.csproj` targeting `net8.0`, and add it to the solution. Include NuGet package references needed (e.g., `Microsoft.AspNetCore.OpenApi` is optional but no extra effort needed).
- [ ] 3. Create the test project at `tests/BookstoreApi.Tests/BookstoreApi.Tests.csproj` targeting `net8.0` with references to xUnit, FluentAssertions, Microsoft.AspNetCore.Mvc.Testing, and a project reference to the API project. Add it to the solution.
- [ ] 4. Verify the solution builds successfully with `dotnet build`.

## Phase 2: Book Model & Validation

- [ ] 5. Create `src/BookstoreApi/Models/Book.cs` with the `Book` class containing properties: `Id` (int, auto-generated), `Title` (string, required, max 200), `Author` (string, required, max 150), `Isbn` (string, required, 13-digit numeric via regex), `Price` (decimal, required, > 0), `Genre` (string, required, max 50). Use Data Annotations for validation.

## Phase 3: Service Layer

- [ ] 6. Create `src/BookstoreApi/Services/IBookService.cs` with the `IBookService` interface defining methods: `GetAll()`, `GetById(int id)`, `Add(Book book)`, `Update(int id, Book book)`, `Delete(int id)`.
- [ ] 7. Create `src/BookstoreApi/Services/BookService.cs` implementing `IBookService` using a thread-safe `ConcurrentDictionary<int, Book>` as the in-memory store. Auto-generate IDs using `Interlocked.Increment`. Ensure all methods are thread-safe.

## Phase 4: Controller

- [ ] 8. Create `src/BookstoreApi/Controllers/BooksController.cs` with `[ApiController]` and `[Route("api/books")]` attributes. Inject `IBookService` via constructor. Implement all five endpoints (GET all, GET by id, POST, PUT, DELETE) delegating to the service. POST should return `CreatedAtAction` with a `Location` header. The controller must contain no business logic.

## Phase 5: Program.cs & DI Registration

- [ ] 9. Configure `src/BookstoreApi/Program.cs` to register `BookService` as a singleton implementation of `IBookService`, add controllers, and map controller routes. Ensure the app listens on port 8080 (for Docker compatibility). Do not add Swagger unless trivially included by the template.
- [ ] 10. Verify the solution builds and the API starts locally with `dotnet run` responding on the configured port.

## Phase 6: Unit Tests

- [ ] 11. Create `tests/BookstoreApi.Tests/BookServiceTests.cs` with xUnit tests covering all 8 required scenarios: (1) Add book returns book with generated Id, (2) GetAll returns all added books, (3) GetById returns correct book, (4) GetById for non-existent Id returns null, (5) Update existing book succeeds and persists, (6) Update non-existent book returns false, (7) Delete existing book succeeds, (8) Delete non-existent book returns false. Use FluentAssertions for assertions.

## Phase 7: Integration Tests

- [ ] 12. Create `tests/BookstoreApi.Tests/BooksControllerTests.cs` using `WebApplicationFactory<Program>` and xUnit to test all 9 required integration scenarios: (1) GET /api/books returns 200 with empty list, (2) POST valid data returns 201 with created book, (3) POST invalid data returns 400 (missing fields, bad ISBN, negative price — at least three sub-cases), (4) GET by valid Id returns 200, (5) GET by invalid Id returns 404, (6) PUT valid data returns 204, (7) PUT non-existent Id returns 404, (8) DELETE valid Id returns 204, (9) DELETE non-existent Id returns 404.
- [ ] 13. Run `dotnet test` and verify all unit and integration tests pass with zero failures.

## Phase 8: Dockerfile & .dockerignore

- [ ] 14. Create a `.dockerignore` file excluding `bin/`, `obj/`, `.git/`, `.github/`, `*.md`, and other non-essential files.
- [ ] 15. Create a multi-stage `Dockerfile` at the repository root. Stage 1 (build): use `mcr.microsoft.com/dotnet/sdk:8.0` to restore, build, and publish in Release mode. Stage 2 (runtime): use `mcr.microsoft.com/dotnet/aspnet:8.0`, copy published output, expose port 8080, set `ASPNETCORE_URLS` to `http://+:8080`, and set the entrypoint to run the API.
- [ ] 16. Verify the Docker image builds successfully with `docker build -t bookstore-api .`.

## Phase 9: GitHub Actions CI Workflow

- [ ] 17. Create `.github/workflows/ci.yml` triggered on push to `main` and all pull_request events. Use `ubuntu-latest` runner. Steps: (1) checkout repo, (2) setup .NET 8 SDK, (3) restore NuGet packages, (4) build solution in Release configuration, (5) run all xUnit tests, (6) build Docker image.

## Phase 10: Documentation & Final Verification

- [ ] 18. Review and update `README.md` to accurately describe the project, setup/run instructions (local and Docker), how to run tests, API endpoint documentation, and project structure. Ensure it matches what was actually built.
- [ ] 19. Verify the complete project structure matches the layout defined in SPEC.md (all files in correct directories).
- [ ] 20. Run full end-to-end verification: `dotnet build` succeeds, `dotnet test` passes all tests, `docker build` succeeds, and the API responds to CRUD requests (create, read, update, delete cycle) when running in the container on port 8080.
