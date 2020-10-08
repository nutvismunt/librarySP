using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.enums
{
    public enum OrderStatus
    {
        [Display(Name = "Забронирован")]
        Booked,
        [Display(Name = "Ожидает")]
        Waiting,
        [Display(Name = "Выдан")]
        Given
    }
}
