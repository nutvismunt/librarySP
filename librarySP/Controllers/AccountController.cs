﻿
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
    public class AccountController : Controller
    {
        private readonly IUserManagerRepository _userManagerRep;
        const string userRole = "Пользователь";

        public AccountController(IUserManagerRepository userManagerRep)
        {
            _userManagerRep = userManagerRep;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { 
                    Email = model.Email, 
                    UserName = model.Email, 
                    Name = model.Name, 
                    Surname = model.Surname, 
                    PhoneNumber = model.PhoneNum };
                var result = await _userManagerRep.CreateUser(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManagerRep.AddRole(user, userRole); // по умолчанию выдается роль пользователя
                    await _userManagerRep.SignIn(user, false);
                    return RedirectToAction("Index", "Home");
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


        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }


        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userManagerRep.PasswordSignIn(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неверный логин или пароль");
                }
            }
            return View(model);
        }

        [AllowAnonymous]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _userManagerRep.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}
