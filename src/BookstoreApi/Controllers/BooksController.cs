using BookstoreApi.Models;
using BookstoreApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreApi.Controllers;

/// <summary>
/// API controller for managing books in the bookstore.
/// </summary>
[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    /// <summary>
    /// Initializes a new instance of the <see cref="BooksController"/> class.
    /// </summary>
    /// <param name="bookService">The book service.</param>
    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    /// <summary>
    /// Retrieves all books.
    /// </summary>
    /// <returns>A list of all books.</returns>
    [HttpGet]
    public ActionResult<IEnumerable<Book>> GetAll()
    {
        return Ok(_bookService.GetAll());
    }

    /// <summary>
    /// Retrieves a book by its identifier.
    /// </summary>
    /// <param name="id">The book identifier.</param>
    /// <returns>The book if found; otherwise, 404.</returns>
    [HttpGet("{id}")]
    public ActionResult<Book> GetById(int id)
    {
        var book = _bookService.GetById(id);
        if (book is null)
            return NotFound();

        return Ok(book);
    }

    /// <summary>
    /// Creates a new book.
    /// </summary>
    /// <param name="book">The book to create.</param>
    /// <returns>The created book with a Location header.</returns>
    [HttpPost]
    public ActionResult<Book> Create(Book book)
    {
        var created = _bookService.Add(book);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Updates an existing book.
    /// </summary>
    /// <param name="id">The book identifier.</param>
    /// <param name="book">The updated book data.</param>
    /// <returns>204 on success; 400 if IDs mismatch; 404 if not found.</returns>
    [HttpPut("{id}")]
    public IActionResult Update(int id, Book book)
    {
        if (book.Id != 0 && book.Id != id)
            return BadRequest();

        if (!_bookService.Update(id, book))
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Deletes a book by its identifier.
    /// </summary>
    /// <param name="id">The book identifier.</param>
    /// <returns>204 on success; 404 if not found.</returns>
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (!_bookService.Delete(id))
            return NotFound();

        return NoContent();
    }
}
