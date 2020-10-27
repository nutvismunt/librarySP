using System;

namespace BusinessLayer.Interfaces
{
    public interface IReportService
    {
        // дата начала и конца формирования отчета
        void ReportOrders(DateTime from,DateTime to);

        void ReportBooks(DateTime from, DateTime to);

        void ReportUsers(DateTime from, DateTime to);
    }
}
