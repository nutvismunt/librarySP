using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusinessLayer.Interfaces;
using BusinessLayer.Models.OrderDTO;
using DataLayer.Entities;
using DataLayer.enums;
using DataLayer.Interfaces;
using ExtensionMethods.IQueryableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public OrderService(IRepository<Order> repository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public void Create(OrderViewModel order)
        {
            _repository.Create(_mapper.Map<Order>(order));
            _unitOfWork.Save();
        }

        public void Delete(Order order)
        {
            var local = _unitOfWork.Context.Set<Order>().Local.
                FirstOrDefault(entry => entry.Id.Equals(order.Id));
            if (local != null) _repository.Detatch(local);
            _repository.Delete(order);
            _unitOfWork.Save();
        }

        public OrderViewModel GetOrder(long id)
        {
            return _mapper.Map<Order, OrderViewModel>(_repository.GetItem(id));
        }

        public IQueryable<OrderViewModel> GetOrders()
        {
            var order = _mapper.Map<List<OrderViewModel>>(_repository.GetItems().
                ProjectTo<OrderViewModel>(_mapper.ConfigurationProvider));
            var orders = order.AsQueryable();           
            return orders;
        }

        public List<OrderViewModel> SearchOrder(string searchString)
        {
            var order =_repository.GetItems().Search(searchString);
            return _mapper.Map<List<OrderViewModel>>(order);
        }

        public IQueryable<OrderViewModel> SortOrders(string sort, bool asc)
        {
            var query = _repository.GetItems().SortedItems(sort, asc);
            var sorter = _mapper.Map<List<OrderViewModel>>(query.ProjectTo<OrderViewModel>(_mapper.ConfigurationProvider));
            return sorter.AsQueryable();
        }

        public OrderStatus Status(string orderStatus)
        {
            return (OrderStatus)Enum.Parse(typeof(OrderStatus), orderStatus);
        }

        public void Update(OrderViewModel order)
        {
            var local = _unitOfWork.Context.Set<Order>().Local.
                FirstOrDefault(entry => entry.Id.Equals(order.Id));
            if (local != null) _repository.Detatch(local);
            _repository.Update(_mapper.Map<Order>(order));
            _unitOfWork.Save();
        }
    }
}
