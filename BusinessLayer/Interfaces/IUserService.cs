using BusinessLayer.Models.UserDTO;
using BusinessLayer.Services;
using DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IUserService
    {
        //usermanager

        Task SignIn(User user, bool b);

        Task<SignInResult> PasswordSignIn(string email, string password, bool persistent, bool lockBool);

        Task<IdentityResult> UserValidator(EditUserViewModel model, User user);

        IPasswordHasher<User> UserHasher(EditUserViewModel model);

        Task SignOut();

        IQueryable<UserViewModel> GetUsers();

            
        Task<User> GetUserById(string Id);  //using httpcontext

        Task<User> GetUser();

        Task<IdentityResult> UpdateUser(User user);

        Task<IdentityResult> DeleteUser(User user);

        Task<IdentityResult> CreateUser(User user, string userPassword);

        Task<IdentityResult> AddRole(User user, string role);

        Task<IList<string>> GetRoles(User user);

        Task<IdentityResult> AddToRoles(User user, IEnumerable<string> addedRoles);

        Task<IdentityResult> RemoveFromRoles(User user, IEnumerable<string> removedRoles);


        List<User> SearchUser(string searchString);

        IQueryable<User> SortUsers(string sort, bool asc = true);


        //RoleManager

        Task<IdentityResult> CreateRoleAsync(string name);

        Task<IdentityRole> FindRoleById(string id);

        Task<IdentityResult> DeleteRole(IdentityRole role);

        IQueryable<IdentityRole> GetAllRoles();
        
    }
}
