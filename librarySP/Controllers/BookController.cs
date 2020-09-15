using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using librarySP.Database;
using librarySP.Database.Entities;
using librarySP.Database.Repositories.Interfaces;
using librarySP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace librarySP.Controllers
{
    public class BookController : Controller
    {
        private IBookRepository bookRepository;
        const string librarian = "Библиотекарь";
        const string user = "Пользователь";

        public BookController(IBookRepository _bookRepository)
        {
            bookRepository = _bookRepository;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View(bookRepository.GetBooks());
        }

        [Authorize(Roles = librarian)]
        public IActionResult CreateBook()
        {
            return View();
        }

        [Authorize(Roles = librarian)]
        [HttpPost]
        public IActionResult CreateBook(Book book)
        {
            bookRepository.CreateBook(book);

            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult DetailsBook(int? id)
        {
            if (id != null)
            {
                bookRepository.DetailsBook(id);
                if (id != null) return View(id);
            }
            return NotFound();
        }

        [Authorize(Roles = librarian)]
        public IActionResult EditBook(int? id)
        {
            if (id != null)
            {
                bookRepository.DetailsBook(id);
                if (id != null)
                    return View(id);
            }
            return NotFound();
        }

        [Authorize(Roles = librarian)]
        [HttpPost]
        public IActionResult EditBook(Book book)
        {
            bookRepository.EditBook(book);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = librarian)]
        [HttpGet]
        [ActionName("DeleteBook")]
        public IActionResult ConfirmDelete(int? id)
        {
            if (id != null)
            {
                bookRepository.ConfirmDelete(id);
                if (id != null)
                    return View(id);
            }
            return NotFound();
        }

        [Authorize(Roles = librarian)]
        [HttpPost]
        public IActionResult DeleteBook(int? id)
        {
     
                if (id != null)
                {
                    bookRepository.DeleteBook(id);
                    return RedirectToAction("Index");
                }

            return NotFound();
        }


    }
}
