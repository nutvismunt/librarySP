﻿using DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.Initializers
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            const string adminName = "Администратор";
            string adminEmail = "admin@gmail.com";
            string adminPassword = "Aa!123";

            const string librarianName = "Библиотекарь";
            string librarianEmail = "librarian@gmail.com";
            string librarianPassword = "Aa!123";


            string superUserEmail = "nutvismunt@gmail.com";
            string superUserPassword = "Aa!123";

            const string userName = "Пользователь";

            if (await roleManager.FindByNameAsync(adminName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(adminName));
            }

            if (await roleManager.FindByNameAsync(librarianName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(librarianName));
            }

            if (await roleManager.FindByNameAsync(userName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(userName));
            }

            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                User admin = new User { Email = adminEmail, UserName = adminEmail };
                IdentityResult result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, adminName);
                }
            }

            if (await userManager.FindByNameAsync(librarianEmail) == null)
            {
                User librarian = new User { Email = librarianEmail, UserName = librarianEmail };
                IdentityResult result = await userManager.CreateAsync(librarian, librarianPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(librarian, librarianName);
                }
            }

            if (await userManager.FindByNameAsync(superUserEmail) == null)
            {
                User superUser = new User { Email = superUserEmail, UserName = superUserEmail };
                IdentityResult result = await userManager.CreateAsync(superUser, superUserPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(superUser, adminName);
                    await userManager.AddToRoleAsync(superUser, librarianName);
                    await userManager.AddToRoleAsync(superUser, userName);
                }
            }
        }
    }
}
