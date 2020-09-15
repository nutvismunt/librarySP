using librarySP.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace librarySP.Database.Repositories.Interfaces
{
    public interface IBookRepository 
    {
        
        IEnumerable<Book> GetBooks();

        Book book { get; set; }

        //  void CreateBook();
        void CreateBook(Book book);

        void DetailsBook(int? id);

        void EditBook(int? id);

        void EditBook(Book book);

        void ConfirmDelete(int? id);

        void DeleteBook(int? id);

    }
}
