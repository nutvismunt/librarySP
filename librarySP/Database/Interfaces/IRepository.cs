﻿using System.Linq;


namespace librarySP.Database.Interfaces
{
    public interface IRepository <T> where T : class
    {
        IQueryable<T> GetItems();

        T GetItem(long id);

        void Create(T item);

        void Update(T item);

        void Delete(T item); 

    }
}
