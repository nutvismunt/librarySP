using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.Entities.Reports
{
    //отчет по книгам
    public class ReportBooks
    {
        public long Id { get; set; }

        public string BookName { get; set; }

        public string ISBN { get; set; }

        public string BookInStock { get; set; }

        public string LastOrderDate{ get; set; }

        public string AllGivenBooks { get; set; }

        public string AllReturnBooks { get; set; }
    }
}
