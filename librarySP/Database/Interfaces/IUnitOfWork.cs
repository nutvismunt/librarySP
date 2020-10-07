using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace librarySP.Database.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        LibraryContext Context { get; }
        void Save();
    }
}
