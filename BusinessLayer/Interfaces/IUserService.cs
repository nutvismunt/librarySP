using DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IUserService
    {
        //UserManager
        Task<User> GetUserById(string Id);

        Task<User> GetUser();

        UserManager<User> UserManagerEx();

        Task<IdentityResult> UpdateUser(User user);

        Task<IdentityResult> DeleteUser(User user);

        IQueryable<User> GetUsers();

        Task<IdentityResult> CreateUser(User user, string userPassword);

        Task<IdentityResult> AddRole(User user, string role);
        Task<IList<string>> GetRoles(User user);
        Task<IdentityResult> AddToRoles(User user, IEnumerable<string> addedRoles);

        Task<IdentityResult> RemoveFromRoles(User user, IEnumerable<string> removedRoles);

        Task SignIn(User user, bool b);

        Task SignOut();

        Task<SignInResult> PasswordSignIn(string email, string password, bool persistent, bool lockBool);

        List<User> SearchUser(string searchString);

        IQueryable<User> SortUsers(string sort, bool asc = true);


        //RoleManager
        RoleManager<IdentityRole> RoleManagerEx();

        Task<IdentityResult> CreateRoleAsync(string name);

        Task<IdentityRole> FindRoleById(string id);

        Task<IdentityResult> DeleteRole(IdentityRole role);
        IQueryable<IdentityRole> GetAllRoles();
        
    }
}
