using Microsoft.AspNetCore.Identity;
using System;

namespace DataLayer.Entities
{
    public class User : IdentityUser
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public DateTime UserDate { get; set; } //дата создания пользователя

        public DateTime LasOrder { get; set; } // когда в последний раз производил заказ

        public int TotalOrders { get; set; } // всего произведено заказов


    }
}
