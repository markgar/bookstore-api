using System.Net;
using System.Net.Http.Json;
using BookstoreApi.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookstoreApi.Tests;

public class BooksControllerEdgeCaseTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public BooksControllerEdgeCaseTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    private static Book CreateValidBook() => new()
    {
        Title = "Edge Case Book",
        Author = "Test Author",
        Isbn = "9780306406157",
        Price = 29.99m,
        Genre = "Science"
    };

    [Fact]
    public async Task Post_WithMissingAuthor_Returns400()
    {
        var book = new Book
        {
            Title = "Some Title",
            Author = "",
            Isbn = "9780306406157",
            Price = 10.00m,
            Genre = "Fiction"
        };

        var response = await _client.PostAsJsonAsync("/api/books", book);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_WithMissingGenre_Returns400()
    {
        var book = new Book
        {
            Title = "Some Title",
            Author = "Some Author",
            Isbn = "9780306406157",
            Price = 10.00m,
            Genre = ""
        };

        var response = await _client.PostAsJsonAsync("/api/books", book);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_WithZeroPrice_Returns400()
    {
        var book = new Book
        {
            Title = "Some Title",
            Author = "Some Author",
            Isbn = "9780306406157",
            Price = 0m,
            Genre = "Fiction"
        };

        var response = await _client.PostAsJsonAsync("/api/books", book);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_WithIsbnContainingLetters_Returns400()
    {
        var book = new Book
        {
            Title = "Some Title",
            Author = "Some Author",
            Isbn = "978030640615X",
            Price = 10.00m,
            Genre = "Fiction"
        };

        var response = await _client.PostAsJsonAsync("/api/books", book);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_WithIsbn12Digits_Returns400()
    {
        var book = new Book
        {
            Title = "Some Title",
            Author = "Some Author",
            Isbn = "978030640615",
            Price = 10.00m,
            Genre = "Fiction"
        };

        var response = await _client.PostAsJsonAsync("/api/books", book);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_WithIsbn14Digits_Returns400()
    {
        var book = new Book
        {
            Title = "Some Title",
            Author = "Some Author",
            Isbn = "97803064061571",
            Price = 10.00m,
            Genre = "Fiction"
        };

        var response = await _client.PostAsJsonAsync("/api/books", book);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_WithTitleExceedingMaxLength_Returns400()
    {
        var book = new Book
        {
            Title = new string('A', 201),
            Author = "Some Author",
            Isbn = "9780306406157",
            Price = 10.00m,
            Genre = "Fiction"
        };

        var response = await _client.PostAsJsonAsync("/api/books", book);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_WithAuthorExceedingMaxLength_Returns400()
    {
        var book = new Book
        {
            Title = "Some Title",
            Author = new string('A', 151),
            Isbn = "9780306406157",
            Price = 10.00m,
            Genre = "Fiction"
        };

        var response = await _client.PostAsJsonAsync("/api/books", book);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_WithGenreExceedingMaxLength_Returns400()
    {
        var book = new Book
        {
            Title = "Some Title",
            Author = "Some Author",
            Isbn = "9780306406157",
            Price = 10.00m,
            Genre = new string('A', 51)
        };

        var response = await _client.PostAsJsonAsync("/api/books", book);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_WithIdMismatch_Returns400()
    {
        var postResponse = await _client.PostAsJsonAsync("/api/books", CreateValidBook());
        var created = await postResponse.Content.ReadFromJsonAsync<Book>();

        var updated = CreateValidBook();
        updated.Id = created!.Id + 100;

        var response = await _client.PutAsJsonAsync($"/api/books/{created.Id}", updated);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAll_AfterPostingMultipleBooks_ReturnsAll()
    {
        // Post multiple books
        var response1 = await _client.PostAsJsonAsync("/api/books", CreateValidBook());
        var response2 = await _client.PostAsJsonAsync("/api/books", CreateValidBook());
        response1.StatusCode.Should().Be(HttpStatusCode.Created);
        response2.StatusCode.Should().Be(HttpStatusCode.Created);

        var response = await _client.GetAsync("/api/books");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var books = await response.Content.ReadFromJsonAsync<List<Book>>();
        books.Should().NotBeNull();
        books!.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task Delete_ThenGet_Returns404()
    {
        var postResponse = await _client.PostAsJsonAsync("/api/books", CreateValidBook());
        var created = await postResponse.Content.ReadFromJsonAsync<Book>();

        await _client.DeleteAsync($"/api/books/{created!.Id}");

        var getResponse = await _client.GetAsync($"/api/books/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_CreatedBookHasLocationHeader()
    {
        var response = await _client.PostAsJsonAsync("/api/books", CreateValidBook());

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain("/api/books/");
    }

    [Fact]
    public async Task Put_WithZeroPrice_Returns400()
    {
        var postResponse = await _client.PostAsJsonAsync("/api/books", CreateValidBook());
        var created = await postResponse.Content.ReadFromJsonAsync<Book>();

        var updated = new Book
        {
            Id = created!.Id,
            Title = "Title",
            Author = "Author",
            Isbn = "9780306406157",
            Price = 0m,
            Genre = "Fiction"
        };

        var response = await _client.PutAsJsonAsync($"/api/books/{created.Id}", updated);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task FullCrudCycle_WorksEndToEnd()
    {
        // Create
        var book = CreateValidBook();
        var postResponse = await _client.PostAsJsonAsync("/api/books", book);
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await postResponse.Content.ReadFromJsonAsync<Book>();
        created.Should().NotBeNull();
        var id = created!.Id;

        // Read
        var getResponse = await _client.GetAsync($"/api/books/{id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetched = await getResponse.Content.ReadFromJsonAsync<Book>();
        fetched!.Title.Should().Be(book.Title);

        // Update
        var updated = CreateValidBook();
        updated.Id = id;
        updated.Title = "Updated CRUD Title";
        var putResponse = await _client.PutAsJsonAsync($"/api/books/{id}", updated);
        putResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify update
        var getUpdated = await _client.GetAsync($"/api/books/{id}");
        var updatedBook = await getUpdated.Content.ReadFromJsonAsync<Book>();
        updatedBook!.Title.Should().Be("Updated CRUD Title");

        // Delete
        var deleteResponse = await _client.DeleteAsync($"/api/books/{id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion
        var getDeleted = await _client.GetAsync($"/api/books/{id}");
        getDeleted.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Put_WithMissingAuthor_Returns400()
    {
        var postResponse = await _client.PostAsJsonAsync("/api/books", CreateValidBook());
        var created = await postResponse.Content.ReadFromJsonAsync<Book>();

        var updated = new Book
        {
            Id = created!.Id,
            Title = "Title",
            Author = "",
            Isbn = "9780306406157",
            Price = 10.00m,
            Genre = "Fiction"
        };

        var response = await _client.PutAsJsonAsync($"/api/books/{created.Id}", updated);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_WithMissingGenre_Returns400()
    {
        var postResponse = await _client.PostAsJsonAsync("/api/books", CreateValidBook());
        var created = await postResponse.Content.ReadFromJsonAsync<Book>();

        var updated = new Book
        {
            Id = created!.Id,
            Title = "Title",
            Author = "Author",
            Isbn = "9780306406157",
            Price = 10.00m,
            Genre = ""
        };

        var response = await _client.PutAsJsonAsync($"/api/books/{created.Id}", updated);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_WithTitleExceedingMaxLength_Returns400()
    {
        var postResponse = await _client.PostAsJsonAsync("/api/books", CreateValidBook());
        var created = await postResponse.Content.ReadFromJsonAsync<Book>();

        var updated = new Book
        {
            Id = created!.Id,
            Title = new string('A', 201),
            Author = "Author",
            Isbn = "9780306406157",
            Price = 10.00m,
            Genre = "Fiction"
        };

        var response = await _client.PutAsJsonAsync($"/api/books/{created.Id}", updated);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_WithAuthorExceedingMaxLength_Returns400()
    {
        var postResponse = await _client.PostAsJsonAsync("/api/books", CreateValidBook());
        var created = await postResponse.Content.ReadFromJsonAsync<Book>();

        var updated = new Book
        {
            Id = created!.Id,
            Title = "Title",
            Author = new string('A', 151),
            Isbn = "9780306406157",
            Price = 10.00m,
            Genre = "Fiction"
        };

        var response = await _client.PutAsJsonAsync($"/api/books/{created.Id}", updated);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_WithGenreExceedingMaxLength_Returns400()
    {
        var postResponse = await _client.PostAsJsonAsync("/api/books", CreateValidBook());
        var created = await postResponse.Content.ReadFromJsonAsync<Book>();

        var updated = new Book
        {
            Id = created!.Id,
            Title = "Title",
            Author = "Author",
            Isbn = "9780306406157",
            Price = 10.00m,
            Genre = new string('A', 51)
        };

        var response = await _client.PutAsJsonAsync($"/api/books/{created.Id}", updated);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_WithIsbnContainingLetters_Returns400()
    {
        var postResponse = await _client.PostAsJsonAsync("/api/books", CreateValidBook());
        var created = await postResponse.Content.ReadFromJsonAsync<Book>();

        var updated = new Book
        {
            Id = created!.Id,
            Title = "Title",
            Author = "Author",
            Isbn = "978030640615X",
            Price = 10.00m,
            Genre = "Fiction"
        };

        var response = await _client.PutAsJsonAsync($"/api/books/{created.Id}", updated);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_Twice_SecondReturns404()
    {
        var postResponse = await _client.PostAsJsonAsync("/api/books", CreateValidBook());
        var created = await postResponse.Content.ReadFromJsonAsync<Book>();

        var first = await _client.DeleteAsync($"/api/books/{created!.Id}");
        first.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var second = await _client.DeleteAsync($"/api/books/{created.Id}");
        second.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ResponseContainsAllFieldsCorrectly()
    {
        var book = new Book
        {
            Title = "Specific Title",
            Author = "Specific Author",
            Isbn = "9781234567890",
            Price = 42.50m,
            Genre = "Mystery"
        };

        var response = await _client.PostAsJsonAsync("/api/books", book);
        var created = await response.Content.ReadFromJsonAsync<Book>();

        created.Should().NotBeNull();
        created!.Id.Should().BeGreaterThan(0);
        created.Title.Should().Be("Specific Title");
        created.Author.Should().Be("Specific Author");
        created.Isbn.Should().Be("9781234567890");
        created.Price.Should().Be(42.50m);
        created.Genre.Should().Be("Mystery");
    }

    [Fact]
    public async Task Get_ReturnsJsonContentType()
    {
        var response = await _client.GetAsync("/api/books");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
    }
}
