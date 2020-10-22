using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLayer.Entities.Reports
{
    //отчет по заказам
    public class ReportOrders
    {
        public long Id { get; set; }
        public string UserNameSurname { get; set; }

        public string BookName { get; set; }

        public long ISBN { get; set; }

        public string OrderGiven { get; set; }

        public string OrderReturn { get; set; }

        public string BookAmount { get; set; }

    }
}
