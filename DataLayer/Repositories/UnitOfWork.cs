using DataLayer;
using DataLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.Repositories
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
