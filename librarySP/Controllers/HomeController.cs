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
using System.Collections;
using System.Collections.Generic;
using Parser.Jobs;
using ExtensionMethods.IQueryableExtensions;

namespace librarySP.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IReportService _reportService;
        private readonly ILogger<ParserBooks> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IParserBooks _parserBooks;
        private IQueryable<BookViewModel> bookModels;


        public HomeController(IBookService bookService, ILogger<ParserBooks> logger,
            IReportService reportService, IHttpClientFactory httpClientFactory, IParserBooks parserBooks)
        {
            _bookService = bookService;
            _reportService = reportService;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _parserBooks = parserBooks;
        }


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