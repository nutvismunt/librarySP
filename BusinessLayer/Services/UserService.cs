using BusinessLayer.Interfaces;
using BusinessLayer.Models.UserDTO;
using DataLayer.Entities;
using DataLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContext;
        private readonly SignInManager<User> _signInManager;
        private readonly ISearchItem<User> _searchItem;
        private readonly ISortItem<User> _sortItem;

        public UserService (
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IHttpContextAccessor httpContext,
            RoleManager<IdentityRole> roleManager,
            ISearchItem<User> searchItem,
            ISortItem<User> sortItem)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContext = httpContext;
            _roleManager = roleManager;
            _searchItem = searchItem;
            _sortItem = sortItem;
        }

        public Task<IdentityResult> AddRole(User user, string role)
        {
            return _userManager.AddToRoleAsync(user, role);
        }

        public async Task<IdentityResult> CreateUser(User user, string userPassword)
        {
            return await _userManager.CreateAsync(user, userPassword);
        }

        public Task<IdentityResult> DeleteUser(User user)
        {
            return _userManager.DeleteAsync(user); ;
        }

        public async Task<User> GetUser()
        {
            return await _userManager.GetUserAsync(_httpContext.HttpContext.User);
        }

        public Task<User> GetUserById(string Id)
        {
            return _userManager.FindByIdAsync(Id); 
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

        public Task<IdentityResult> CreateRoleAsync(string name)
        {
            return _roleManager.CreateAsync(new IdentityRole(name));
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


        public Task<IdentityRole> FindRoleById(string id)
        {
            return _roleManager.FindByIdAsync(id);
        }

        public Task<IdentityResult> DeleteRole(IdentityRole role)
        {
            return _roleManager.DeleteAsync(role); 
        }

        public Task<IList<string>> GetRoles(User user)
        {
            return _userManager.GetRolesAsync(user);
        }

        public IQueryable<IdentityRole> GetAllRoles()
        {
            return _roleManager.Roles;
        }

        public Task<IdentityResult> AddToRoles(User user, IEnumerable<string> addedRoles)
        {
            return _userManager.AddToRolesAsync(user, addedRoles);
        }

        public Task<IdentityResult> RemoveFromRoles(User user, IEnumerable<string> removedRoles)
        {
            return _userManager.RemoveFromRolesAsync(user, removedRoles);
        }

        public List<User> SearchUser(string searchString)
        {
            return _searchItem.Search(searchString);
        }

        public IQueryable<User> SortUsers(string sort, bool asc = true)
        {
            return _sortItem.SortedItems(sort, asc);
        }


        public async Task<IdentityResult> UserValidator(EditUserViewModel model, User user)
        {
            IdentityResult result;

            if (model.NewPassword != null)
            {
                var _passwordValidator = _httpContext.HttpContext.RequestServices.GetService(typeof(IPasswordValidator<User>)) as IPasswordValidator<User>;
                result = await _passwordValidator.ValidateAsync(_userManager, user, model.NewPassword);
            }
            else
            {
                result = await _userManager.UpdateAsync(user);
            }
            return result;
        }

        public IPasswordHasher<User> UserHasher(EditUserViewModel model)
        {
            IPasswordHasher<User> _passwordHasher;

            if (model.NewPassword != null)
            {
                _passwordHasher =_httpContext.HttpContext.RequestServices.GetService(typeof(IPasswordHasher<User>)) as IPasswordHasher<User>;
            }
            else
            {
                _passwordHasher = null;
            }
            return  _passwordHasher;
        }
    }
}
