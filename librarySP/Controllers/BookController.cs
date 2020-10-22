using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessLayer.Models.BookDTO;
using BusinessLayer.Interfaces;
using System.Linq;
using System;
using Microsoft.Extensions.Logging;
using BusinessLayer.Parser;

namespace librarySP.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        const string librarian = "Библиотекарь";
        private readonly ILogger<Parser> _logger;
        private IParser _parser;
        public BookController(IBookService bookService, ILogger<Parser> logger, IParser _parser)
        {
            _bookService = bookService;
            _logger = logger;
            this._parser = _parser;
        }


        [HttpGet]
        public IActionResult AddBookFromUrl()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBookFromUrl(UrlPicDownload picDownload, string iSBN,int bookAmount)
        {
            iSBN= new String (iSBN.Where(Char.IsDigit).ToArray());
            var isbn = long.Parse(iSBN);
            var book= await _parser.ParseAsync(picDownload, isbn);
            book.BookInStock = bookAmount;
            book.WhenAdded = DateTime.Now;
            _bookService.Update(book);

           return RedirectToAction("Index");
        }

        [Authorize(Roles = librarian)]
        public IActionResult Index(string searchString, int search, string sortBook, string boolSort)
        {
            var book = _bookService.GetBooks().Where(c => c.BookInStock > 0);
            
            ViewBag.NameSort = boolSort == "true" ? "false" : "true";
            ViewBag.AuthorSort = boolSort == "true" ? "false" : "true";
            if (!string.IsNullOrEmpty(ViewBag.NameSort))
            {
                sortBook = "BookName";
            }
            if (!string.IsNullOrEmpty(ViewBag.AuthorSort))
            {
                sortBook = "BookAuthor";
            }

            var b = Convert.ToBoolean(boolSort);

            if (!string.IsNullOrEmpty(sortBook))
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


        [Authorize(Roles = librarian)]
        public IActionResult CreateBook()
        {
            return View();
        }

        [Authorize(Roles = librarian)]
        [HttpPost]
        public async Task<IActionResult> CreateBook(BookViewModel book, IFormFile uploadedFile)
        {

            if (uploadedFile != null)
            {
                var path = "/Files/" + uploadedFile.FileName;

                using (var fileStream = new FileStream("wwwroot" + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                var bookModel = new BookViewModel { 
                    Id = book.Id, 
                    BookName = book.BookName, 
                    BookDescription = book.BookDescription, 
                    BookGenre = book.BookGenre, 
                    BookYear = book.BookYear, 
                    BookAuthor = book.BookAuthor, 
                    BookPublisher = book.BookPublisher, 
                    BookInStock = book.BookInStock, 
                    BookPicName = uploadedFile.FileName, 
                    BookPicPath = path,
                    ISBN=book.ISBN
                };

                _bookService.Create(bookModel);
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = librarian)]
        public IActionResult DetailsBook(long id)
        {
            var book = _bookService.GetBook(id);
            if (book != null)
                return View(book);
            return NotFound();
        }

        [Authorize(Roles = librarian)]
        public IActionResult EditBook(long id)
        {
            var book = _bookService.GetBook(id);
            if (book == null)
            {
                return NotFound();
            }
            var bookModel = new BookViewModel { 
                Id = book.Id, 
                BookName = book.BookName, 
                BookDescription = book.BookDescription, 
                BookGenre = book.BookGenre, 
                BookYear = book.BookYear, 
                BookAuthor = book.BookAuthor, 
                BookPublisher = book.BookPublisher, 
                BookInStock = book.BookInStock, 
                BookPicName = book.BookPicName, 
                BookPicPath = book.BookPicPath 
            };

            return View(bookModel);
        }

        [Authorize(Roles = librarian)]
        [HttpPost]
        public async Task<IActionResult> EditBook(BookViewModel bookViewModel, IFormFile uploadedFile)
        {
            if (ModelState.IsValid)
            {
                var book = _bookService.GetBook(bookViewModel.Id);
                if (book != null)
                {
                    book.Id = bookViewModel.Id;
                    book.BookName = bookViewModel.BookName;
                    book.BookDescription = bookViewModel.BookDescription;
                    book.BookGenre = bookViewModel.BookGenre;
                    book.BookYear = bookViewModel.BookYear;
                    book.BookAuthor = bookViewModel.BookAuthor;
                    book.BookPublisher = bookViewModel.BookPublisher;
                    book.BookInStock = bookViewModel.BookInStock;
                    var v = uploadedFile;
                    if (uploadedFile != null)
                    {


                        if (System.IO.File.Exists("wwwroot" + book.BookPicPath))
                        {
                            System.IO.File.Delete("wwwroot" + book.BookPicPath);
                        }
                        string path = "/Files/" + uploadedFile.FileName;

                        using (var fileStream = new FileStream("wwwroot" + path, FileMode.Create))
                        {
                            await uploadedFile.CopyToAsync(fileStream);

                        }
                        book.BookPicName = uploadedFile.FileName;
                        book.BookPicPath = path;
                    }

                    _bookService.Update(book);
                }

                else return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = librarian)]
        [HttpGet]
        [ActionName("DeleteBook")]
        public IActionResult ConfirmDelete(long id)
        {
            var book = _bookService.GetBook(id);
            if (book != null)
            {
                if (book != null)
                    return View(book);
            }
            return NotFound();
        }

        [Authorize(Roles = librarian)]
        [HttpPost]
        public IActionResult DeleteBook(long id)
        {
            var book = _bookService.GetBook(id);
            if (book!=null)
            {
                if (System.IO.File.Exists("wwwroot" + book.BookPicPath))
                {
                    System.IO.File.Delete("wwwroot" + book.BookPicPath);
                }
                _bookService.Delete(book);
                return RedirectToAction("Index");
            }
            return NotFound();
        }

    }
}
