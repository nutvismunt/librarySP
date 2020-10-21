using BusinessLayer.Interfaces;
using BusinessLayer.Models.OrderDTO;
using BusinessLayer.Models.UserDTO;
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

        const string librarianRole = "Библиотекарь";
        const string userRole = "Пользователь";

        public OrderController(IBookService bookService, IOrderService orderService,
                               IUserService userService)
        {
            _bookService = bookService;
            _orderService = orderService;
            _userService = userService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Order(long bookId)
        {
            ViewBag.Data = bookId;
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Order(OrderViewModel order, long bookId)
        {
            var book = _bookService.GetBook(bookId);
      
            var user = await _userService.GetUser();

            order.UserId = user.Id;
            order.UserName = user.UserName;
            order.ClientNameSurName = user.Name + " " + user.Surname;
            order.ClientPhoneNum = user.PhoneNumber;
            order.OrderStatus = 0;
            order.BookName = book.BookName;
            order.BookAuthor = book.BookAuthor;
            order.OrderTime = DateTime.Now;
            book.BookInStock -= order.Amount;
            if (book.BookInStock > 0)
            {
                _orderService.Create(order);
            _bookService.Update(book);
            }
            if (User.IsInRole(userRole))
                return RedirectToAction("OrderList");
            else return RedirectToAction("OrderAllList");
        }

        [Authorize]
        public async Task<IActionResult> OrderList()
        {
            var user = await _userService.GetUser();
            var orders = _orderService.GetOrders()
                .Where(c => c.UserId == user.Id)
                .Where(c => c.OrderStatus == 0)
                .AsNoTracking();
            return View(orders);
        }

        [Authorize(Roles = librarianRole)]
        public IActionResult OrderAllList(string searchString, int search)
        {
            var orders = from b in _orderService.GetOrders() select b;
            if (!string.IsNullOrEmpty(searchString))
            {
                var orderSearcher = _orderService.SearchOrder(searchString);

                return View(orderSearcher);
            }
            else
            {
                return View(orders.AsNoTracking().ToList());
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
                _bookService.Update(book);
                if (User.IsInRole(userRole))
                { return RedirectToAction("OrderList"); }
                if (User.IsInRole(librarianRole))
                { return RedirectToAction("OrderAllList"); }

            }
            return NotFound();
        }

        [Authorize(Roles = librarianRole)]
        [HttpPost]
        public IActionResult GivingBook(long id)
        {
            var order = _orderService.GetOrder(id);
            if (order !=null)
            {
                 order.OrderStatus = _orderService.Status("Given");
                _orderService.Update(order);
                if (User.IsInRole(librarianRole))
                { return RedirectToAction("OrderAllList"); }

            }
            return NotFound();
        }

        [Authorize]
        [HttpPost]
        public IActionResult SendOrder()
        {
            var user = _userService.GetUser().Result.Id;
            var orders = _orderService.GetOrders()
                .Where(c => c.UserId == user)
                .Where(c => c.OrderStatus == 0)
                .AsNoTracking();
            foreach (var item in orders)
            {
                item.OrderStatus = _orderService.Status("Waiting");
                _orderService.Update(item);
            }
            return RedirectToAction("OrderList");
        }
    }
}
