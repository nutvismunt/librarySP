﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using librarySP.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using librarySP.Database.Interfaces;
using librarySP.Database.Entities;
using Microsoft.AspNetCore.Authorization;

namespace librarySP.Controllers
{
    public class HomeController : Controller
    {

        private readonly IRepository<Book> _dbB;
        private readonly ILogger<HomeController> _logger;


        public HomeController(IRepository<Book> bookRep, ILogger<HomeController> logger)
        {
            _logger = logger;
            _dbB = bookRep;
        }

        public async Task <IActionResult> Index(string searchString, int search, string sortOrder)
        {
            var book = from b in _dbB.GetItems() select b;

            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
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

            if (!String.IsNullOrEmpty(searchString))
            {
                switch (search)
                {
                    case 0:
                        book = book.Where(s => s.BookName.ToLower().Contains(searchString.ToLower()));
                        break;
                    case 1:
                        book = book.Where(s => s.BookDescription.ToLower().Contains(searchString.ToLower()));
                        break;
                    case 2:
                        book = book.Where(s => s.BookGenre.ToLower().Contains(searchString.ToLower()));
                        break;
                    case 3:
                        book = book.Where(s => s.BookYear.ToLower().Contains(searchString.ToLower()));
                        break;
                    case 4:
                        book = book.Where(s => s.BookAuthor.ToLower().Contains(searchString.ToLower()));
                        break;
                    case 5:
                        book = book.Where(s => s.BookPublisher.ToLower().Contains(searchString.ToLower()));
                        break;
                }

            }
            return View(await book.ToListAsync());
        }


 

        [Authorize]
        public IActionResult DetailsBook(long id)
        {
            if (id>0)
            {
                Book book = _dbB.GetItem(id);
                if (book != null)
                    return View(book);
            }
            return NotFound();
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}