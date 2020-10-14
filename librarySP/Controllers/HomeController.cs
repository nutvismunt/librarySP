using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using DataLayer.Entities;
using BusinessLayer.Models;
using DataLayer.Interfaces;

namespace librarySP.Controllers
{
    public class HomeController : Controller
    {

        private readonly IRepository<Book> _dbB;
        private readonly ILogger<HomeController> _logger;
        private readonly ISearchItem<Book> _searchBook;


        public HomeController(IRepository<Book> bookRep, ILogger<HomeController> logger, ISearchItem<Book> searchBook)
        {
            _logger = logger;
            _dbB = bookRep;
            _searchBook = searchBook;
        }

        public async Task<IActionResult> Index(string searchString, int search, string sortOrder)
        {
            var book = from b in _dbB.GetItems() select b;

            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["AuthorSortParm"] = sortOrder == "Author" ? "author_desc" : "Author";
            switch (sortOrder)
            {
                case "name_desc":
                    book = book.OrderByDescending(s => s.BookName);
                    break;
                case "Author":
                    book = book.OrderBy(s => s.BookAuthor);
                    break;
                case "author_desc":
                    book = book.OrderByDescending(s => s.BookAuthor);
                    break;
                default:
                    book = book.OrderBy(s => s.BookName);
                    break;
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                var bookSearcher = _searchBook.Search(searchString);
                if (bookSearcher != null)
                    return View(bookSearcher);
            }
            else
                return View(await book.ToListAsync());
            return View();
        }

        [Authorize]
        public IActionResult DetailsBook(long id)
        {
            Book book = _dbB.GetItem(id);
                if (book != null)
                    return View(book);
            return NotFound();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}