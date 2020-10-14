using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Models.UserDTO
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string NewPassword { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string PhoneNum { get; set; }

    }
}
