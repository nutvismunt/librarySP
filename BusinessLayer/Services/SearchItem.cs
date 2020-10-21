using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DataLayer.Services
{
    public class SearchItem<T> : ISearchItem<T> where T : class
    {
        private readonly ILogger<SearchItem<T>> _logger;
        private readonly IRepository<T> _rep;

        public SearchItem(ILogger<SearchItem<T>> logger, IRepository<T> rep)
        {
            _logger = logger;
            _rep = rep;
        }

        public List<T> Search(string searchString)
        {
            var items = from b in _rep.GetItems().AsNoTracking() select b;
            var list = new List<T> { };
            var dataHolder = "";
            foreach (var item in items)
            {
                foreach (var info in item.GetType().GetProperties())
                {
                    var dataValue = info.GetValue(item);
                    if (dataValue != null)
                    {
                        dataHolder = dataValue.ToString().ToLower();
                        if (dataHolder.Contains(searchString.ToLower()))
                        {
                            if (list.Contains(item) == false)
                                list.Add(item);
                        }
                    }
                }
                //  _logger.LogInformation(c); //вывод содержимого
            }
            return list;
        }
    }
}
