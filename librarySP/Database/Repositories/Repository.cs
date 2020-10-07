using librarySP.Database.Entities;
using librarySP.Database.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace librarySP.Database.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly IUnitOfWork _unitOfWork;
        public Repository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public void Create(T entity)
        {
            _unitOfWork.Context.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            _unitOfWork.Context.Set<T>().Remove(entity);
        }


        public T GetItem(long id)
        {
            return _unitOfWork.Context.Set<T>().Find(id);
        }
        public IQueryable<T> GetItems()
        {
            return _unitOfWork.Context.Set<T>().AsQueryable<T>();
        }


        public void Update(T entity)
        {
            _unitOfWork.Context.Entry(entity).State = EntityState.Modified;
            _unitOfWork.Context.Set<T>().Attach(entity);
        }
    }
}
