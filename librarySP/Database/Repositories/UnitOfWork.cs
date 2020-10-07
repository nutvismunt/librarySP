using librarySP.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace librarySP.Database.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public LibraryContext Context { get; }

        public UnitOfWork(LibraryContext context)
        {
            Context = context;
        }
        public void Save()
        {
            Context.SaveChanges();
        }

        public void Dispose()
        {
            Context.Dispose();

        }
    }
}
