using BookstoreApi.Models;

namespace BookstoreApi.Services;

/// <summary>
/// Defines operations for managing books in the bookstore.
/// </summary>
public interface IBookService
{
    /// <summary>
    /// Retrieves all books in the store.
    /// </summary>
    /// <returns>A collection of all books.</returns>
    IEnumerable<Book> GetAll();

    /// <summary>
    /// Retrieves a book by its unique identifier.
    /// </summary>
    /// <param name="id">The book identifier.</param>
    /// <returns>The book if found; otherwise, <c>null</c>.</returns>
    Book? GetById(int id);

    /// <summary>
    /// Adds a new book to the store.
    /// </summary>
    /// <param name="book">The book to add.</param>
    /// <returns>The added book with its generated identifier.</returns>
    Book Add(Book book);

    /// <summary>
    /// Updates an existing book.
    /// </summary>
    /// <param name="id">The identifier of the book to update.</param>
    /// <param name="book">The updated book data.</param>
    /// <returns><c>true</c> if the book was found and updated; otherwise, <c>false</c>.</returns>
    bool Update(int id, Book book);

    /// <summary>
    /// Deletes a book by its unique identifier.
    /// </summary>
    /// <param name="id">The identifier of the book to delete.</param>
    /// <returns><c>true</c> if the book was found and deleted; otherwise, <c>false</c>.</returns>
    bool Delete(int id);
}
