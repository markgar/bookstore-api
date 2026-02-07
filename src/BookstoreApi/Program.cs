using BookstoreApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<IBookService, BookService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapControllers();

app.Run();

/// <summary>
/// Partial class declaration to support WebApplicationFactory in integration tests.
/// </summary>
public partial class Program { }
