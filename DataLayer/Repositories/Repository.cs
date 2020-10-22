using DataLayer.Entities;
using DataLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        internal LibraryContext context;
        internal DbSet<T> dbSet;
        private readonly IUnitOfWork _unitOfWork;

        public Repository(IUnitOfWork unitOfWork, LibraryContext context)
        {
            _unitOfWork = unitOfWork;
            this.context = context;
            this.dbSet = context.Set<T>();
        }

        public virtual IEnumerable<T> Get(
    Expression<Func<T, bool>> filter = null,
    Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
    string includeProperties = "")
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public void Create(T entity)
        {
            dbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }


        public T GetItem(long id)
        {
            var entity = dbSet.Find(id);
            return entity;
        }
        public IQueryable<T> GetItems()
        {
            var items = dbSet.AsQueryable<T>();
            return items;
        }


        public void Update(T entity)
        {
            _unitOfWork.Context.Entry(entity).State = EntityState.Modified;

        }

        public void Detatch(T entity)
        {
            _unitOfWork.Context.Entry(entity).State = EntityState.Detached;
        }
    }
}
