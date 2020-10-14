using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Models;
using BusinessLayer.Models.UserDTO;
using DataLayer.Entities;
using DataLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace librarySP.Controllers
{
    public class UsersController : Controller
    {
        private readonly ISearchItem<User> _searchUser;
        private readonly IUserManagerRepository _userManagerRep;

        const string admin = "Администратор";
        const string userRole = "Пользователь";

        public UsersController(IUserManagerRepository userManagerRep, ISearchItem<User> searchUser)
        {
            _userManagerRep = userManagerRep;
            _searchUser = searchUser;
        }

        [Authorize(Roles = admin)]
        public IActionResult Index(string searchString, int search)
        {
            var users =  _userManagerRep.GetUsers();
            if (!string.IsNullOrEmpty(searchString))
            {
                var userSearcher = _searchUser.Search(searchString);
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
                User user = new User { 
                    Email = model.Email,
                    UserName = model.Email, 
                    Name = model.Name, 
                    Surname = model.Surname, 
                    PhoneNumber = model.PhoneNum };

                var result = _userManagerRep.CreateUser(user, model.Password).Result;
                if (result.Succeeded)
                {
                   await _userManagerRep.AddRole(user, userRole); // по умолчанию выдается роль пользователя
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
            User user = _userManagerRep.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            EditUserViewModel model = new EditUserViewModel { 
                Id = user.Id, 
                Email = user.Email, 
                Name = user.Name, 
                Surname = user.Surname, 
                PhoneNum = user.PhoneNumber };
            return View(model);
        }

        [Authorize(Roles = admin)]
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = _userManagerRep.GetUserById(model.Id);
                if (user != null)
                {
                    user.Email = model.Email;
                    user.UserName = model.Email;
                    user.Name = model.Name;
                    user.Surname = model.Surname;
                    user.PhoneNumber = model.PhoneNum;

                    IdentityResult result;
                    IPasswordHasher<User> _passwordHasher;

                    if (model.NewPassword != null)
                    {
                        var _passwordValidator = HttpContext.RequestServices.GetService(typeof(IPasswordValidator<User>)) as IPasswordValidator<User>;
                        _passwordHasher = HttpContext.RequestServices.GetService(typeof(IPasswordHasher<User>)) as IPasswordHasher<User>;
                        result = await _passwordValidator.ValidateAsync(_userManagerRep.UserManagerEx(), user, model.NewPassword);
                    }
                    else
                    {
                        _passwordHasher = null;
                        result = _userManagerRep.UpdateUser(user).Result;
                    }

                    if (result.Succeeded)
                    {
                        if (model.NewPassword != null)
                        {
                            user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
                            await _userManagerRep.UpdateUser(user);
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
            User user = _userManagerRep.GetUserById(id);
            if (user != null)
            {
                await _userManagerRep.DeleteUser(user);
            }
            return RedirectToAction("Index");
        }
    }
}

