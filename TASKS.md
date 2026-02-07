# Bookstore API — Task Plan

## Phase 1: Project Scaffolding

- [x] 1. Create the solution file `BookstoreApi.sln` at the repository root.
- [x] 2. Remove any default scaffolding or template code from the solution file (the current `.sln` has no project references — it will be populated as projects are created in the next steps).
- [x] 3. Create the API project at `src/BookstoreApi/BookstoreApi.csproj` targeting `net8.0` using `dotnet new webapi` (or manually), and add it to the solution via `dotnet sln add`. Remove any template-generated scaffolding files (e.g., `WeatherForecast.cs`, sample controllers) that do not belong. Ensure the `.csproj` does not include Swagger packages unless trivially present.
- [x] 4. Create the test project at `tests/BookstoreApi.Tests/BookstoreApi.Tests.csproj` targeting `net8.0` using `dotnet new xunit` (or manually). Add NuGet references: `FluentAssertions`, `Microsoft.AspNetCore.Mvc.Testing`. Add a project reference to the API project. Add it to the solution via `dotnet sln add`. Remove any template-generated test files (e.g., `UnitTest1.cs`).
- [x] 5. Verify the solution builds successfully with `dotnet build`.

## Phase 2: Book Model & Validation

- [ ] 6. Create `src/BookstoreApi/Models/Book.cs` with the `Book` class containing properties: `Id` (int, auto-generated), `Title` (string, required, max 200), `Author` (string, required, max 150), `Isbn` (string, required, must be exactly 13 numeric digits — use `[RegularExpression(@"^\d{13}$")]`), `Price` (decimal, required, must be > 0 — use `[Range(0.01, ...)]` or a custom validation), `Genre` (string, required, max 50). Use Data Annotations for all validation. Add XML doc comments on public members per .NET conventions.

## Phase 3: Service Layer

- [ ] 7. Create `src/BookstoreApi/Services/IBookService.cs` with the `IBookService` interface defining methods: `IEnumerable<Book> GetAll()`, `Book? GetById(int id)`, `Book Add(Book book)`, `bool Update(int id, Book book)`, `bool Delete(int id)`. Add XML doc comments.
- [ ] 8. Create `src/BookstoreApi/Services/BookService.cs` implementing `IBookService` using a thread-safe `ConcurrentDictionary<int, Book>` as the in-memory store. Auto-generate IDs using `Interlocked.Increment` (starting from 0 so first book gets Id 1). Ensure all methods are thread-safe. `GetAll()` returns all values, `GetById()` returns null if not found, `Update()` returns false if Id not found, `Delete()` returns false if Id not found.

## Phase 4: Controller

- [ ] 9. Create `src/BookstoreApi/Controllers/BooksController.cs` with `[ApiController]` and `[Route("api/books")]` attributes. Inject `IBookService` via constructor. Implement all five endpoints delegating to the service:
  - `GET /api/books` → 200 with list of books
  - `GET /api/books/{id}` → 200 with book, or 404 if not found
  - `POST /api/books` → 201 with created book and `Location` header (use `CreatedAtAction`)
  - `PUT /api/books/{id}` → 204 on success, 404 if not found, 400 if validation fails. If the Id in the request body does not match the `{id}` in the URL, return 400.
  - `DELETE /api/books/{id}` → 204 on success, 404 if not found
  - The controller must contain no business logic — only delegation to the service.

## Phase 5: Program.cs & DI Registration

- [ ] 10. Configure `src/BookstoreApi/Program.cs`: register `BookService` as a singleton implementation of `IBookService` via `builder.Services.AddSingleton<IBookService, BookService>()`. Add controllers via `builder.Services.AddControllers()`. Map controller routes via `app.MapControllers()`. Ensure the app listens on port 8080 for Docker compatibility (set via `ASPNETCORE_URLS` environment variable or `builder.WebHost.UseUrls`). Make the `Program` class accessible to the test project for `WebApplicationFactory<Program>` — either add `InternalsVisibleTo` in the `.csproj` or add a `public partial class Program { }` declaration at the bottom of `Program.cs`.
- [ ] 11. Verify the solution builds and the API starts locally with `dotnet run --project src/BookstoreApi` responding on port 8080. Test with a quick `curl http://localhost:8080/api/books` or equivalent to confirm a 200 response with `[]`.

## Phase 6: Unit Tests

- [ ] 12. Create `tests/BookstoreApi.Tests/BookServiceTests.cs` with xUnit tests using FluentAssertions, covering all 8 required scenarios:
  1. Adding a book returns the book with a generated Id (Id > 0).
  2. GetAll returns all added books.
  3. GetById with a valid Id returns the correct book.
  4. GetById with a non-existent Id returns null.
  5. Updating an existing book succeeds (returns true) and persists the changes.
  6. Updating a non-existent book returns false.
  7. Deleting an existing book succeeds (returns true) and the book is no longer retrievable.
  8. Deleting a non-existent book returns false.

## Phase 7: Integration Tests

- [ ] 13. Create `tests/BookstoreApi.Tests/BooksControllerTests.cs` using `WebApplicationFactory<Program>` and xUnit with FluentAssertions to test all 10 required integration scenarios:
  1. GET /api/books returns 200 and an empty JSON array when no books exist.
  2. POST /api/books with valid data returns 201, the created book in the body, and a Location header.
  3. POST /api/books with invalid data returns 400 — test at least three sub-cases: missing required fields, invalid ISBN (non-13-digit), and negative/zero price.
  4. GET /api/books/{id} with a valid Id returns 200 and the correct book.
  5. GET /api/books/{id} with a non-existent Id returns 404.
  6. PUT /api/books/{id} with valid data returns 204 and the changes persist (verify with a subsequent GET).
  7. PUT /api/books/{id} with a non-existent Id returns 404.
  8. PUT /api/books/{id} with invalid data (missing required fields, bad ISBN, negative price) returns 400.
  9. DELETE /api/books/{id} with a valid Id returns 204.
  10. DELETE /api/books/{id} with a non-existent Id returns 404.
- [ ] 14. Run `dotnet test` and verify all unit and integration tests pass with zero failures.

## Phase 8: Dockerfile & .dockerignore

- [ ] 15. Create a `.dockerignore` file at the repository root excluding: `bin/`, `obj/`, `.git/`, `.github/`, `*.md`, `*.sln.DotSettings`, `TestResults/`, and other non-essential files.
- [ ] 16. Create a multi-stage `Dockerfile` at the repository root:
  - **Stage 1 (build):** Use `mcr.microsoft.com/dotnet/sdk:8.0` as the base. Copy `.sln` and all `.csproj` files first for layer caching, run `dotnet restore`. Then copy all source, run `dotnet publish -c Release -o /app/publish`.
  - **Stage 2 (runtime):** Use `mcr.microsoft.com/dotnet/aspnet:8.0` as the base. Copy published output from build stage. `EXPOSE 8080`. Set `ENV ASPNETCORE_URLS=http://+:8080`. Set `ENTRYPOINT ["dotnet", "BookstoreApi.dll"]`.
- [ ] 17. Verify the Docker image builds successfully with `docker build -t bookstore-api .`.

## Phase 9: GitHub Actions CI Workflow

- [ ] 18. Create `.github/workflows/ci.yml` with:
  - **Triggers:** `push` to `main` branch and all `pull_request` events.
  - **Runner:** `ubuntu-latest`.
  - **Steps:** (1) `actions/checkout@v4`, (2) `actions/setup-dotnet@v4` with `dotnet-version: '8.0.x'`, (3) `dotnet restore`, (4) `dotnet build --configuration Release --no-restore`, (5) `dotnet test --configuration Release --no-build --verbosity normal`, (6) `docker build -t bookstore-api .`.
  - Ensure the working directory is correct if the solution is not at the repo root (use `working-directory` or adjust paths).

## Phase 10: Documentation & Final Verification

- [ ] 19. Review and update `README.md` to accurately describe: project overview, prerequisites (.NET 8, Docker), how to build and run locally, how to run tests, how to build and run via Docker on port 8080, API endpoints table with request/response details, and project structure tree. Ensure it matches what was actually built.
- [ ] 20. Verify the complete project structure matches the layout defined in SPEC.md — all required files exist in the correct directories: `src/BookstoreApi/Controllers/BooksController.cs`, `src/BookstoreApi/Models/Book.cs`, `src/BookstoreApi/Services/IBookService.cs`, `src/BookstoreApi/Services/BookService.cs`, `src/BookstoreApi/Program.cs`, `src/BookstoreApi/BookstoreApi.csproj`, `tests/BookstoreApi.Tests/BooksControllerTests.cs`, `tests/BookstoreApi.Tests/BookServiceTests.cs`, `tests/BookstoreApi.Tests/BookstoreApi.Tests.csproj`, `Dockerfile`, `.github/workflows/ci.yml`, `BookstoreApi.sln`.
- [ ] 21. Run full end-to-end verification: `dotnet build` succeeds, `dotnet test` passes all tests with zero failures, `docker build -t bookstore-api .` succeeds, start the container with `docker run -d -p 8080:8080 bookstore-api`, and verify a full CRUD cycle against the running container: POST a book → GET by Id → PUT update → GET to verify update → DELETE → GET returns 404. Stop and remove the container afterward.
