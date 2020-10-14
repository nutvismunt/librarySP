using BusinessLayer.Interfaces;
using DataLayer.Entities;
using DataLayer.enums;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BusinessLayer.Services
{
    public class SearchBook<T> : ISearchBook<T> where T : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SearchBook<T>> _logger;
        private readonly IRepository<T> _rep;

        public SearchBook(IUnitOfWork unitOfWork, ILogger<SearchBook<T>> logger, IRepository<T> rep)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _rep = rep;
        }

       

        public IQueryable<T> Search(string searchString)
        {
            // пока что не работает
            var items = from b in _rep.GetItems() select b;
            IQueryable<T> ts;
            List<T> list = new List<T>{ };
            string c="";
            foreach(var item in items)
            {

                foreach(PropertyInfo info in item.GetType().GetProperties())
                {
                    c = info.GetValue(item).ToString().ToLower();
                    if(c.Contains(searchString.ToLower()))
                    {
                        list.Add(item);
                    }
                    
                }

                _logger.LogInformation(c);
            }
            return items;







            /*
           IQueryable<T> objects =  _unitOfWork.Context.Set<T>().AsNoTracking() ;
            foreach (var objectT in objects)
            {
                return (IQueryable<T>)objectT.GetType().GetMembers().AsQueryable();
            }
            _logger.LogInformation("done");
            return (IQueryable<T>)objects.GetType().GetMembers().AsQueryable();  item.GetType().GetProperties().Where(c=>c.GetValue(item).ToString().Contains(searchStr))

            */
            //return _unitOfWork.Context.Set<T>().AsQueryable().Where(s=>s.GetType().GetMembers().AsQueryable().ToString().ToLower().Contains(searchStr.ToLower()));
        }
    }
}
