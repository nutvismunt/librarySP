using AutoMapper;
using AutoMapper.Internal;
using DataLayer.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;

namespace ExtensionMethods.IQueryableExtensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T>  SortedItems<T>(this IQueryable<T> items,string sort, bool ascdesc) where T : class
        {
            var property = typeof(T).GetProperty(sort);                                                 // выбор поля объекта для сортировки
            var parameterExpression = Expression.Parameter(typeof(T), "x");                             // параметр в лямбда выражении
            var propertyExpression = Expression.Property(parameterExpression, sort);
            var selectorExpression = Expression.Lambda(propertyExpression, parameterExpression);        // построение лямбда выражения x=>x.SomeField

            var query = items.Expression;                                                               // объекты в выражении для expression
            var types = new[] { items.ElementType, property.PropertyType };
            var methodName = ascdesc ? "OrderBy" : "OrderByDescending";
            query = Expression.Call(typeof(Queryable), methodName, types, query, selectorExpression);   // построение выражения в expression  
            // сортировка объектов с помощью запроса query
            return items.Provider.CreateQuery<T>(query);

        }

        public static IQueryable<T> Search<T>(this IQueryable<T> items, string searchString) where T : class
        {
            var parameterExpression = Expression.Parameter(typeof(T), "x");                                                     // параметр в лямбда выражении
            Expression<Func<T, bool>> b = null;                                                                                 // объявление пустых переменных для использования вне цикла
            MethodCallExpression propertyToString = null;
            MethodCallExpression propertyToLower = null;
            List<T> ts = new List<T>();
            var p = items;
            int t;
            var query = items.Expression;
            if (items != null)
            {
                foreach (var item in items.First().GetType().GetProperties())                                                       // цикл для проверки всех полей сущности
                {
                    if (item.Name != "BookPicName"&& item.Name!="BookPicPath") { 
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
                    b = Expression.Lambda<Func<T, bool>>(query, parameterExpression);

                    p = items.Where(b).AsNoTracking();
                    try { t = p.AsEnumerable<T>().Count<T>(); }
                    catch { break; }
                    if (t > 0)    
                        ts.AddRange(p.Except(ts));
                    }
                }
            }
            return ts.AsQueryable();
        }


    }
}
