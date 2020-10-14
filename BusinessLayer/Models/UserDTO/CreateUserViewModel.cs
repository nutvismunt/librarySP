using BusinessLayer.Models.RoleDTO;
using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Models.UserDTO
{
    public class CreateUserViewModel
    {
        public string UserId { get; set; }
        [Required(ErrorMessage = "Поле Email не заполнено")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле Пароль не заполнено")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Поле Имя не заполнено")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле Фамилия не заполнено")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Поле Телефон не заполнено")]
        public string PhoneNum { get; set; }

        public ChangeRoleViewModel ChangeRole { get; set; }

    }
}
