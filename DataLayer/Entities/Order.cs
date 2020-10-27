using DataLayer.enums;
using System;

namespace DataLayer.Entities
{
    public class Order
    {
        public long Id { get; set; }

        public int Amount { get; set; }

        public string UserId { get; set; }

        public long BookId { get; set; }

        public long ISBN { get; set; }

        public string BookName { get; set; }

        public string BookAuthor { get; set; }

        public string UserName { get; set; }

        public string ClientNameSurName { get; set; }

        public string ClientPhoneNum { get; set; }

        public OrderStatus? OrderStatus { get; set; }

        // когда был произведен заказ
        public DateTime OrderTime { get; set; }

        // когда заказ был возвразен
        public DateTime OrderReturned { get; set; }


    }
}
