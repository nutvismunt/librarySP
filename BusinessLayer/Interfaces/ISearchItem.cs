﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLayer.Interfaces
{
    public interface ISearchItem<T> where T : class
    {
        // поиск объектов
         IQueryable<T> Search(string searchString);
    }
}
