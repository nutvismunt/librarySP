namespace DataLayer.Entities
{
    public class ParserLastUrl
    {
        public long Id { get; set; }

        public int LastUrl { get; set; } //url последней книги в фоновой задаче

        public int BookAmount { get; set; } //количество книг которые должна добавить фоновая задача
       
    }
}
