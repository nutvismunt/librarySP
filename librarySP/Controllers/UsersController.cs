using System;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using BusinessLayer.Models.UserDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace librarySP.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        const string admin = "Администратор";
        const string userRole = "Пользователь";

        public UsersController(IUserService userService) => _userService = userService;

        [Authorize(Roles = admin)]
        public IActionResult Index(string searchString)
        {
            var users = _userService.GetUsers();
            if (!string.IsNullOrEmpty(searchString))
            {
                var userSearcher = _userService.SearchUser(searchString);
                if (userSearcher != null)
                    return View(userSearcher);
            }
            return View(users.ToList());
        }

        [Authorize(Roles = admin)]
        public IActionResult CreateUser() => View();

        [Authorize(Roles = admin)]
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new UserViewModel
                {
                    Email = model.Email,
                    UserName = model.Email,
                    Name = model.Name,
                    Surname = model.Surname,
                    PhoneNumber = model.PhoneNum,
                    UserDate = DateTime.Now
                };

                var result = await _userService.CreateUser(user, model.Password);
                if (result.Succeeded)
                {
                    await _userService.AddRole(user, userRole); // по умолчанию выдается роль пользователя
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [Authorize(Roles = admin)]
        public IActionResult EditUser(string id)
        {
            var user = _userService.GetUserById(id).Result;
            if (user == null)
            {
                return NotFound();
            }
            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                PhoneNum = user.PhoneNumber
            };
            return View(model);
        }

        [Authorize(Roles = admin)]
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.GetUserById(model.Id);
                if (user != null)
                {
                    user.Email = model.Email;
                    user.UserName = model.Email;
                    user.Name = model.Name;
                    user.Surname = model.Surname;
                    user.PhoneNumber = model.PhoneNum;
                    var result = await _userService.UserValidator(model, user);
                    var _passwordHasher = _userService.UserHasher(model);
                    if (result.Succeeded)
                    {
                        if (model.NewPassword != null)
                        {
                            user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
                            await _userService.UpdateUser(user);
                            return RedirectToAction("Index");
                        }
                        else { return RedirectToAction("Index"); }
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            return View(model);
        }

        [Authorize(Roles = admin)]
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = _userService.GetUserById(id).Result;
            if (user != null)
            {
                await _userService.DeleteUser(user);
            }
            return RedirectToAction("Index");
        }
    }
}

