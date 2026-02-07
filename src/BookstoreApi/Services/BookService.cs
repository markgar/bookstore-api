using System.Collections.Concurrent;
using BookstoreApi.Models;

namespace BookstoreApi.Services;

/// <summary>
/// In-memory implementation of <see cref="IBookService"/> using a thread-safe store.
/// </summary>
public class BookService : IBookService
{
    private readonly ConcurrentDictionary<int, Book> _books = new();
    private int _nextId;

    /// <inheritdoc />
    public IEnumerable<Book> GetAll() => _books.Values;

    /// <inheritdoc />
    public Book? GetById(int id) => _books.GetValueOrDefault(id);

    /// <inheritdoc />
    public Book Add(Book book)
    {
        book.Id = Interlocked.Increment(ref _nextId);
        _books[book.Id] = book;
        return book;
    }

    /// <inheritdoc />
    public bool Update(int id, Book book)
    {
        if (!_books.TryGetValue(id, out var existing))
            return false;

        book.Id = id;
        // Atomically replace the old value; retry if a concurrent update changed it.
        while (!_books.TryUpdate(id, book, existing))
        {
            if (!_books.TryGetValue(id, out existing))
                return false;
        }

        return true;
    }

    /// <inheritdoc />
    public bool Delete(int id) => _books.TryRemove(id, out _);
}
