using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using BusinessLayer.Models.BookDTO;
using BusinessLayer.ReportBuilder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace librarySP.Controllers
{
    public class ReportController : Controller
    {

        private IReportService _reportService;

        public ReportController(IReportService reportService)
        {

            _reportService = reportService;

        }

        public ActionResult ReportBooks()
        {
            _reportService.ReportBooks();
            return RedirectToAction("Index");
        }

        public ActionResult ReportOrders()
        {
            _reportService.ReportOrders();
            return RedirectToAction("Index");
        }

        public ActionResult ReportUsers()
        {
            _reportService.ReportUsers();
            return RedirectToAction("Index");
        }


        public ActionResult Index()
        {
            return View();
        }
    }
}
