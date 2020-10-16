using DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Models.UserDTO
{
    public class UserViewModel : IdentityUser
    {
        public string Name { get; set; }

        public string Surname { get; set; }
    }
}
