using BookstoreApi.Models;
using BookstoreApi.Services;
using FluentAssertions;

namespace BookstoreApi.Tests;

public class BookServiceCoverageTests
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
    public void Update_WithZeroId_ReturnsFalse()
    {
        var result = _service.Update(0, CreateValidBook());
        result.Should().BeFalse();
    }

    [Fact]
    public void Update_WithNegativeId_ReturnsFalse()
    {
        var result = _service.Update(-1, CreateValidBook());
        result.Should().BeFalse();
    }

    [Fact]
    public void GetAll_ReturnsExactBooksAdded()
    {
        var book1 = CreateValidBook();
        book1.Title = "Book One";
        var book2 = CreateValidBook();
        book2.Title = "Book Two";

        var added1 = _service.Add(book1);
        var added2 = _service.Add(book2);

        var all = _service.GetAll().ToList();
        all.Should().HaveCount(2);
        all.Should().Contain(b => b.Title == "Book One" && b.Id == added1.Id);
        all.Should().Contain(b => b.Title == "Book Two" && b.Id == added2.Id);
    }

    [Fact]
    public void Add_ReturnsReferenceToSameBookObject()
    {
        var book = CreateValidBook();
        var result = _service.Add(book);
        ReferenceEquals(book, result).Should().BeTrue();
    }

    [Fact]
    public void Update_DoesNotChangeOtherBooks()
    {
        var book1 = _service.Add(CreateValidBook());
        var book2 = _service.Add(CreateValidBook());

        var updated = CreateValidBook();
        updated.Title = "Changed";
        _service.Update(book1.Id, updated);

        var fetched2 = _service.GetById(book2.Id);
        fetched2!.Title.Should().Be("Test Book");
    }

    [Fact]
    public void Delete_DoesNotAffectOtherBooks()
    {
        var book1 = _service.Add(CreateValidBook());
        var book2 = _service.Add(CreateValidBook());

        _service.Delete(book1.Id);

        var fetched2 = _service.GetById(book2.Id);
        fetched2.Should().NotBeNull();
        fetched2!.Id.Should().Be(book2.Id);
    }

    [Fact]
    public void Add_AfterDeletingAll_StillIncrementsId()
    {
        var book1 = _service.Add(CreateValidBook());
        _service.Delete(book1.Id);

        var book2 = _service.Add(CreateValidBook());
        book2.Id.Should().BeGreaterThan(book1.Id);
    }
}
