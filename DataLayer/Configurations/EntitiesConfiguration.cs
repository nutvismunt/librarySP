using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Configurations
{
    public class EntitiesConfiguration : IEntityTypeConfiguration<Book>, IEntityTypeConfiguration<Order>, IEntityTypeConfiguration<ParserLastUrl>
    {
        // добавленеи сущности книг в бд
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.Property(b => b.Id).ValueGeneratedOnAdd();
        }

        //добавление сущности заказов в бд
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(o => o.Id).ValueGeneratedOnAdd();
        }

        // добавление сущности парсера в бд
        public void Configure(EntityTypeBuilder<ParserLastUrl> builder)
        {
            builder.Property(o => o.Id).ValueGeneratedOnAdd();
        }

        //User строится в LibraryContext с помощью IdentityDbContext<User>
    }
}
