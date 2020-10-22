using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class Book
    {
        public long Id { get; set; }

        public string BookName { get; set; }

        public string BookDescription { get; set; }

        public string BookGenre { get; set; }

        public string BookYear { get; set; }

        public string BookAuthor { get; set; }

        public string BookPublisher { get; set; }

        public int BookInStock { get; set; }

        public long ISBN { get; set; }

        // путь к изображению и его название
        public string BookPicName { get; set; } //в конце названия .png

        public string BookPicPath { get; set; } //путь указан в виде /Files/BookPicName.png

        public DateTime WhenAdded { get; set; }

        public DateTime LastTimeOrdered { get; set; }

        public int TotalOrders { get; set; }

        public int TotalReturns { get; set; }
    }
}
