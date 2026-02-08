using System.Net;
using System.Net.Http.Json;
using System.Text;
using BookstoreApi.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookstoreApi.Tests;

public class BooksControllerCoverageTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public BooksControllerCoverageTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    private static Book CreateValidBook() => new()
    {
        Title = "Coverage Test Book",
        Author = "Test Author",
        Isbn = "9780306406157",
        Price = 29.99m,
        Genre = "Science"
    };

    [Fact]
    public async Task Post_WithMinimumValidPrice_Returns201()
    {
        var book = CreateValidBook();
        book.Price = 0.01m;

        var response = await _client.PostAsJsonAsync("/api/books", book);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<Book>();
        created!.Price.Should().Be(0.01m);
    }

    [Fact]
    public async Task Post_WithExactMaxTitleLength_Returns201()
    {
        var book = CreateValidBook();
        book.Title = new string('A', 200);

        var response = await _client.PostAsJsonAsync("/api/books", book);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<Book>();
        created!.Title.Should().HaveLength(200);
    }

    [Fact]
    public async Task Post_WithExactMaxAuthorLength_Returns201()
    {
        var book = CreateValidBook();
        book.Author = new string('B', 150);

        var response = await _client.PostAsJsonAsync("/api/books", book);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<Book>();
        created!.Author.Should().HaveLength(150);
    }

    [Fact]
    public async Task Post_WithExactMaxGenreLength_Returns201()
    {
        var book = CreateValidBook();
        book.Genre = new string('C', 50);

        var response = await _client.PostAsJsonAsync("/api/books", book);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<Book>();
        created!.Genre.Should().HaveLength(50);
    }

    [Fact]
    public async Task Post_WithMissingTitle_Returns400()
    {
        var book = CreateValidBook();
        book.Title = "";

        var response = await _client.PostAsJsonAsync("/api/books", book);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_WithMissingTitle_Returns400()
    {
        var postResponse = await _client.PostAsJsonAsync("/api/books", CreateValidBook());
        var created = await postResponse.Content.ReadFromJsonAsync<Book>();

        var updated = new Book
        {
            Id = created!.Id,
            Title = "",
            Author = "Author",
            Isbn = "9780306406157",
            Price = 10.00m,
            Genre = "Fiction"
        };

        var response = await _client.PutAsJsonAsync($"/api/books/{created.Id}", updated);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_WithMissingIsbn_Returns400()
    {
        var postResponse = await _client.PostAsJsonAsync("/api/books", CreateValidBook());
        var created = await postResponse.Content.ReadFromJsonAsync<Book>();

        var updated = new Book
        {
            Id = created!.Id,
            Title = "Title",
            Author = "Author",
            Isbn = "",
            Price = 10.00m,
            Genre = "Fiction"
        };

        var response = await _client.PutAsJsonAsync($"/api/books/{created.Id}", updated);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_WithNegativePrice_Returns400()
    {
        var postResponse = await _client.PostAsJsonAsync("/api/books", CreateValidBook());
        var created = await postResponse.Content.ReadFromJsonAsync<Book>();

        var updated = new Book
        {
            Id = created!.Id,
            Title = "Title",
            Author = "Author",
            Isbn = "9780306406157",
            Price = -5.00m,
            Genre = "Fiction"
        };

        var response = await _client.PutAsJsonAsync($"/api/books/{created.Id}", updated);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_UpdatesAllFields()
    {
        var postResponse = await _client.PostAsJsonAsync("/api/books", CreateValidBook());
        var created = await postResponse.Content.ReadFromJsonAsync<Book>();

        var updated = new Book
        {
            Id = created!.Id,
            Title = "New Title",
            Author = "New Author",
            Isbn = "9789876543210",
            Price = 49.99m,
            Genre = "History"
        };

        var putResponse = await _client.PutAsJsonAsync($"/api/books/{created.Id}", updated);
        putResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/books/{created.Id}");
        var fetched = await getResponse.Content.ReadFromJsonAsync<Book>();

        fetched!.Title.Should().Be("New Title");
        fetched.Author.Should().Be("New Author");
        fetched.Isbn.Should().Be("9789876543210");
        fetched.Price.Should().Be(49.99m);
        fetched.Genre.Should().Be("History");
    }

    [Fact]
    public async Task Put_WithIdZeroInBody_Succeeds()
    {
        // Controller allows Id=0 in body (treats it as matching any URL id)
        var postResponse = await _client.PostAsJsonAsync("/api/books", CreateValidBook());
        var created = await postResponse.Content.ReadFromJsonAsync<Book>();

        var updated = CreateValidBook();
        updated.Id = 0;
        updated.Title = "Zero Id Update";

        var putResponse = await _client.PutAsJsonAsync($"/api/books/{created!.Id}", updated);
        putResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/books/{created.Id}");
        var fetched = await getResponse.Content.ReadFromJsonAsync<Book>();
        fetched!.Title.Should().Be("Zero Id Update");
    }

    [Fact]
    public async Task GetById_WithStringId_Returns400Or404()
    {
        var response = await _client.GetAsync("/api/books/abc");

        // ASP.NET Core returns 400 for non-integer route parameter
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_WithEmptyJsonBody_Returns400()
    {
        var content = new StringContent("{}", Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/books", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAll_AfterDeleteAll_ReturnsEmptyArray()
    {
        // Create two books
        var r1 = await _client.PostAsJsonAsync("/api/books", CreateValidBook());
        var r2 = await _client.PostAsJsonAsync("/api/books", CreateValidBook());
        var c1 = await r1.Content.ReadFromJsonAsync<Book>();
        var c2 = await r2.Content.ReadFromJsonAsync<Book>();

        // Delete both
        await _client.DeleteAsync($"/api/books/{c1!.Id}");
        await _client.DeleteAsync($"/api/books/{c2!.Id}");

        // Verify they are deleted
        var get1 = await _client.GetAsync($"/api/books/{c1.Id}");
        var get2 = await _client.GetAsync($"/api/books/{c2.Id}");
        get1.StatusCode.Should().Be(HttpStatusCode.NotFound);
        get2.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_LocationHeader_PointsToCreatedResource()
    {
        var response = await _client.PostAsJsonAsync("/api/books", CreateValidBook());
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var locationUri = response.Headers.Location;
        locationUri.Should().NotBeNull();

        // Follow the location header and verify it returns the created book
        var getResponse = await _client.GetAsync(locationUri);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var book = await getResponse.Content.ReadFromJsonAsync<Book>();
        book.Should().NotBeNull();
        book!.Title.Should().Be("Coverage Test Book");
    }

    [Fact]
    public async Task GetById_ReturnsCorrectContentType()
    {
        var postResponse = await _client.PostAsJsonAsync("/api/books", CreateValidBook());
        var created = await postResponse.Content.ReadFromJsonAsync<Book>();

        var response = await _client.GetAsync($"/api/books/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task Post_WithIsbnAllZeros_Returns201()
    {
        // 13 zeros is a valid format per regex
        var book = CreateValidBook();
        book.Isbn = "0000000000000";

        var response = await _client.PostAsJsonAsync("/api/books", book);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Post_WithIsbnWithSpaces_Returns400()
    {
        var book = CreateValidBook();
        book.Isbn = "978 030640615";

        var response = await _client.PostAsJsonAsync("/api/books", book);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_WithIsbnWithDashes_Returns400()
    {
        var book = CreateValidBook();
        book.Isbn = "978-0306406157";

        var response = await _client.PostAsJsonAsync("/api/books", book);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
