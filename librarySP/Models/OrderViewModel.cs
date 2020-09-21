using librarySP.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace librarySP.Models
{
    public class OrderViewModel
    {
        public IEnumerable<Order> Orders { get; set; }
        public Order Order { get; set; }

        public IEnumerable<User> Users { get;  }
        public User User { get; }

        public IEnumerable<Book> Books { get; }
        public Book Book { get;  }
    }
}
