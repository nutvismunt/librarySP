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

        void ReportOrders(DateTime from,DateTime to);

        void ReportBooks(DateTime from, DateTime to);

        void ReportUsers(DateTime from, DateTime to);
    }
}
