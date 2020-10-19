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

namespace librarySP.Controllers
{
    public class HomeController : Controller
    {

        private readonly IBookService _bookService;


        public HomeController(IBookService bookService)
        {
            _bookService = bookService;
        }

        public IActionResult Index(string searchString, int search, string sortBook)
        {
            var book = _bookService.GetBooks();
            if (!string.IsNullOrEmpty(sortBook))
            {
                var bookSorter = _bookService.SortBooks(sortBook);
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