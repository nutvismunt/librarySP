using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLayer.Interfaces
{
    public interface ISearchBook<T> where T : class
    {
        public IQueryable<T> Search(string searchString);
    }
}
