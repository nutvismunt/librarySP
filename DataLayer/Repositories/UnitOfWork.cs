using DataLayer.Interfaces;

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
