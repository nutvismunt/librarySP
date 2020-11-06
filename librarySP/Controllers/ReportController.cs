using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using GemBox.Spreadsheet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace librarySP.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService) => _reportService = reportService;

        public ActionResult Index(DateTime from, DateTime to, string entity)
        {
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
            var file=new ExcelFile();
            byte[] fileContents;
            var options = SaveOptions.XlsxDefault;
            if (from < to && entity != null)
            {
                switch (entity)
                {
                    case "books":
                        file = _reportService.ReportBooks(from, to);
                        break;
                    case "orders":
                        file = _reportService.ReportOrders(from, to);
                        break;
                    case "users":
                        file = _reportService.ReportUsers(from, to);
                        break;
                }
                using (var stream = new MemoryStream())
                {
                    file.Save(stream, options);
                    fileContents = stream.ToArray();
                }
                return File(fileContents, options.ContentType, "Report.xlsx");
            }
            else if (from == DateTime.MinValue && to == DateTime.MinValue && entity != null)
            {
                switch (entity)
                {
                    case "books":
                        file = _reportService.ReportBooks(DateTime.MinValue, DateTime.MaxValue);
                        break;
                    case "orders":
                        file = _reportService.ReportOrders(DateTime.MinValue, DateTime.MaxValue);
                        break;
                    case "users":
                        file = _reportService.ReportUsers(DateTime.MinValue, DateTime.MaxValue);
                        break;
                }
                using (var stream = new MemoryStream())
                {
                    file.Save(stream, options);
                    fileContents = stream.ToArray();
                }
                return File(fileContents, options.ContentType, "Report.xlsx");
            }
            else if (from > to && entity != null)
            {
                ViewBag.DateError = "Некорректный диапазон дат";
            }
            return View();
        }
    }
}
