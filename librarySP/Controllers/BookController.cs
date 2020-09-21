using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using librarySP.Database;
using librarySP.Database.Entities;
using librarySP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace librarySP.Controllers
{
    public class BookController : Controller
    {
        private LibraryContext db;

        protected int BookInStockHolder;
        protected int AmountHolder;
        const string librarian = "Библиотекарь";
        const string user = "Пользователь";

        public BookController(LibraryContext context)
        {
            db = context;
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
        public async Task<IActionResult> Order(int? id)
        {
            if (id == null) return RedirectToAction("Index");
            ViewBag.BookId = id;
            Book book = await db.Books.FirstOrDefaultAsync(p => p.Id == id);
            // ViewBag.UserId = this.db.;
            return View();
        }

        [HttpPost]
        public async Task <IActionResult> Order(Order order, int? id, long? userIdHolder)
        {
            db.Orders.Add(order);
            Book book = await db.Books.FirstOrDefaultAsync(p => p.Id == id);
            order.Book.BookInStock -= order.Amount;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> OrderList()
        {
            var book = db.Orders.Include(c => c.Book).AsNoTracking();
            return View(await book.ToListAsync());
        }


        [HttpPost]
        public async Task<IActionResult> DeleteOrder(int? id, long? bookIdHolder, long? userIdHolder)
        {


            if (id != null)
            {

                    Book bookHolder = await db.Books.FirstOrDefaultAsync(p => p.Id == bookIdHolder);
                    Order orderHolder = await db.Orders.FirstOrDefaultAsync(p => p.OrderId == id);
                    bookHolder.BookInStock += orderHolder.Amount;


                db.Entry(orderHolder).State = EntityState.Deleted;
                await db.SaveChangesAsync();

                return RedirectToAction("OrderList");
            }
            return NotFound();
        }
    }
}
