using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace librarySP.Database.Entities
{
    public class Book 
    {
        public long Id { get; set; }
        public string BookName { get; set; }

        public string BookDescription { get; set; }

        public string BookGenre{ get; set; }

        public string BookYear { get; set; }

        public string BookAuthor { get; set; }

        public int BookInStock { get; set; }

        //public string BookImage { get; set; }
    }
}
