﻿using BusinessLayer.Interfaces;
using BusinessLayer.ReportBuilder;
using DataLayer.Entities;
using DataLayer.Interfaces;
using GemBox.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace BusinessLayer.Services
{
    public class ReportService : IReportService
    {
        private readonly IRepository<Book> _bookService;
        private readonly IRepository<Order> _orderService;
        private readonly IRepository<User> _userService;
        private readonly ReportBuilder.ReportBuilder _reportBuilder;

        public ReportService(IRepository<Book> bookService, IRepository<Order> orderService, IRepository<User> userService,
            ReportBuilder.ReportBuilder reportBuilder)
        {
            _bookService = bookService;
            _orderService = orderService;
            _userService = userService;
            _reportBuilder = reportBuilder;
        }

        //отчет по заказам
        public ExcelFile ReportOrders(DateTime from, DateTime to)
        {
            DateTime date = new DateTime(2016);
            //заголовки указываются отдельно, чтобы не выводить название полей из класса сущности
            var column = new List<string> {
            "Id заказа","Клиент","Номер","Книга","ISBN",
            "Время заказа", "Заказ возвращен", "Количество", "Статус"
            };
            //выбирается какие поля сущности будут добавлены в отчет
            IQueryable<string> columns = column.AsQueryable();
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
            //вывод отчета между датами, если даты указаны
              return _reportBuilder.ReportBuilding(columns, orders.Where(c => c.OrderTime >= from && c.OrderTime <= to));
        }

        //отчет по книгам
        public ExcelFile ReportBooks(DateTime from, DateTime to)
        {
            DateTime date = new DateTime(2016);
            //заголовки указываются отдельно, чтобы не выводить название полей из класса сущности
            var column = new List<string> {
            "Id книги","Книга","ISBN","В наличии","Последний заказ",
            "Всего заказов", "Всего возвращено", "Дата добавления"
            };
            //выбирается какие поля сущности будут добавлены в отчет
            IQueryable<string> columns = column.AsQueryable();
            var books = _bookService.GetItems()
                .Select(c =>
                    new {
                        c.Id,
                        c.BookName,
                        c.ISBN,
                        c.BookInStock,
                        c.LastTimeOrdered,
                        c.TotalOrders,
                        c.TotalReturns,
                        c.WhenAdded
                    });
            //вывод отчета между датами, если даты указаны
              return  _reportBuilder.ReportBuilding(columns, books.Where(c => c.WhenAdded >= from && c.WhenAdded <= to));
        }

        //отчет по пользователям
        public ExcelFile ReportUsers(DateTime from, DateTime to)
        {
            DateTime date = new DateTime(2016);
            //заголовки указываются отдельно, чтобы не выводить название полей из класса сущности
            var column = new List<string> {
            "Имя","Фамилия","Почта","Номер","Дата добавления",
            "Всего заказано"
            };
            //выбирается какие поля сущности будут добавлены в отчет
            IQueryable<string> columns = column.AsQueryable();
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
            //вывод отчета между датами, если даты указаны
               return _reportBuilder.ReportBuilding(columns, users.Where(c => c.UserDate >= from && c.UserDate <= to));
        }
    }
}
