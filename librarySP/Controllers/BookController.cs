using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Models.BookDTO;
using BusinessLayer.Interfaces;
using System.Linq;
using System;
using Parser.Parser;
using System.Collections.Generic;
using ExtensionMethods.IQueryableExtensions;
using Microsoft.Extensions.DependencyInjection;

namespace librarySP.Controllers
{
    public class BookController : Controller
    {
        private readonly IServiceProvider _provider;
        private readonly IBookService _bookService;
        private readonly IParserBook _parserBook;
        private readonly ILabirintBook _labirintBook;
        private IQueryable<BookViewModel> bookModels;
        const string librarian = "Библиотекарь";

        public BookController(IServiceProvider serviceProvider, IBookService bookService, IParserBook parserBook, ILabirintBook labirintBook)
        {
            _provider = serviceProvider;
            _bookService = bookService;
            _parserBook = parserBook;
            _labirintBook = labirintBook;
        }

        [Authorize(Roles = librarian)]
        public IActionResult Index(string searchString, string sortBook, string boolSort, List<long> id)
        {
            List<BookViewModel> list = new List<BookViewModel>();
            if (id.Any())
            {
                foreach (var item in id)
                {
                    var bookList = _bookService.GetBook(item);
                    list.Add(bookList);
                }
                bookModels = list.AsQueryable();
            }
            if (bookModels == null)
                bookModels = _bookService.GetBooks().Where(c => c.BookInStock > 0);
            //меняется значение в зависимости от нажатия на заголовок таблицы
            ViewBag.NameSort = boolSort == "NameFalse" ? "NameTrue" : "NameFalse";
            ViewBag.AuthorSort = boolSort == "AuthorFalse" ? "AuthorTrue" : "AuthorFalse";
            // true или false для asc/desc сортировки
            switch (boolSort)
            {
                case "NameFalse":
                    sortBook = "BookName"; boolSort = "false";
                    break;
                case "NameTrue":
                    sortBook = "BookName"; boolSort = "true";
                    break;
                case "AuthorFalse":
                    sortBook = "BookAuthor"; boolSort = "false";
                    break;
                case "AuthorTrue":
                    sortBook = "BookAuthor"; boolSort = "true";
                    break;
            }
            //перевод в булево для отправки в метод;
            var b = Convert.ToBoolean(boolSort);
            if (!string.IsNullOrEmpty(searchString))
            {
                //поиск
                bookModels = bookModels.Search(searchString).AsQueryable();
                if (bookModels != null)
                    return View(bookModels);
            }
            if (!string.IsNullOrEmpty(sortBook))
            {
                //сортировка
                bookModels = bookModels.SortedItems(sortBook, b);
                if (bookModels != null)
                    return View(bookModels);
            }

            return View(bookModels);
        }

        [Authorize(Roles = librarian)]
        public IActionResult CreateBook() => View();

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
                var bookModel = book;
                bookModel.BookPicName = uploadedFile.FileName;
                bookModel.BookPicPath = path;
                _bookService.Create(bookModel);
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = librarian)]
        public IActionResult DetailsBook(long id)
        {
            var book = _bookService.GetBook(id);
            if (book != null) return View(book);
            else 
                return NotFound();
        }

        [Authorize(Roles = librarian)]
        public IActionResult EditBook(long id)
        {
            var book = _bookService.GetBook(id);
            if (book == null) return NotFound();
            return View(book);
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
                    book = bookViewModel;
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
            if (book != null) return View(book);
            else
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
        public IActionResult AddBookFromUrl() => View();

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
        public IActionResult ParserSettings() => View();

        [Authorize(Roles = librarian)]
        public IActionResult ParserSettings(int amount)
        {
            var settings = _labirintBook.GetParseSettings();
            settings.BookAmount = amount;
            _labirintBook.UpdateSettings(settings);
            return View();
        }

        public async Task<IActionResult> StartParser()
        {
            using (var scope = _provider.CreateScope())
            {
                var bookService = scope.ServiceProvider.GetService<IBookService>();
                var parse = scope.ServiceProvider.GetService<IParserBooks>();
                var labId = scope.ServiceProvider.GetService<ILabirintBook>();
                var pic = new UrlPicDownload();
                //полчение количества книг, которые нужно добавить
                var amount = labId.GetParseSettings().BookAmount;
                var str = "";
                //id книги
                var lastISBN = labId.GetBookUrl();
                for (var i = 0; i < amount; i++)
                {
                    str = await parse.ParseBooksAsync(pic, amount,lastISBN);
                    //+1 выполнение цикла, если книга не найдена
                    if (str == "такой книги нет или она уже добавлена") amount++;
                    lastISBN--;
                }
                // обновить url последней книги в бд
                labId.Update(str);
            }
            return RedirectToAction("ParserSettings");
        }
    }
}
