using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using librarySP.Database;
using librarySP.Database.Entities;
using librarySP.Models;
using Microsoft.AspNetCore.Authorization;
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
        protected int BookInStockHolder;
        protected int AmountHolder;
        const string librarian = "Библиотекарь";
        const string user = "Пользователь";

        public BookController(UserManager<User> userManager, LibraryContext context)
        {
            db = context;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await db.Books.ToListAsync());
        }


        [Authorize(Roles = librarian)]
        public IActionResult CreateBook()
        {
            return View();
        }

        [Authorize(Roles = librarian)]
        [HttpPost]
        public async Task<IActionResult> CreateBook(Book book)
        {
            db.Books.Add(book);
            await db.SaveChangesAsync();
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
        public async Task<IActionResult> EditBook(Book book)
        {
            db.Books.Update(book);
            await db.SaveChangesAsync();
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
                Book book = new Book { Id = id.Value };
                db.Entry(book).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return NotFound();
        }

        [HttpGet]
        public IActionResult Order(int? id)
        {
            if (id == null) return RedirectToAction("Index");
            ViewBag.BookId = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Order(Order order, long id)
        {

            var user = await _userManager.GetUserAsync(HttpContext.User);
            order.UserId = user.Id;
            order.BookId = id;
            order.UserName = user.UserName;
            order.ClientNameSurName = user.Name + " " + user.Surname;
            order.ClientPhoneNum = user.PhoneNum;
            order.IsRequested = false;
            Book book = await db.Books.FirstOrDefaultAsync(p => p.Id == id);
            book.BookInStock -= order.Amount;
            db.Orders.Add(order);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> OrderList()
        {
            var User = await _userManager.GetUserAsync(HttpContext.User);
            var book = db.Orders.Include(c => c.Book).Where(c=>c.UserId==User.Id).Where(c=>c.IsRequested==false).AsNoTracking();
                return View(await book.ToListAsync());


        }
        public async Task<IActionResult> OrderAllList()
        {
            var User = await _userManager.GetUserAsync(HttpContext.User);
            var book = db.Orders.Include(c => c.Book).Where(c => c.UserId == User.Id).Where(c => c.IsRequested == true).AsNoTracking();

            return View(await book.ToListAsync());
        }




        [HttpPost]
        public async Task<IActionResult> DeleteOrder(int? id, long? bookIdHolder, string userIdHolder)
        {


            if (id != null)
            {

                Book bookHolder = await db.Books.FirstOrDefaultAsync(p => p.Id == bookIdHolder);
                Order orderHolder = await db.Orders.FirstOrDefaultAsync(p => p.OrderId == id);
                bookHolder.BookInStock += orderHolder.Amount;


                db.Entry(orderHolder).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                if(User.IsInRole(user))
                { return RedirectToAction("OrderList"); }
                if(User.IsInRole(librarian))
                { return RedirectToAction("OrderAllList"); }

            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> SendOrder(int? id)
        {
            var User = await _userManager.GetUserAsync(HttpContext.User);
            var order =await db.Orders.Where(c => c.UserId == User.Id).Where(c => c.IsRequested == false).ToListAsync();
            foreach(var item in order)
            {
                item.IsRequested = true;
            }
                await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}
