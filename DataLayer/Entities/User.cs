using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class User : IdentityUser
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public DateTime UserDate { get; set; } //дата создания пользователя

        public DateTime LasOrder { get; set; }

        public int TotalOrders { get; set; }


    }
}
