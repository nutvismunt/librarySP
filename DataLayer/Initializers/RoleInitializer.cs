using DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace DataLayer.Initializers
{
    public class RoleInitializer
    {
        // инициализатор ролей
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            // админ
            const string adminName = "Администратор";
            string adminEmail = "admin@gmail.com";
            string adminPassword = "Aa!123";
            //библиотекарь
            const string librarianName = "Библиотекарь";
            string librarianEmail = "librarian@gmail.com";
            string librarianPassword = "Aa!123";
            //superuser
            string superUserEmail = "nutvismunt@gmail.com";
            string superUserPassword = "Aa!123";
            // пользователь
            const string userName = "Пользователь";

            if (await roleManager.FindByNameAsync(adminName) == null)
            {await roleManager.CreateAsync(new IdentityRole(adminName));} //создать роль админа
            if (await roleManager.FindByNameAsync(librarianName) == null)
            {await roleManager.CreateAsync(new IdentityRole(librarianName));} //создать роль библиотекаря
            if (await roleManager.FindByNameAsync(userName) == null)
            {await roleManager.CreateAsync(new IdentityRole(userName));} // создать роль пользователя
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                //создать аккаунт админа
                var admin = new User { Email = adminEmail, UserName = adminEmail };
                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                { await userManager.AddToRoleAsync(admin, adminName); }
            }

            if (await userManager.FindByNameAsync(librarianEmail) == null)
            {
                //создать аккаунт библиотекаря
                var librarian = new User { Email = librarianEmail, UserName = librarianEmail };
                var result = await userManager.CreateAsync(librarian, librarianPassword);
                if (result.Succeeded)
                { await userManager.AddToRoleAsync(librarian, librarianName);}
            }

            if (await userManager.FindByNameAsync(superUserEmail) == null)
            {
                //создать аккаунт superuser
                var superUser = new User { Email = superUserEmail, UserName = superUserEmail };
                var result = await userManager.CreateAsync(superUser, superUserPassword);
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
