using System;

namespace BusinessLayer.Interfaces
{
    public interface IReportService
    {
        // дата начала и конца формирования отчета
        string ReportOrders(DateTime from,DateTime to);

        string ReportBooks(DateTime from, DateTime to);

        string ReportUsers(DateTime from, DateTime to);
    }
}
