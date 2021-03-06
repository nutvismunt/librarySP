﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using BusinessLayer.Models.RoleDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace librarySP.Controllers
{
    public class RolesController : Controller
    {

        private readonly IUserService _userService;
        const string admin = "Администратор";
        public RolesController(IUserService userService) => _userService = userService;

        //вывод ролей
        public IActionResult Index() => View(_userService.GetAllRoles());

        //страница с созданием ролей
        public IActionResult CreateRole() => View();

        //прием данных с view для создания роли
        [Authorize(Roles = admin)]
        [HttpPost]
        public async Task<IActionResult> CreateRole(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var result = await _userService.CreateRoleAsync(name);
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
            return View(name);
        }

        // удаление роли
        [Authorize(Roles = admin)]
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
           var role = await _userService.FindRoleById(id);
            if (role != null)
            {
                await _userService.DeleteRole(role);
            }
            return RedirectToAction("Index");
        }

        // вывод списка пользователей
        [Authorize(Roles = admin)]
        public IActionResult UserList() => View(_userService.GetUsers());

        [Authorize(Roles = admin)]
        public async Task<IActionResult> EditRole(string userId)
        {

            var user = await _userService.GetUserById(userId);
            if (user != null)
            {
                var userRoles = await _userService.GetRoles(user);
                var allRoles = _userService.GetAllRoles().ToList();
                var model = new ChangeRoleViewModel
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserRoles = userRoles,
                    AllRoles = allRoles
                };
                return View(model);
            }
            return NotFound();
        }

        // редактирование роли
        [Authorize(Roles = admin)]
        [HttpPost]
        public async Task<IActionResult> EditRole(string userId, List<string> roles)
        {
            var user = await _userService.GetUserById(userId);
            if (user != null)
            {
                var userRoles = await _userService.GetRoles(user);
                var addedRoles = roles.Except(userRoles);
                var removedRoles = userRoles.Except(roles);
                await _userService.AddToRoles(user, addedRoles);
                await _userService.RemoveFromRoles(user, removedRoles);
                return RedirectToAction("UserList");
            }
            return NotFound();
        }
    }
}
