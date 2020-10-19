using System.Linq;
using System;


namespace DataLayer.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetItems();

        T GetItem(long id);

        void Create(T item);

        void Update(T item);

        void Delete(T item);
        void Detatch(T item);

    }
}
