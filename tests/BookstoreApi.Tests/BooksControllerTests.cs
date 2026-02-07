using System.Net;
using System.Net.Http.Json;
using BookstoreApi.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookstoreApi.Tests;

public class BooksControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public BooksControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    private static Book CreateValidBook() => new()
    {
        Title = "Integration Test Book",
        Author = "Test Author",
        Isbn = "9780306406157",
        Price = 29.99m,
        Genre = "Science"
    };

    [Fact]
    public async Task GetAll_WhenNoBooksExist_ReturnsEmptyArray()
    {
        var response = await _client.GetAsync("/api/books");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var books = await response.Content.ReadFromJsonAsync<List<Book>>();
        books.Should().NotBeNull();
    }

    [Fact]
    public async Task Post_WithValidData_Returns201AndCreatedBook()
    {
        var book = CreateValidBook();

        var response = await _client.PostAsJsonAsync("/api/books", book);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        var created = await response.Content.ReadFromJsonAsync<Book>();
        created.Should().NotBeNull();
        created!.Id.Should().BeGreaterThan(0);
        created.Title.Should().Be(book.Title);
    }

    [Theory]
    [InlineData("", "Author", "9780306406157", 29.99, "Genre")]
    [InlineData("Title", "Author", "123", 29.99, "Genre")]
    [InlineData("Title", "Author", "9780306406157", -1, "Genre")]
    public async Task Post_WithInvalidData_Returns400(
        string title, string author, string isbn, decimal price, string genre)
    {
        var book = new Book
        {
            Title = title,
            Author = author,
            Isbn = isbn,
            Price = price,
            Genre = genre
        };

        var response = await _client.PostAsJsonAsync("/api/books", book);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetById_WithValidId_Returns200AndBook()
    {
        var book = CreateValidBook();
        var postResponse = await _client.PostAsJsonAsync("/api/books", book);
        var created = await postResponse.Content.ReadFromJsonAsync<Book>();

        var response = await _client.GetAsync($"/api/books/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Book>();
        result!.Title.Should().Be(book.Title);
    }

    [Fact]
    public async Task GetById_WithNonExistentId_Returns404()
    {
        var response = await _client.GetAsync("/api/books/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Put_WithValidData_Returns204AndPersistsChanges()
    {
        var book = CreateValidBook();
        var postResponse = await _client.PostAsJsonAsync("/api/books", book);
        var created = await postResponse.Content.ReadFromJsonAsync<Book>();

        var updated = CreateValidBook();
        updated.Id = created!.Id;
        updated.Title = "Updated Title";

        var putResponse = await _client.PutAsJsonAsync($"/api/books/{created.Id}", updated);
        putResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/books/{created.Id}");
        var result = await getResponse.Content.ReadFromJsonAsync<Book>();
        result!.Title.Should().Be("Updated Title");
    }

    [Fact]
    public async Task Put_WithNonExistentId_Returns404()
    {
        var book = CreateValidBook();

        var response = await _client.PutAsJsonAsync("/api/books/99999", book);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("", "Author", "9780306406157", 29.99, "Genre")]
    [InlineData("Title", "Author", "123", 29.99, "Genre")]
    [InlineData("Title", "Author", "9780306406157", -1, "Genre")]
    public async Task Put_WithInvalidData_Returns400(
        string title, string author, string isbn, decimal price, string genre)
    {
        var postResponse = await _client.PostAsJsonAsync("/api/books", CreateValidBook());
        var created = await postResponse.Content.ReadFromJsonAsync<Book>();

        var book = new Book
        {
            Id = created!.Id,
            Title = title,
            Author = author,
            Isbn = isbn,
            Price = price,
            Genre = genre
        };

        var response = await _client.PutAsJsonAsync($"/api/books/{created.Id}", book);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_WithValidId_Returns204()
    {
        var postResponse = await _client.PostAsJsonAsync("/api/books", CreateValidBook());
        var created = await postResponse.Content.ReadFromJsonAsync<Book>();

        var response = await _client.DeleteAsync($"/api/books/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_WithNonExistentId_Returns404()
    {
        var response = await _client.DeleteAsync("/api/books/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
