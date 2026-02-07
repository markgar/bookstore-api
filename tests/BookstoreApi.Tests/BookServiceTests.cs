using BookstoreApi.Models;
using BookstoreApi.Services;
using FluentAssertions;

namespace BookstoreApi.Tests;

public class BookServiceTests
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
    public void Add_ReturnsBookWithGeneratedId()
    {
        var book = CreateValidBook();

        var result = _service.Add(book);

        result.Id.Should().BeGreaterThan(0);
        result.Title.Should().Be("Test Book");
    }

    [Fact]
    public void GetAll_ReturnsAllAddedBooks()
    {
        _service.Add(CreateValidBook());
        _service.Add(CreateValidBook());

        var result = _service.GetAll();

        result.Should().HaveCount(2);
    }

    [Fact]
    public void GetById_WithValidId_ReturnsCorrectBook()
    {
        var added = _service.Add(CreateValidBook());

        var result = _service.GetById(added.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(added.Id);
        result.Title.Should().Be(added.Title);
    }

    [Fact]
    public void GetById_WithNonExistentId_ReturnsNull()
    {
        var result = _service.GetById(999);

        result.Should().BeNull();
    }

    [Fact]
    public void Update_ExistingBook_ReturnsTrueAndPersistsChanges()
    {
        var added = _service.Add(CreateValidBook());
        var updated = CreateValidBook();
        updated.Title = "Updated Title";

        var result = _service.Update(added.Id, updated);

        result.Should().BeTrue();
        var fetched = _service.GetById(added.Id);
        fetched!.Title.Should().Be("Updated Title");
    }

    [Fact]
    public void Update_NonExistentBook_ReturnsFalse()
    {
        var result = _service.Update(999, CreateValidBook());

        result.Should().BeFalse();
    }

    [Fact]
    public void Delete_ExistingBook_ReturnsTrueAndRemovesBook()
    {
        var added = _service.Add(CreateValidBook());

        var result = _service.Delete(added.Id);

        result.Should().BeTrue();
        _service.GetById(added.Id).Should().BeNull();
    }

    [Fact]
    public void Delete_NonExistentBook_ReturnsFalse()
    {
        var result = _service.Delete(999);

        result.Should().BeFalse();
    }
}
