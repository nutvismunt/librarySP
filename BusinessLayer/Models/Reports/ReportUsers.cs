using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.Entities.Reports
{
    //отчет по пользователям
    public class ReportUsers
    {
        public long Id { get; set; }

        public string UserNameSurname { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Registered { get; set; }

        public string LastOrderDate { get; set; }

        public string TotalOrders { get; set; }
    }
}
