using System;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace librarySP.Controllers
{
    public class ReportController : Controller
    {
        private IReportService _reportService;

        public ReportController(IReportService reportService) => _reportService = reportService;

        public ActionResult Index(DateTime from, DateTime to, string entity)
        {
            if (from < to && entity!=null)
            {
                switch (entity)
                {
                    case "books":
                        _reportService.ReportBooks(from,to);
                        break;
                    case "orders":
                        _reportService.ReportOrders(from,to);
                        break;
                    case "users":
                        _reportService.ReportUsers(from,to);
                        break;
                }
            }
            else if(from > to && entity != null)
            {
                ViewBag.DateError = "Некорректный диапазон дат";
                return View();
            };
            return View();
        }
    }
}
