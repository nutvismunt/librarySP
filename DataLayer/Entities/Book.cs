using System;

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

        // когда книга добавлена в бд
        public DateTime WhenAdded { get; set; }

        // когда был произведен последний заказ книги
        public DateTime LastTimeOrdered { get; set; }

        // всего заказано
        public int TotalOrders { get; set; }

        // возвращено книг в библиотеку
        public int TotalReturns { get; set; }
    }
}
