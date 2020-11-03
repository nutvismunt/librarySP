using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace librarySP.Controllers
{
    public class ReportController : Controller
    {
        private IReportService _reportService;

        public ReportController(IReportService reportService) => _reportService = reportService;

        public ActionResult Index(DateTime from, DateTime to, string entity)
        {
            string path = "";
            var root = "wwwroot";
            if (from < to && entity != null)
            {
                switch (entity)
                {
                    case "books":
                        path = _reportService.ReportBooks(from, to);
                        break;
                    case "orders":
                        path = _reportService.ReportOrders(from, to);
                        break;
                    case "users":
                        path = _reportService.ReportUsers(from, to);
                        break;
                }
                return File(path.Replace(root, "~"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            else if (from == DateTime.MinValue && to == DateTime.MinValue && entity != null)
            {
                switch (entity)
                {
                    case "books":
                        path = _reportService.ReportBooks(DateTime.MinValue, DateTime.MaxValue);
                        break;
                    case "orders":
                        path = _reportService.ReportOrders(DateTime.MinValue, DateTime.MaxValue);
                        break;
                    case "users":
                        path = _reportService.ReportUsers(DateTime.MinValue, DateTime.MaxValue);
                        break;
                }
                return File(path.Replace(root, "~"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            else if (from > to && entity != null)
            {
                ViewBag.DateError = "Некорректный диапазон дат";
            }
            return View();
        }
    }
}
