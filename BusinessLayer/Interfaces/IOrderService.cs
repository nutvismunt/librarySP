using BusinessLayer.Models.OrderDTO;
using DataLayer.Entities;
using DataLayer.enums;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Interfaces
{
    public interface IOrderService
    {
        IQueryable<OrderViewModel> GetOrders();

        OrderViewModel GetOrder(long id);

        void Create(OrderViewModel order);

        void Update(OrderViewModel order);

        void Delete(Order order);

        List<OrderViewModel> SearchOrder(string searchString);

        IQueryable<OrderViewModel> SortOrders(string sort, bool asc = true);
        OrderStatus Status(string orderStatus);

    }
}
