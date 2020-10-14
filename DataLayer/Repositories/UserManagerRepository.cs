using DataLayer.Entities;
using DataLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class UserManagerRepository : IUserManagerRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContext;
        private readonly SignInManager<User> _signInManager;
        public UserManagerRepository (UserManager<User> userManager, SignInManager<User> signInManager, IHttpContextAccessor httpContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContext = httpContext;
        }

        public Task<IdentityResult> AddRole(User user, string role)
        {
            return _userManager.AddToRoleAsync(user, role);
        }

        public async Task<IdentityResult> CreateUser(User user, string userPassword)
        {
            return await _userManager.CreateAsync(user, userPassword);;
        }

        public Task<IdentityResult> DeleteUser(User user)
        {
            return _userManager.DeleteAsync(user); ;
        }

        public async Task<User> GetUser()
        {
            User user =await _userManager.GetUserAsync(_httpContext.HttpContext.User);

            return user;
        }

        public User GetUserById(string Id)
        {
            return _userManager.FindByIdAsync(Id).Result; 
        }

        public IQueryable<User> GetUsers()
        {
            var users = from u in _userManager.Users select u;
            return users;
        }

        public Task<SignInResult> PasswordSignIn(string email, string password, bool persistent, bool lockBool)
        {
            return _signInManager.PasswordSignInAsync(email,password,persistent, lockBool);
        }

        public Task SignIn(User user, bool b)
        {
            return _signInManager.SignInAsync(user, b);
        }

        public Task SignOut()
        {
            return _signInManager.SignOutAsync();
        }

        public Task<IdentityResult> UpdateUser(User user)
        {
            return _userManager.UpdateAsync(user);
        }

        public UserManager<User> UserManagerEx()
        {
            return _userManager;
        }
    }
}
