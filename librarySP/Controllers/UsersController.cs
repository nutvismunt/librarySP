using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using librarySP.Database.Entities;
using librarySP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace librarySP.Controllers
{
    public class UsersController : Controller
    {
        UserManager<User> _userManager;

        const string admin = "Администратор";
        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(Roles = admin)]
        public IActionResult Index() => View(_userManager.Users.ToList());

        [Authorize(Roles = admin)]
        public IActionResult CreateUser() => View();

        [Authorize(Roles = admin)]
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.Email, Name = model.Name, Surname = model.Surname, PhoneNum = model.PhoneNum };

                ViewBag.UserId = user.Id;

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
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

  /*      public async Task<IActionResult> ChangePassword(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ChangePasswordViewModel model = new ChangePasswordViewModel { Id = user.Id, Email = user.Email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    var _passwordValidator = HttpContext.RequestServices.GetService(typeof(IPasswordValidator<User>)) as IPasswordValidator<User>;
                    var _passwordHasher = HttpContext.RequestServices.GetService(typeof(IPasswordHasher<User>)) as IPasswordHasher<User>;

                    IdentityResult result = await _passwordValidator.ValidateAsync(_userManager, user, model.NewPassword);
                    if (result.Succeeded)
                    {
                        user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
                        await _userManager.UpdateAsync(user);
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
                else
                {
                    ModelState.AddModelError(string.Empty, "Пользователь не найден");
                }
            }
            return View(model);
        }*/
    }
}

