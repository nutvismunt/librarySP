using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using BusinessLayer.Models;
using BusinessLayer.Interfaces;
using BusinessLayer.Models.BookDTO;
using System.Linq;
using System;
using Microsoft.Extensions.Logging;
using BusinessLayer.Parser;

namespace librarySP.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ILogger<Parser> _logger;

        public HomeController(IBookService bookService, ILogger<Parser> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }



        public async Task<IActionResult> Index(string searchString, int search,
                                               string sortBook, string boolSort)
        {
        //    Parser parser = new Parser(_logger);
          //  await parser.ParseAsync(picDownload);
            
            ViewBag.NameSort = boolSort == "false" ? "true" : "false";
            ViewBag.AuthorSort = boolSort == "false" ? "true" : "false";
            if (!string.IsNullOrEmpty(ViewBag.NameSort))
            {
                sortBook = "BookName";
            }
             if(!string.IsNullOrEmpty(ViewBag.AuthorSort))
            {
                sortBook = "BookAuthor";
            }

            var b = Convert.ToBoolean(boolSort);
            var book = _bookService.GetBooks().Where(c=>c.BookInStock>0);
            if (!string.IsNullOrEmpty(boolSort))
            {
                var bookSorter = _bookService.SortBooks(sortBook,b);
                if (bookSorter != null)
                    return View(bookSorter);
            }

            else if (!string.IsNullOrEmpty(searchString))
            {
                var bookSearcher = _bookService.SearchBook(searchString);
                if (bookSearcher != null)
                    return View(bookSearcher);

            }

            return View(book.ToList());
        }

        [Authorize]
        public IActionResult DetailsBook(long id)
        {
            var book =_bookService.GetBook(id);
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