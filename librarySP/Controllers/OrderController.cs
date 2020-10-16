using BusinessLayer.Interfaces;
using BusinessLayer.Models.OrderDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace librarySP.Controllers
{
    public class OrderController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;

        const string librarian = "Библиотекарь";
        const string user = "Пользователь";

        public OrderController(IBookService bookService, IOrderService orderService, IUserService userService)
        {
            _bookService = bookService;
            _orderService = orderService;
            _userService = userService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Order()
        {

            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Order(OrderViewModel order, long id, DateTime dateTime)
        {
            var user = await _userService.GetUser();
            order.UserId = user.Id;
            order.BookId = id;
            order.UserName = user.UserName;
            order.ClientNameSurName = user.Name + " " + user.Surname;
            order.ClientPhoneNum = user.PhoneNumber;
            order.OrderStatus = 0;
            var book = _bookService.GetBook(id);
            book.BookInStock -= order.Amount;
            order.BookName = book.BookName;
            order.BookAuthor = book.BookAuthor;
            order.OrderTime = dateTime;
            _orderService.Create(order);
            return RedirectToAction("OrderList");
        }

        [Authorize]
        public async Task<IActionResult> OrderList()
        {
            var user = await _userService.GetUser();
            var orders = _orderService.GetOrders().Where(c => c.UserId == user.Id).Where(c => c.OrderStatus == 0).AsNoTracking();
            return View(orders);
        }

        [Authorize(Roles = librarian)]
        public async Task<IActionResult> OrderAllList(string searchString, int search)
        {
            var orders = from b in _orderService.GetOrders() select b;
            if (!string.IsNullOrEmpty(searchString))
            {
                var orderSearcher=_orderService.SearchOrder(searchString);

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
            var book = _bookService.GetBook(bookIdHolder);
            if (book != null)
            {
                var order = _orderService.GetOrder(id);
                book.BookInStock += order.Amount;

                _orderService.Delete(order);
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
            const int b = 2;
            var order = _orderService.GetOrder(id);
            if (order !=null)
            {
                 order.OrderStatus = _orderService.Status("Given");

                _orderService.Update(order);
                if (User.IsInRole(librarian))
                { return RedirectToAction("OrderAllList"); }

            }
            return NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendOrder()
        {
            var user = await _userService.GetUser();
            var orders = await _orderService.GetOrders().Where(c => c.UserId == user.Id).Where(c => c.OrderStatus == 0).ToListAsync();
            foreach (var item in orders)
            {
                item.OrderStatus = _orderService.Status("Waiting");
                _orderService.Update(item);
            }
            return RedirectToAction("OrderList");
        }
    }
}
