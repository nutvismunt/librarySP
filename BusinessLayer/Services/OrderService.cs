using AutoMapper;
using BusinessLayer.Interfaces;
using BusinessLayer.Models.OrderDTO;
using DataLayer.Entities;
using DataLayer.enums;
using DataLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public void Create(OrderViewModel order)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Order, OrderViewModel>()).CreateMapper();
            _repository.Create(config.Map<Order>(order));
            _unitOfWork.Save();
        }

        public void Delete(Order order)
        {
            var local = _unitOfWork.Context.Set<Order>().Local
    .FirstOrDefault(entry => entry.Id.Equals(order.Id));
            if (local != null)
            {
                _repository.Detatch(local);
            }
            _repository.Delete(order);
            _unitOfWork.Save();
        }

        public OrderViewModel GetOrder(long id)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Order, OrderViewModel>()).CreateMapper();
            return config.Map<Order, OrderViewModel>(_repository.GetItem(id));
        }

        public IQueryable<OrderViewModel> GetOrders()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Order, OrderViewModel>()).CreateMapper();
            var order = config.Map<List<Order>, List<OrderViewModel>>(_repository.GetItems().ToList());
            var orders = order.AsQueryable();
            
            return orders;
        }

        public List<OrderViewModel> SearchOrder(string searchString)
        {
            var order = _searchOrder.Search(searchString);
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Order, OrderViewModel>()).CreateMapper();
            return config.Map<List<OrderViewModel>>(order);
        }

        public IQueryable<OrderViewModel> SortOrders(string sort, bool asc)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Order, OrderViewModel>()).CreateMapper();
            var sorter = config.Map<List<Order>, List<OrderViewModel>>(_sortOrder.SortedItems(sort,asc).ToList());
            var orders = sorter.AsQueryable();

            return orders;
        }

        public OrderStatus Status(string orderStatus)
        {
            return (OrderStatus)Enum.Parse(typeof(OrderStatus), orderStatus);
        }

        public void Update(OrderViewModel order)
        {
            var local = _unitOfWork.Context.Set<Order>().Local
                .FirstOrDefault(entry => entry.Id.Equals(order.Id));
            if (local != null)
            {
                _repository.Detatch(local);
            }
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Order, OrderViewModel>()).CreateMapper(); 
            _repository.Update(config.Map<Order>(order));
            _unitOfWork.Save();
        }
    }
}
