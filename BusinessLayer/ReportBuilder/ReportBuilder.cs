using GemBox.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BusinessLayer.ReportBuilder
{
    public static class ReportBuilder
    {
        public static string ReportBuilding(IQueryable columns, IQueryable rows)
        {
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
            // создание файла excel
            var workBook = new ExcelFile();
            // название листа таблицы
            var workSheet = workBook.Worksheets.Add("Report");
            // создание таблицы
            var table = new DataTable();
            // задаются заголовки таблица
            foreach (var column in columns)
            {
                table.Columns.Add(column.ToString());
            }
            //заполнение таблица данными
            foreach (var item in rows)
            {
                //добавление данных разделено на два цикла для переноса строки в самой таблице
                var RowsList = new List<object> { };
                foreach (var row in item.GetType().GetProperties())
                {
                    // заполнение данных для одной строки
                    RowsList.Add(row.GetValue(item));
                }
                // добавление полученной строки в таблицу
                table.Rows.Add(RowsList.ToArray());
            }
            // добавление полученной таблицы в лист excel файла
            workSheet.InsertDataTable(table,
                new InsertDataTableOptions()
                {
                    ColumnHeaders = true,
                });
            //выравнивание ширины ячейки по содержимому
            var columnCount = workSheet.CalculateMaxUsedColumns(); // подсчет заголовков
            for (var i = 0; i < columnCount; i++)
                //расчет ширины ячейки для каждого заголовка в зависимости от содержимого
                workSheet.Columns[i].AutoFit(1, workSheet.Rows[1], workSheet.Rows[workSheet.Rows.Count - 1]);
            // генерация названия файла
            var filename = "Report";
            var path = "wwwroot/xlsxHolder/" + filename + ".xlsx";
            workBook.Save(path);
            return path;
        }
    }
}
