using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace librarySP.Models
{
    public class CreateUserViewModel
    {
        [Required (ErrorMessage ="Поле Email не заполнено")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле Пароль не заполнено")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Поле Имя не заполнено")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле Фамилия не заполнено")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Поле Телефон не заполнено")]
        public string PhoneNum { get; set; }
    }
}
