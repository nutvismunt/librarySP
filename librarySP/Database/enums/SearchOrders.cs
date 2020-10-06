using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace librarySP.Database.enums
{
    public enum SearchOrders
    {
        [Display(Name = "ID заказа")]
        OrderId,
        [Display(Name = "ID книги")]
        BookId,
        [Display(Name = "Название книги")]
        BookName,
        [Display(Name = "Почта")]
        UserName,
        [Display(Name = "Телефон")]
        ClientPhoneNum,
        [Display(Name = "Пользователь")]
        ClientNameSurName
    }
}
