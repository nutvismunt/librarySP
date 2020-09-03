using librarySP.Database.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace librarySP.Models
{
    public class LibraryContext : IdentityDbContext<User>
    {
        public DbSet<Book> Books { get; set; }
        public DbSet <Client> Clients  { get; set; }
      //  public DbSet<User> Users { get; set; }
        public DbSet<Microsoft.AspNetCore.Identity.IdentityUserClaim<Guid>> IdentityUserClaims { get; set; }





        public LibraryContext(DbContextOptions<LibraryContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }


    }
}
