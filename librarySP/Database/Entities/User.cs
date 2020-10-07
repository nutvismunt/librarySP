using librarySP.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace librarySP.Database.Entities
{
    public class User : IdentityUser
    {
            public string Name { get; set; }

            public string Surname { get; set; }
        
            public string PhoneNum { get; set; }

        public static implicit operator User(RegisterViewModel v)
        {
            throw new NotImplementedException();
        }
    }
}
