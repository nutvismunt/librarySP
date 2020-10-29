using AutoMapper.Internal;
using DataLayer.Interfaces;
using GemBox.Spreadsheet.Charts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
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

        public IQueryable<T> Search(string searchString)
        {
            var items = from c in _rep.GetItems().AsNoTracking() select c;                                                      // получение объектов
            var parameterExpression = Expression.Parameter(typeof(T), "b");                                                     // параметр в лямбда выражении
            Expression<Func<T, bool>> b = null;                                                                                 // объявление пустых переменных для использования вне цикла
            MethodCallExpression propertyToString = null;
            MethodCallExpression propertyToLower = null;
            var query = items.Expression;
            foreach (var item in items.First().GetType().GetProperties())                                                       // цикл для проверки всех полей сущности
            {
                var property = Expression.Property(parameterExpression, item.Name);                                             // выбор поля для поиска
                if (item.PropertyType != typeof(string))                                                                        // если выражение не является string, то производится перевод в string... 
                {                                                                                                               // (в ином случае при переводе string в string программа выдает ошибку)
                    propertyToString = Expression.Call(property, typeof(object).GetMethod("ToString", Type.EmptyTypes));     
                    propertyToLower = Expression.Call(propertyToString, typeof(string).GetMethod("ToLower", Type.EmptyTypes));  // приведение к единому регистру для нечувствительного к регистру поиска
                }
                else
                    propertyToLower = Expression.Call(property, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
                var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });                                    // составление метода для contains
                var search = Expression.Constant(searchString.ToLower(), typeof(string));                                       // приведение запроса к нижнему регистру
                query = Expression.Call(propertyToLower, method, search);                                                       // построение выражения
                b = Expression.Lambda<Func<T, bool>>(query, parameterExpression);                                               // построение лямбда выражения для where
                var p = items.Where(b);                                                                                         // поиск
                if (p.Any()) goto exit;                                                                                         // если поле нескольких объектов одной сущности содержат в себе...
            }                                                                                                                   // элемент из поисковой строки, то выводится список этих объектов  
        exit:
            return items.Where(b);

        }
    }
}
