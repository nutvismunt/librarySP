using System.ComponentModel.DataAnnotations;

namespace DataLayer.enums
{
    public enum OrderStatus
    {
        [Display(Name = "Забронирован")]

        Booked,
        [Display(Name = "Ожидает")]

        Waiting,
        [Display(Name = "Выдан")]
        Given,

       [Display(Name = "Завершен")]
        Completed
    }
}
