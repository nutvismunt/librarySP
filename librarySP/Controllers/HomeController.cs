using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using BusinessLayer.Models;
using BusinessLayer.Interfaces;
using System.Linq;
using System;
using Parser;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Parser.Parser;
using AutoMapper.QueryableExtensions;
using BusinessLayer.Models.BookDTO;

namespace librarySP.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IReportService _reportService;
        private readonly ILogger<ParserBooks> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IParserBooks _parserBooks;


        public HomeController(IBookService bookService, ILogger<ParserBooks> logger,
            IReportService reportService, IHttpClientFactory httpClientFactory, IParserBooks parserBooks)
        {
            _bookService = bookService;
            _reportService = reportService;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _parserBooks = parserBooks;
        }

        public IActionResult Index(string searchString, string sortBook, string boolSort)
        {
            //меняется значение в зависимости от нажатия на заголовок таблицы
            ViewBag.NameSort = boolSort == "NameFalse" ? "NameTrue" : "NameFalse";
            ViewBag.AuthorSort = boolSort == "AuthorFalse" ? "AuthorTrue" : "AuthorFalse";
            // true или false для asc/desc сортировки
            switch (boolSort)
            {
                case "NameFalse": sortBook = "BookName"; boolSort = "false"; 
                    break;
                case "NameTrue": sortBook = "BookName"; boolSort = "true"; 
                    break;
                case "AuthorFalse": sortBook = "BookAuthor"; boolSort = "false";
                    break;
                case "AuthorTrue": sortBook = "BookAuthor"; boolSort = "true"; 
                    break;
            }
            //перевод в булево для отправки в метод
            var b = Convert.ToBoolean(boolSort);
            var book = _bookService.GetBooks().Where(c => c.BookInStock > 0);
            if (!string.IsNullOrEmpty(sortBook))
            {
                //сортировка
                var bookSorter = _bookService.SortBooks(sortBook, b);
                if (bookSorter != null)
                    return View(bookSorter.AsNoTracking());
            }
            else if (!string.IsNullOrEmpty(searchString))
            {
                //поиск
                var bookSearcher = _bookService.SearchBook(searchString);
                if (bookSearcher != null)
                    return View(bookSearcher);
            }
            return View(book);
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