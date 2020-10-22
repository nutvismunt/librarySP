using BusinessLayer.Interfaces;
using BusinessLayer.Models.OrderDTO;
using BusinessLayer.ReportBuilder;
using DataLayer.Entities;
using DataLayer.Entities.Reports;
using DataLayer.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace BusinessLayer.Services
{
    public class ReportService : IReportService 
    {
        private readonly IRepository<Book> _bookService;
        private readonly IRepository<Order> _orderService;
        private readonly IRepository<User> _userService;


        public ReportService (IRepository<Book> bookService, IRepository<Order> orderService, IRepository<User> userService)
        {
            _bookService = bookService;
            _orderService = orderService;
            _userService = userService;

        }

        public void ReportOrders()
        {
            var orders = _orderService.GetItems()
                .Select(c =>
                new {
                    c.Id,
                    c.ClientNameSurName,
                    c.ClientPhoneNum,
                    c.BookName,
                    c.ISBN,
                    c.OrderTime,
                    c.OrderReturned,
                    c.Amount,
                    c.OrderStatus
                });
           ReportBuilder.ReportBuilder.ReportBuilding(orders);
        }

        public void ReportBooks()
        {
            var books = _bookService.GetItems()
                .Select(c =>
                    new {
                        c.Id,
                        c.BookName,
                        c.ISBN,
                        c.BookInStock,
                        c.LastTimeOrdered,
                        c.TotalOrders,
                        c.TotalReturns
                    });
            ReportBuilder.ReportBuilder.ReportBuilding(books);
        }

        public void ReportUsers()
        {
            var users = _userService.GetItems()
                .Select(c =>
                new
                {
                    c.Name,
                    c.Surname,
                    c.UserName,
                    c.PhoneNumber,
                    c.UserDate,
                    c.TotalOrders

                });
            ReportBuilder.ReportBuilder.ReportBuilding(users);
        }
    }
}
