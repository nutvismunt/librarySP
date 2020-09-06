using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace librarySP.Models
{
    public class EditUserViewModel 
    {
            public string Id { get; set; }

            public string Email { get; set; }

            public string Name { get; set; }

            public string Surname { get; set; }

            public string PhoneNum { get; set; }

    }
}
