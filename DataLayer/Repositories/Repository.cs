using DataLayer.Entities;
using DataLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DataLayer.Repositories
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

            var entity = _unitOfWork.Context.Set<T>().Find(id);
           _unitOfWork.Context.Entry(entity).State = EntityState.Detached;
            return entity;

        }
        public IQueryable<T> GetItems()
        {
         //   _unitOfWork.Context.Entry(items).State = EntityState.Detached;
            var items = _unitOfWork.Context.Set<T>().AsQueryable<T>();

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
