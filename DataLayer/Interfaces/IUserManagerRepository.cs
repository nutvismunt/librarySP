using DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
    public interface IUserManagerRepository
    {
        Task<User> GetUser();
        UserManager<User> UserManagerEx();
        Task<IdentityResult> UpdateUser(User user);
        Task<IdentityResult> DeleteUser(User user);
        User GetUserById(string Id);
        IQueryable<User> GetUsers();
        Task<IdentityResult> CreateUser(User user, string userPassword);
        Task<IdentityResult> AddRole(User user, string role);
        Task SignIn(User user, bool b);
        Task SignOut();
        Task<SignInResult> PasswordSignIn(string email,string password, bool persistent,  bool lockBool);
    }
}
