using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using DataLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Controllers
{
    public class BookController : Controller
    {

        private readonly IRepository<Order> _dbO;
        private readonly IRepository<Book> _dbB;
        private readonly IRepository<User> _dbU;
        private readonly IUnitOfWork _unitOfWork;
        UserManager<User> _userManager;



        const string librarian = "Библиотекарь";
        const string user = "Пользователь";

        public BookController(UserManager<User> userManager, IUnitOfWork unit, IRepository<Order> orderRep, IRepository<Book> bookRep, IRepository<User> userRep)
        {
            _dbO = orderRep;
            _dbB = bookRep;
            _dbU = userRep;
            _unitOfWork = unit;

            _userManager = userManager;


        }

        [Authorize(Roles = librarian)]
        public async Task<IActionResult> Index(string searchString, int search, string sortOrder)
        {
            var book = from b in _dbB.GetItems() select b;

            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
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
            if (!string.IsNullOrEmpty(searchString))
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


        [Authorize(Roles = librarian)]
        public IActionResult CreateBook()
        {
            return View();
        }

        [Authorize(Roles = librarian)]
        [HttpPost]
        public async Task<IActionResult> CreateBook(Book book, IFormFile uploadedFile)
        {

            if (uploadedFile != null)
            {
                string path = "/Files/" + uploadedFile.FileName;

                using (var fileStream = new FileStream("wwwroot" + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                Book bookModel = new Book { Id = book.Id, BookName = book.BookName, BookDescription = book.BookDescription, BookGenre = book.BookGenre, BookYear = book.BookYear, BookAuthor = book.BookAuthor, BookPublisher = book.BookPublisher, BookInStock = book.BookInStock, BookPicName = uploadedFile.FileName, BookPicPath = path };

                _dbB.Create(bookModel);
                _unitOfWork.Save();
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = librarian)]
        public IActionResult DetailsBook(long id)
        {
            if (id > 0)
            {
                Book book = _dbB.GetItem(id);
                if (book != null)
                    return View(book);
            }
            return NotFound();
        }

        [Authorize(Roles = librarian)]
        public IActionResult EditBook(long id)
        {
            Book book = _dbB.GetItem(id);
            if (book == null)
            {
                return NotFound();
            }
            BookPicViewModel bookPic = new BookPicViewModel { Id = book.Id, BookName = book.BookName, BookDescription = book.BookDescription, BookGenre = book.BookGenre, BookYear = book.BookYear, BookAuthor = book.BookAuthor, BookPublisher = book.BookPublisher, BookInStock = book.BookInStock, BookPicName = book.BookPicName, BookPicPath = book.BookPicPath };

            return View(bookPic);
        }

        [Authorize(Roles = librarian)]
        [HttpPost]
        public async Task<IActionResult> EditBook(BookPicViewModel bookViewModel, IFormFile uploadedFile)
        {
            if (ModelState.IsValid)
            {
                Book book = _dbB.GetItem(bookViewModel.Id);
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


                        if (System.IO.File.Exists("wwwroot"+ book.BookPicPath))
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

                    _dbB.Update(book);
                    _unitOfWork.Save();
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
            if (id > 0)
            {
                Book book = _dbB.GetItem(id);
                if (book != null)
                    return View(book);
            }
            return NotFound();
        }

        [Authorize(Roles = librarian)]
        [HttpPost]
        public IActionResult DeleteBook(long id)
        {
            if (id > 0)
            {
                Book book = _dbB.GetItem(id);
                if (System.IO.File.Exists("wwwroot" + book.BookPicPath))
                {
                    System.IO.File.Delete("wwwroot" + book.BookPicPath);
                }
                _dbB.Delete(book);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return NotFound();
        }

    }
}
