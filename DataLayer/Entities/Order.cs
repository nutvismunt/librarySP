using DataLayer.enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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

        public DateTime OrderTime { get; set; }

        public DateTime OrderReturned { get; set; }


    }
}
