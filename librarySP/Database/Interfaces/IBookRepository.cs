using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace librarySP.Database.Interfaces
{
    interface IBookRepository <T> : IDisposable
        where T : class
    {
        IEnumerable<T> GetBooks();
        T GetBook(int id);
        void Create(T item);
        void Update(T item);
        void Delete(int id); 
        void Save();
    }
}
