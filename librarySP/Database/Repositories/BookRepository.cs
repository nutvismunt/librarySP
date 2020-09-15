using librarySP.Database.Entities;
using librarySP.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace librarySP.Database.Repositories
{
    public class BookRepository : IBookRepository
    {
        private LibraryContext _db;

        public Book book { get;set; }

        public BookRepository (LibraryContext db)
        {
            _db = db;
        }

        public IEnumerable<Book> GetBooks()
        {
            return _db.Books.AsEnumerable();
        }

        public void CreateBook(Book book)
        {
            _db.Books.Add(book);
            _db.SaveChanges();
        }

        public void DetailsBook(int? id)
        {
            Book book =  _db.Books.FirstOrDefault(p => p.Id == id);
        }

        public void EditBook(int? id)
        {
            Book book = _db.Books.FirstOrDefault(p => p.Id == id);
        }

        public void EditBook(Book book)
        {
            _db.Books.Update(book);
            _db.SaveChanges();
        }

         public  void ConfirmDelete(int? id)
        {
            Book book = _db.Books.FirstOrDefault(p => p.Id == id);
        }

        public void DeleteBook(int? id)
        {
            Book book = new Book { Id = id.Value };
            _db.Entry(book).State = EntityState.Deleted;
            _db.SaveChanges();
        }

    }

}
