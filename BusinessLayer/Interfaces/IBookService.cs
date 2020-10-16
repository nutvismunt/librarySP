using DataLayer.Entities;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Interfaces
{
    public interface IBookService
    {
        IQueryable<Book> GetBooks();

        Book GetBook(long id);

        void Create(Book book);

        void Update(Book book);

        void Delete(Book book);

        List<Book> SearchBook(string searchString);

        IQueryable<Book> SortBooks(string sort, bool asc = true);

    }
}
