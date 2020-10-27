using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

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
            var items = from b in _rep.GetItems().AsNoTracking() select b;   // получение объектов
            var list = new List<T> { };                                      // создание пустого списка для заполнения
            var dataHolder = "";
            foreach (var item in items)                                      // для каждой строки в определенной таблице (сущности) бд
            {
                foreach (var info in item.GetType().GetProperties())         // получение типа полей
                {
                    var dataValue = info.GetValue(item);                     //получение содержимого поля
                    if (dataValue != null)
                    {
                        dataHolder = dataValue.ToString().ToLower();         // приведение к нижнему регистру для того, чтобы поиск выдавал выражения независимо от регистра
                        if (dataHolder.Contains(searchString.ToLower()))     // проверяется содержит ли строка поисковой запрос
                        {
                            if (list.Contains(item) == false)                // если ещё не существует в списке, то добавляется в список, условие добавлено из-за особенности Identity
                                list.Add(item);                              // identity при поиске выдает сразу 4 экземпляра одного объекта
                        }
                    }
                }
            }
            return list;                                                     // возвращаются результаты поиска в виде списка
        }
    }
}
