using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Models.BookDTO;
using BusinessLayer.Interfaces;
using System.Linq;
using System;
using BusinessLayer.Parser;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.Impl.Triggers;
using Microsoft.Extensions.Options;
using BusinessLayer.Services;
using System.Collections.ObjectModel;
using BusinessLayer.Models.JobDTO;
using static Quartz.MisfireInstruction;
using Microsoft.Extensions.Hosting;
using BusinessLayer.Services.Jobs;

namespace librarySP.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        const string librarian = "Библиотекарь";
        private readonly IParserBook _parserBook;
        private readonly ILabirintBook _labirintBook;

        public BookController(IBookService bookService, IParserBook parserBook,
            ILabirintBook labirintBook)
        {
            _bookService = bookService;
            _parserBook = parserBook;
            _labirintBook = labirintBook;

        }


        [Authorize(Roles = librarian)]
        public IActionResult Index(string searchString,string sortBook, string boolSort)
        {
            //меняется значение в зависимости от нажатия на заголовок таблицы
            ViewBag.NameSort = boolSort == "NameFalse" ? "NameTrue" : "NameFalse";
            ViewBag.AuthorSort = boolSort == "AuthorFalse" ? "AuthorTrue" : "AuthorFalse";
            // true или false для asc/desc сортировки
            switch (boolSort)
            {
                case "NameFalse":
                    sortBook = "BookName";
                    boolSort = "false";
                    break;
                case "NameTrue":
                    sortBook = "BookName";
                    boolSort = "true";
                    break;
                case "AuthorFalse":
                    sortBook = "BookAuthor";
                    boolSort = "false";
                    break;
                case "AuthorTrue":
                    sortBook = "BookAuthor";
                    boolSort = "true";
                    break;
            }
            //перевод в булево для отправки в метод
            var b = Convert.ToBoolean(boolSort);
            var book = _bookService.GetBooks().Where(c => c.BookInStock > 0);
            if (!string.IsNullOrEmpty(sortBook))
            {//сортировка
                var bookSorter = _bookService.SortBooks(sortBook, b);
                if (bookSorter != null)
                    return View(bookSorter.AsNoTracking().ToList());
            }
            else if (!string.IsNullOrEmpty(searchString))
            {//поиск
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
                    if (uploadedFile != null)
                    { // удаление старого изображения, если добавлено новое
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
                if (book != null) return View(book);
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
                // удаление изображения книги
                if (System.IO.File.Exists("wwwroot" + book.BookPicPath))
                {
                    System.IO.File.Delete("wwwroot" + book.BookPicPath);
                }
                _bookService.Delete(book);
                return RedirectToAction("Index");
            }
            return NotFound();
        }


        [Authorize(Roles = librarian)]
        [HttpGet]
        public IActionResult AddBookFromUrl()
        {
            return View();
        }

        [Authorize(Roles = librarian)]
        [HttpPost]
        public async Task<IActionResult> AddBookFromUrl(UrlPicDownload picDownload, string iSBN, int bookAmount)
        {
            iSBN = new String(iSBN.Where(Char.IsDigit).ToArray());
            var isbn = long.Parse(iSBN);
            var book = await _parserBook.ParseBookAsync(picDownload, isbn);
            book.BookInStock = bookAmount;
            book.WhenAdded = DateTime.Now;
            _bookService.Update(book);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ParserSettings()
        {

            return View();
        }

        [Authorize(Roles = librarian)]
        [HttpPost]
        public IActionResult ParserSettings(int amount)
        {
            var settings = _labirintBook.GetParseSettings();
            settings.BookAmount = amount;
            _labirintBook.UpdateSettings(settings);

            return View();
        }


    }
}
