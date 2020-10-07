using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using librarySP.Database.Entities;
using librarySP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace librarySP.Controllers
{
    public class UsersController : Controller
    {
        UserManager<User> _userManager;

        const string admin = "Администратор";
        const string userRole = "Пользователь";
        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(Roles = admin)]
        public IActionResult Index(string searchString, int search)
        {
            var users = from u in _userManager.Users select u;
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (search)
                {
                    case 0: users = users.Where(s => s.Email.ToLower().Contains(searchString.ToLower()));
                        break;
                    case 1: users = users.Where(s => s.Name.ToString().ToLower().Contains(searchString.ToLower()));
                        break;
                    case 2: users = users.Where(s => s.Surname.ToLower().Contains(searchString.ToLower()));
                        break;
                    case 3: users = users.Where(s => s.PhoneNum.ToLower().Contains(searchString.ToLower()));
                        break;
                }
                return View(users.ToList());
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
                User user = new User { Email = model.Email, UserName = model.Email, Name = model.Name, Surname = model.Surname, PhoneNum = model.PhoneNum };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, userRole); // по умолчанию выдается роль пользователя
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
        public async Task<IActionResult> EditUser(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            EditUserViewModel model = new EditUserViewModel { Id = user.Id, Email = user.Email, Name = user.Name, Surname=user.Surname, PhoneNum=user.PhoneNum };
            return View(model);
        }

        [Authorize(Roles = admin)]
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    user.Email = model.Email;
                    user.UserName = model.Email;
                    user.Name = model.Name;
                    user.Surname = model.Surname;
                    user.PhoneNum = model.PhoneNum;

                    IdentityResult result;
                    IPasswordHasher<User> _passwordHasher;

                    if (model.NewPassword != null)
                    {
                        var _passwordValidator = HttpContext.RequestServices.GetService(typeof(IPasswordValidator<User>)) as IPasswordValidator<User>;
                        _passwordHasher = HttpContext.RequestServices.GetService(typeof(IPasswordHasher<User>)) as IPasswordHasher<User>;
                        result = await _passwordValidator.ValidateAsync(_userManager, user, model.NewPassword);
                    }
                    else
                    {
                        _passwordHasher = null;
                        result = await _userManager.UpdateAsync(user);
                    }

                    if (result.Succeeded)
                    {
                        if (model.NewPassword != null)
                        {
                            user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
                            await _userManager.UpdateAsync(user);
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
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }
    }
}

