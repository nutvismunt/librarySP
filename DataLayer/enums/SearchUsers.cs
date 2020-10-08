using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.enums
{
    public enum SearchUsers
    {
        [Display(Name = "Почта")]
        Email,

        [Display(Name = "Имя")]
        Name,

        [Display(Name = "Фамилия")]
        Surname,

        [Display(Name = "Телефон")]
        PhoneNum
    }
}
