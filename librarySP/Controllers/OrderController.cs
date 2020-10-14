using DataLayer.Entities;
using DataLayer.enums;
using DataLayer.Interfaces;
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
        private readonly ISearchItem<Order> _searchOrder;
        private readonly IUserManagerRepository _getUser;

        const string librarian = "Библиотекарь";
        const string user = "Пользователь";

        public OrderController(IUnitOfWork unit, IRepository<Order> orderRep, IRepository<Book> bookRep, IRepository<User> userRep, IUserManagerRepository getUser, ISearchItem<Order> searchOrder)
        {
            _dbO = orderRep;
            _dbB = bookRep;
            _dbU = userRep;
            _unitOfWork = unit;
            _getUser = getUser;
            _searchOrder = searchOrder;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Order()
        {

            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Order(Order order, long id, DateTime dateTime)
        {
            var user =await _getUser.GetUser();
            order.UserId = user.Id;
            order.BookId = id;
            order.UserName = user.UserName;
            order.ClientNameSurName = user.Name + " " + user.Surname;
            order.ClientPhoneNum = user.PhoneNumber;
            order.OrderStatus = 0;
            Book book = _dbB.GetItem(id);
            book.BookInStock -= order.Amount;
            order.BookName = book.BookName;
            order.BookAuthor = book.BookAuthor;
            order.OrderTime = dateTime;
            _dbO.Create(order);
            _unitOfWork.Save();
            return RedirectToAction("OrderList");
        }

        [Authorize]
        public IActionResult OrderList()
        {
            var User = _getUser.GetUser().Result;
            var book = _dbO.GetItems().Where(c => c.UserId == User.Id).Where(c => c.OrderStatus == 0).AsNoTracking();
            return View(book);
        }

        [Authorize(Roles = librarian)]
        public async Task<IActionResult> OrderAllList(string searchString, int search)
        {
            var orders = from b in _dbO.GetItems() select b;
            if (!string.IsNullOrEmpty(searchString))
            {
                var orderSearcher=_searchOrder.Search(searchString);

                return View(orderSearcher);
            }
            else
            {
                return View(await orders.AsNoTracking().ToListAsync());
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
            var User = _getUser.GetUser().Result;
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
