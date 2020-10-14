using BusinessLayer.Interfaces;
using DataLayer.Entities;
using DataLayer.enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace librarySP.Controllers
{
    public class OrderController : Controller
    {
        private readonly IRepository<Order> _dbO;
        private readonly IRepository<Book> _dbB;
        private readonly IRepository<User> _dbU;
        private readonly IUnitOfWork _unitOfWork;
        UserManager<User> _userManager;
        const string librarian = "Библиотекарь";
        const string user = "Пользователь";

        public OrderController(IUnitOfWork unit, IRepository<Order> orderRep, IRepository<Book> bookRep, IRepository<User> userRep, UserManager<User> userManager)
        {
            _dbO = orderRep;
            _dbB = bookRep;
            _dbU = userRep;
            _unitOfWork = unit;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Order(int? id)
        {
            if (id == null) return RedirectToAction("Index");
            ViewBag.BookId = id;
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Order(Order order, long id, DateTime dateTime)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            order.UserId = user.Id;
            order.BookId = id;
            order.UserName = user.UserName;
            order.ClientNameSurName = user.Name + " " + user.Surname;
            order.ClientPhoneNum = user.PhoneNum;
            order.OrderStatus = 0;
            Book book = _dbB.GetItem(id);
            book.BookInStock -= order.Amount;
            order.BookName = book.BookName;
            order.OrderTime = dateTime;
            _dbO.Create(order);
            _unitOfWork.Save();
            return RedirectToAction("OrderList");
        }

        [Authorize]
        public async Task<IActionResult> OrderList()
        {
            var User = await _userManager.GetUserAsync(HttpContext.User);
            var book = _dbO.GetItems().Include(c => c.Book).Where(c => c.UserId == User.Id).Where(c => c.OrderStatus == 0).AsNoTracking();
            return View(book);
        }

        [Authorize(Roles = librarian)]
        public async Task<IActionResult> OrderAllList(string searchString, int search)
        {
            var orders = from b in _dbO.GetItems() select b;
            if (!string.IsNullOrEmpty(searchString))
            {
                switch (search)
                {
                    case 0:
                        orders = orders.Where(s => s.OrderId.ToString().ToLower().Contains(searchString.ToLower()));
                        break;
                    case 1:
                        orders = orders.Where(s => s.BookId.ToString().ToLower().Contains(searchString.ToLower()));
                        break;
                    case 2:
                        orders = orders.Where(s => s.BookName.ToLower().Contains(searchString.ToLower()));
                        break;
                    case 3:
                        orders = orders.Where(s => s.UserName.ToLower().Contains(searchString.ToLower()));
                        break;
                    case 4:
                        orders = orders.Where(s => s.ClientPhoneNum.ToLower().Contains(searchString.ToLower()));
                        break;
                    case 5:
                        orders = orders.Where(s => s.ClientNameSurName.ToLower().Contains(searchString.ToLower()));
                        break;
                }
                return View(await orders.Include(c => c.Book).AsNoTracking().ToListAsync());
            }
            else
            {
                return View(await _dbO.GetItems().Include(c => c.Book).AsNoTracking().ToListAsync());
            }

        }


        [Authorize]
        [HttpPost]
        public IActionResult DeleteOrder(long id, long bookIdHolder)
        {
            if (id > 0)
            {

                Book bookHolder = _dbB.GetItem(bookIdHolder);
                Order orderHolder = _dbO.GetItem(id);
                bookHolder.BookInStock += orderHolder.Amount;

                _dbO.Delete(orderHolder);
                _unitOfWork.Save();
                if (User.IsInRole(user))
                { return RedirectToAction("OrderList"); }
                if (User.IsInRole(librarian))
                { return RedirectToAction("OrderAllList"); }

            }
            return NotFound();
        }

        [Authorize(Roles = librarian)]
        [HttpPost]
        public IActionResult GivingBook(long id)
        {
            if (id > 0)
            {
                Order orderHolder = _dbO.GetItem(id);
                orderHolder.OrderStatus = (OrderStatus)2;

                _unitOfWork.Save();
                if (User.IsInRole(librarian))
                { return RedirectToAction("OrderAllList"); }

            }
            return NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendOrder()
        {
            var User = await _userManager.GetUserAsync(HttpContext.User);
            var order = await _dbO.GetItems().Where(c => c.UserId == User.Id).Where(c => c.OrderStatus == 0).ToListAsync();
            foreach (var item in order)
            {
                item.OrderStatus = (OrderStatus)1;
            }
            _unitOfWork.Save();

            return RedirectToAction("OrderList");
        }
    }
}
