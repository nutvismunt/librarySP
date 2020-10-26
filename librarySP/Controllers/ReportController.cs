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


            return View();
        }
    }
}
