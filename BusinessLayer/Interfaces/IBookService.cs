using BusinessLayer.Models.BookDTO;
using DataLayer.Entities;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Interfaces
{
    public interface IBookService
    {
        //интерфейс для сервиса книг
        IQueryable<BookViewModel> GetBooks();

        BookViewModel GetBook(long id);

        void Create(BookViewModel book);

        void Update(BookViewModel book);

        void Delete(Book book);

        List<BookViewModel> SearchBook(string searchString, IQueryable<BookViewModel> query);

        IQueryable<BookViewModel> SortBooks(string sort, bool asc);

    }
}
