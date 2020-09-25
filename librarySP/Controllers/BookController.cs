using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using librarySP.Database;
using librarySP.Database.Entities;
using librarySP.Database.enums;
using librarySP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace librarySP.Controllers
{
    public class BookController : Controller
    {
        private LibraryContext db;

        UserManager<User> _userManager;

        IWebHostEnvironment _appEnvironment;

        const string librarian = "Библиотекарь";
        const string user = "Пользователь";

        public BookController(UserManager<User> userManager, LibraryContext context, IWebHostEnvironment appEnvironment)
        {
            db = context;

            _userManager = userManager;

            _appEnvironment = appEnvironment;
        }

        [Authorize]
        public async Task<IActionResult> Index(string searchString)
        {
            var book = from b in db.Books select b;
            if (!String.IsNullOrEmpty(searchString))
            {
                book = book.Where(s => s.BookName.Contains(searchString));
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
 
            if (uploadedFile!=null)
            {
                string path = "/Files/" + uploadedFile.FileName;

                using (var fileStream = new FileStream(_appEnvironment.WebRootPath+path,FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                Book bookModel = new Book {Id=book.Id,BookName=book.BookName,BookDescription=book.BookDescription,BookGenre=book.BookGenre,BookYear=book.BookYear,BookAuthor=book.BookAuthor,BookInStock=book.BookInStock, BookPicName = uploadedFile.FileName, BookPicPath = path };

                db.Books.Add(bookModel);
                await db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> DetailsBook(int? id)
        {
            if (id != null)
            {
                Book book = await db.Books.FirstOrDefaultAsync(p => p.Id == id);
                if (book != null)
                    return View(book);
            }
            return NotFound();
        }

        [Authorize(Roles = librarian)]
        public async Task<IActionResult> EditBook(int? id)
        {
            Book book = await db.Books.FirstOrDefaultAsync(p => p.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            BookPicViewModel bookPic = new BookPicViewModel { Id = book.Id, BookName = book.BookName, BookDescription = book.BookDescription, BookGenre = book.BookGenre, BookYear = book.BookYear, BookAuthor = book.BookAuthor, BookInStock = book.BookInStock, BookPicName = book.BookPicName, BookPicPath = book.BookPicPath };

            return View(bookPic);
        }

        [Authorize(Roles = librarian)]
        [HttpPost]
        public async Task<IActionResult> EditBook(BookPicViewModel bookViewModel, IFormFile uploadedFile)
        {
            if (ModelState.IsValid)
            {
                Book book = await db.Books.FirstOrDefaultAsync(c => c.Id == bookViewModel.Id);
                if (book != null)
                {

                    book.Id = bookViewModel.Id;
                    book.BookName = bookViewModel.BookName;
                    book.BookDescription = bookViewModel.BookDescription;
                    book.BookGenre = bookViewModel.BookGenre;
                    book.BookYear = bookViewModel.BookYear;
                    book.BookAuthor = bookViewModel.BookAuthor;
                    book.BookInStock = bookViewModel.BookInStock;
                    if (uploadedFile != null)
                    {

                        if (System.IO.File.Exists(_appEnvironment.WebRootPath + book.BookPicPath))
                        {
                            System.IO.File.Delete(_appEnvironment.WebRootPath + book.BookPicPath);
                        }
                        string path = "/Files/" + uploadedFile.FileName;

                        using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                        {
                            await uploadedFile.CopyToAsync(fileStream);

                        }
                        book.BookPicName = uploadedFile.FileName;
                        book.BookPicPath = path;
                    }

                    db.Books.Update(book);
                    await db.SaveChangesAsync();
                }

               else return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

            [Authorize(Roles = librarian)]
        [HttpGet]
        [ActionName("DeleteBook")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                Book book = await db.Books.FirstOrDefaultAsync(p => p.Id == id);
                if (book != null)
                    return View(book);
            }
            return NotFound();
        }

        [Authorize(Roles = librarian)]
        [HttpPost]
        public async Task<IActionResult> DeleteBook(int? id)
        {
            if (id != null)
            {
                Book book = await db.Books.FirstOrDefaultAsync(p => p.Id == id);
                db.Entry(book).State = EntityState.Deleted;

                if (System.IO.File.Exists(_appEnvironment.WebRootPath + book.BookPicPath))
                {
                    System.IO.File.Delete(_appEnvironment.WebRootPath + book.BookPicPath);
                }
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return NotFound();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Order(int? id)
        {
            if (id == null) return RedirectToAction("Index");
            ViewBag.BookId = id;
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Order(Order order, long id)
        {

            var user = await _userManager.GetUserAsync(HttpContext.User);
            order.UserId = user.Id;
            order.BookId = id;
            order.UserName = user.UserName;
            order.ClientNameSurName = user.Name + " " + user.Surname;
            order.ClientPhoneNum = user.PhoneNum;
            order.OrderStatus = 0;
            Book book = await db.Books.FirstOrDefaultAsync(p => p.Id == id);
            book.BookInStock -= order.Amount;
            db.Orders.Add(order);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> OrderList()
        {
            var User = await _userManager.GetUserAsync(HttpContext.User);
            var book = db.Orders.Include(c => c.Book).Where(c => c.UserId == User.Id).Where(c => c.OrderStatus == 0).AsNoTracking();
            return View(await book.ToListAsync());
        }

        [Authorize(Roles = librarian)]
        public async Task<IActionResult> OrderAllList(string searchString)
        {
            var orders = from b in db.Orders select b;
            if (!String.IsNullOrEmpty(searchString))
            {
                orders = orders.Include(c => c.Book).Where(s => s.UserName.Contains(searchString));
                return View(await orders.ToListAsync());
            }
            else
            {
                var book = db.Orders.Include(c => c.Book).AsNoTracking();
                return View(await book.ToListAsync());
            }

        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteOrder(int? id, long? bookIdHolder)
        {
            if (id != null)
            {

                Book bookHolder = await db.Books.FirstOrDefaultAsync(p => p.Id == bookIdHolder);
                Order orderHolder = await db.Orders.FirstOrDefaultAsync(p => p.OrderId == id);
                bookHolder.BookInStock += orderHolder.Amount;

                db.Entry(orderHolder).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                if (User.IsInRole(user))
                { return RedirectToAction("OrderList"); }
                if (User.IsInRole(librarian))
                { return RedirectToAction("OrderAllList"); }

            }
            return NotFound();
        }

        [Authorize(Roles = librarian)]
        [HttpPost]
        public async Task<IActionResult> GivingBook(int? id)
        {
            if (id != null)
            {
                Order orderHolder = await db.Orders.FirstOrDefaultAsync(p => p.OrderId == id);
                orderHolder.OrderStatus = (OrderStatus)2;

                await db.SaveChangesAsync();
                if (User.IsInRole(librarian))
                { return RedirectToAction("OrderAllList"); }

            }
            return NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendOrder()
        {
            var User = await _userManager.GetUserAsync(HttpContext.User);
            var order = await db.Orders.Where(c => c.UserId == User.Id).Where(c => c.OrderStatus == 0).ToListAsync();
            foreach (var item in order)
            {
                item.OrderStatus = (OrderStatus)1;
            }
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}
