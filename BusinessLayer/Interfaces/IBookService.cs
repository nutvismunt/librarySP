using BusinessLayer.Models.BookDTO;
using DataLayer.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IBookService
    {
        IQueryable<BookViewModel> GetBooks();

        BookViewModel GetBook(long id);

        void Create(BookViewModel book);

        void Update(BookViewModel book);

        void Delete(Book book);

        List<BookViewModel> SearchBook(string searchString);

        IQueryable<BookViewModel> SortBooks(string sort, bool asc = true);

    }
}
