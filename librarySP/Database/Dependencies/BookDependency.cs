using librarySP.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace librarySP.Database.Dependencies
{
    public class BookDependency
    {
        public Book book { get; }

        public static implicit operator BookDependency(List<Book> v)
        {
            throw new NotImplementedException();
        }
    }
}
