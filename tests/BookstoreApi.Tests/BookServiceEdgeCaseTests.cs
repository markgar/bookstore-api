using BookstoreApi.Models;
using BookstoreApi.Services;
using FluentAssertions;

namespace BookstoreApi.Tests;

public class BookServiceEdgeCaseTests
{
    private readonly BookService _service = new();

    private static Book CreateValidBook() => new()
    {
        Title = "Test Book",
        Author = "Test Author",
        Isbn = "9781234567890",
        Price = 19.99m,
        Genre = "Fiction"
    };

    [Fact]
    public void GetAll_WhenEmpty_ReturnsEmptyCollection()
    {
        var result = _service.GetAll();

        result.Should().BeEmpty();
    }

    [Fact]
    public void Add_MultipleBooksGetUniqueSequentialIds()
    {
        var book1 = _service.Add(CreateValidBook());
        var book2 = _service.Add(CreateValidBook());
        var book3 = _service.Add(CreateValidBook());

        book1.Id.Should().Be(1);
        book2.Id.Should().Be(2);
        book3.Id.Should().Be(3);
    }

    [Fact]
    public void Add_SetsIdOnPassedInBookObject()
    {
        var book = CreateValidBook();
        book.Id.Should().Be(0);

        var result = _service.Add(book);

        book.Id.Should().Be(result.Id);
        ReferenceEquals(book, result).Should().BeTrue();
    }

    [Fact]
    public void Update_PreservesIdOnBook()
    {
        var added = _service.Add(CreateValidBook());
        var updated = CreateValidBook();
        updated.Title = "New Title";

        _service.Update(added.Id, updated);

        var fetched = _service.GetById(added.Id);
        fetched!.Id.Should().Be(added.Id);
    }

    [Fact]
    public void Update_AllFieldsPersist()
    {
        var added = _service.Add(CreateValidBook());
        var updated = new Book
        {
            Title = "New Title",
            Author = "New Author",
            Isbn = "9789876543210",
            Price = 49.99m,
            Genre = "Non-Fiction"
        };

        _service.Update(added.Id, updated);

        var fetched = _service.GetById(added.Id);
        fetched!.Title.Should().Be("New Title");
        fetched.Author.Should().Be("New Author");
        fetched.Isbn.Should().Be("9789876543210");
        fetched.Price.Should().Be(49.99m);
        fetched.Genre.Should().Be("Non-Fiction");
    }

    [Fact]
    public void Delete_ThenGetAll_DoesNotIncludeDeletedBook()
    {
        var book1 = _service.Add(CreateValidBook());
        var book2 = _service.Add(CreateValidBook());

        _service.Delete(book1.Id);

        var all = _service.GetAll().ToList();
        all.Should().HaveCount(1);
        all[0].Id.Should().Be(book2.Id);
    }

    [Fact]
    public void Delete_SameBookTwice_SecondCallReturnsFalse()
    {
        var added = _service.Add(CreateValidBook());

        _service.Delete(added.Id).Should().BeTrue();
        _service.Delete(added.Id).Should().BeFalse();
    }

    [Fact]
    public void GetById_AfterUpdate_ReturnsUpdatedData()
    {
        var added = _service.Add(CreateValidBook());
        var updated = CreateValidBook();
        updated.Title = "Changed";

        _service.Update(added.Id, updated);
        var fetched = _service.GetById(added.Id);

        fetched!.Title.Should().Be("Changed");
    }
}
