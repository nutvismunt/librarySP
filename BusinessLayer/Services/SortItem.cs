using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace DataLayer.Services
{
    public class SortItem<T> : ISortItem<T> where T : class
    {
        private readonly ILogger<SortItem<T>> _logger;
        private readonly IRepository<T> _repository;
        public readonly IUnitOfWork _unitOfWork;
        public SortItem(ILogger<SortItem<T>> logger, IRepository<T> repository,
                        IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _repository = repository;
            _unitOfWork = unitOfWork;

        }

        public IQueryable<T> SortedItems(string sort, bool ascdesc)
        {
            var items = from b in _repository.GetItems().AsNoTracking() select b;
            var property = typeof(T).GetProperty(sort);
            var parameterExpression = Expression.Parameter(typeof(T), "o");
            var selectorExpression = Expression.Lambda(
                Expression.Property(parameterExpression, sort),
                parameterExpression);
            var query = items.Expression;
            query = Expression.Call(typeof(Queryable),
                ascdesc ? "OrderBy" : "OrderByDescending",
                new Type[]
                {
                    items.ElementType,
                    property.PropertyType
                },
                query,
                selectorExpression
                );

            return items.Provider.CreateQuery<T>(query);
        }

    }
}
