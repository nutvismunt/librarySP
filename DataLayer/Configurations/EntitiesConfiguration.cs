using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Configurations
{
    public class EntitiesConfiguration : IEntityTypeConfiguration<Book>, IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.Property(b => b.Id).ValueGeneratedOnAdd();
        }

        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(o => o.Id).ValueGeneratedOnAdd();
        }
    }
}
