using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Interfaces
{
    public interface IReportService
    {
        // form reportUser
        //form reportOrder
        //form reportBook

        void ReportOrders();

        void ReportBooks();

        void ReportUsers();
    }
}
