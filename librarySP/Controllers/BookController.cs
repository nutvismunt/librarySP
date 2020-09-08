using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using librarySP.Database;
using librarySP.Database.Entities;
using librarySP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace librarySP.Controllers
{
    public class BookController : Controller
    {
        private LibraryContext db;



        public BookController(LibraryContext context)
        {
            db = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await db.Books.ToListAsync());
        }


        public IActionResult CreateBook()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateBook(Book book)
        {
            db.Books.Add(book);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

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
        [HttpPost]
        public async Task<IActionResult> EditBook(Book book)
        {
            db.Books.Update(book);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
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


    }
}
