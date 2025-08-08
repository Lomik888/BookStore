using System.Data;
using System.Data.SqlClient;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers;

public class BooksController : Controller
{
    [HttpGet]
    public IActionResult EnterId()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Books()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var book = await GetBookAsync(id);
        if (book == null)
            return NotFound();

        return View("EnterId", book);
    }

    [HttpGet]
    public async Task<IActionResult> GetBooks(int page, int pageSize)
    {
        ViewData["Page"] = page;
        ViewData["PageSize"] = pageSize;
        
        var books = await GetBooksAsync(page, pageSize);
        if (books == null)
            return NotFound();

        return View("Books", books);
    }

    private async Task<List<BookViewModel>?> GetBooksAsync(int page, int pageSize)
    {
        var books = new List<BookViewModel>();

        await using var conn =
            new SqlConnection(
                "Server=localhost,1433;Database=master;User Id=sa;Password=Root!1234;TrustServerCertificate=True;");
        await conn.OpenAsync();

        await using var cmd = new SqlCommand("get_pagination_books", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@page", page);
        cmd.Parameters.AddWithValue("@page_size", pageSize);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return null;

        while (await reader.ReadAsync())
        {
            books.Add(new BookViewModel
            {
                Title = reader.GetString(0),
                Author = reader.GetString(1),
                PublishedYear = reader.GetInt32(2)
            });
        }

        return books;
    }

    private async Task<BookViewModel?> GetBookAsync(int id)
    {
        await using var conn =
            new SqlConnection(
                "Server=localhost,1433;Database=master;User Id=sa;Password=Root!1234;TrustServerCertificate=True;");
        await conn.OpenAsync();

        await using var cmd = new SqlCommand("get_book", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@book_id", id);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return null;

        return new BookViewModel()
        {
            Title = reader.GetString(0),
            Author = reader.GetString(1),
            PublishedYear = reader.GetInt32(2)
        };
    }
}