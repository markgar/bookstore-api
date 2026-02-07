using System.ComponentModel.DataAnnotations;

namespace BookstoreApi.Models;

/// <summary>
/// Represents a book in the bookstore inventory.
/// </summary>
public class Book
{
    /// <summary>
    /// Gets or sets the unique identifier for the book.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the book.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the author of the book.
    /// </summary>
    [Required]
    [StringLength(150)]
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ISBN-13 of the book. Must be exactly 13 numeric digits.
    /// </summary>
    [Required]
    [RegularExpression(@"^\d{13}$", ErrorMessage = "ISBN must be exactly 13 digits.")]
    public string Isbn { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the price of the book. Must be greater than zero.
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the genre of the book.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Genre { get; set; } = string.Empty;
}
