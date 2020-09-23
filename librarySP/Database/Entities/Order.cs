﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace librarySP.Database.Entities
{
    public class Order
    {
        [Key]
        public long OrderId { get; set; }

        // public bool Delivery {get;set;}

        public int Amount { get; set; }

        public User User { get; set; }

        public string UserId { get; set; }
        public Book Book { get; set; }
        public long BookId { get; set; }
        public string UserName { get; set; }
        public string ClientNameSurName { get; set; }
        public string ClientPhoneNum { get; set; }


        public bool IsRequested { get; set; }

    }
}
