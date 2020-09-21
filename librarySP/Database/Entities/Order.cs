using System;
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

        public long UserId { get; set; }

        public User User { get; set; }


        public long BookId { get; set; }

        public Book Book { get; set; }

    }
}
