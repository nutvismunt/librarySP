using System;

namespace DataLayer.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        LibraryContext Context { get; }

        void Save();
    }
}
