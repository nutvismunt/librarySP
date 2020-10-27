using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLayer.Interfaces
{
    public interface ISortItem<T> where T : class
    {
        //сортировка объектов
        IQueryable<T> SortedItems(string sort, bool asc);
    }
}
