using BusinessLayer.Interfaces;
using DataLayer.Entities;
using DataLayer.enums;
using DataLayer.Interfaces;
using Microsoft.AspNetCore.Razor.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLayer.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _repository;
        private readonly ISearchItem<Order> _searchOrder;
        private readonly ISortItem<Order> _sortOrder;
        private readonly IUnitOfWork _unitOfWork;
        public OrderService(IRepository<Order> repository, ISearchItem<Order> searchOrder, ISortItem<Order> sortOrder, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _searchOrder = searchOrder;
            _sortOrder = sortOrder;
            _unitOfWork = unitOfWork;
        }
        public void Create(Order order)
        {
            _repository.Create(order);
            _unitOfWork.Save();
        }

        public void Delete(Order order)
        {
            _repository.Delete(order);
            _unitOfWork.Save();
        }

        public Order GetOrder(long id)
        {
           return _repository.GetItem(id);
        }

        public IQueryable<Order> GetOrders()
        {
            return _repository.GetItems();
        }

        public List<Order> SearchOrder(string searchString)
        {
            return _searchOrder.Search(searchString);
        }

        public IQueryable<Order> SortOrders(string sort, bool asc = true)
        {
            return _sortOrder.SortedItems(sort);
        }

        public OrderStatus Status(string orderStatus)
        {
            return (OrderStatus)Enum.Parse(typeof(OrderStatus), orderStatus);
        }

        public void Update(Order order)
        {
             _repository.Update(order);
            _unitOfWork.Save();
        }
    }
}
