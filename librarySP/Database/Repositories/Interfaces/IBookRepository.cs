using librarySP.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace librarySP.Database.Repositories.Interfaces
{
    public interface IBookRepository 
    {
        public List<Book> GetBooksAsync();
    }
}
