using BusinessLayer.Interfaces;
using BusinessLayer.Models.OrderDTO;
using BusinessLayer.Models.UserDTO;
using BusinessLayer.Services;
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

            order.ISBN = book.ISBN;
            order.UserId = user.Id;
            order.UserName = user.UserName;
            order.ClientNameSurName = user.Name + " " + user.Surname;
            order.ClientPhoneNum = user.PhoneNumber;
            order.OrderStatus = 0;
            order.BookName = book.BookName;
            order.BookAuthor = book.BookAuthor;
            order.OrderTime = DateTime.Now;
            book.BookInStock -= order.Amount;
            book.LastTimeOrdered = DateTime.Now;
            book.TotalOrders += order.Amount;
            
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
        public IActionResult OrderAllList(string searchString, string boolSort, string sortOrder)
        {
            var orders = from b in _orderService.GetOrders() select b;

            ViewBag.NameSort = boolSort == "false" ? "true" : "false";
            ViewBag.AuthorSort = boolSort == "false" ? "true" : "false";
            if (!string.IsNullOrEmpty(ViewBag.NameSort))
            {
                sortOrder = "BookName";
            }
            if (!string.IsNullOrEmpty(ViewBag.AuthorSort))
            {
                sortOrder = "BookAuthor";
            }

            var o = Convert.ToBoolean(boolSort);
            var book = _bookService.GetBooks().Where(c => c.BookInStock > 0);
        
            if (!string.IsNullOrEmpty(boolSort))
            {
                var orderSorter = _orderService.SortOrders(sortOrder, o);
                if (orderSorter != null)
                    return View(orderSorter);
            }

            else if (!string.IsNullOrEmpty(searchString))
            {
                var orderSearcher = _orderService.SearchOrder(searchString);
                if (orderSearcher != null)
                    return View(orderSearcher);

            }

                return View(orders.AsNoTracking().ToList());


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
        public IActionResult FinishOrder(long id, long bookIdHolder)
        {
            var book = _bookService.GetBook(bookIdHolder);
            if (book != null)
            {
                var order = _orderService.GetOrder(id);
                if (order != null)
                {
                    book.BookInStock += order.Amount;
                    order.OrderStatus = _orderService.Status("Completed");
                    order.OrderReturned = DateTime.Now;
                    book.TotalReturns += order.Amount;
                    _bookService.Update(book);
                    _orderService.Update(order);

                    if (User.IsInRole(librarianRole))
                    { return RedirectToAction("OrderAllList"); }
                }

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
            var user = _userService.GetUser().Result;
            var orders = _orderService.GetOrders()
                .Where(c => c.UserId == user.Id)
                .Where(c => c.OrderStatus == 0)
                .AsNoTracking();
            var b = 0;
            foreach (var item in orders)
            {
                item.OrderStatus = _orderService.Status("Waiting");
                _orderService.Update(item);
                b++;
            }
            user.LasOrder = DateTime.Now;
            user.TotalOrders += b;
            _userService.UpdateUser(user);
            return RedirectToAction("OrderList");
        }
    }
}
