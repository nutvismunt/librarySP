using DataLayer.Entities;
using DataLayer.enums;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Interfaces
{
    public interface IOrderService
    {
        IQueryable<Order> GetOrders();

        Order GetOrder(long id);

        void Create(Order order);

        void Update(Order order);

        void Delete(Order order);

        List<Order> SearchOrder(string searchString);

        IQueryable<Order> SortOrders(string sort, bool asc = true);
        OrderStatus Status(string orderStatus);

    }
}
