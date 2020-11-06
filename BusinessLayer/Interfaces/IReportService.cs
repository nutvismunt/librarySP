using GemBox.Spreadsheet;
using System;

namespace BusinessLayer.Interfaces
{
    public interface IReportService
    {
        // дата начала и конца формирования отчета
        ExcelFile ReportOrders(DateTime from,DateTime to);

        ExcelFile ReportBooks(DateTime from, DateTime to);

        ExcelFile ReportUsers(DateTime from, DateTime to);
    }
}
