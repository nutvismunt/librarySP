using DataLayer.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer
{
    public class LibraryContext : IdentityDbContext<User>
    {
        public DbSet<Book> Books { get; set; }

        public DbSet<Order> Orders { get; set; }

        public LibraryContext(DbContextOptions<LibraryContext> options)
            : base(options)
        {
            Database.Migrate();
        }

    }
}
