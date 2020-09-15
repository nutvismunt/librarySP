using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using librarySP.Models;
using librarySP.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using librarySP.Database.Dependencies;
using librarySP.Database.Repositories.Interfaces;

namespace librarySP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IBookRepository bookRepository;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public HomeController(IBookRepository _bookRepository)
        {
            bookRepository = _bookRepository;
        }

        public IActionResult Index()
        {
            return View(bookRepository.GetBooks());
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