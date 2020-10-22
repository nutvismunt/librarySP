using DataLayer.Entities;
using GemBox.Spreadsheet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ReportBuilder
{
   
    public static class ReportBuilder
    {
        public static void ReportBuilding (IQueryable query)
        {
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
            var workBook = new ExcelFile();
            var workSheet = workBook.Worksheets.Add("Report");
            
            DataTable table = new DataTable();

           // var query = from b in query select b;

            var pol = query.AsQueryable().First();

            foreach (var column in pol.GetType().GetProperties())
            {
              //  var p = column.GetValue(pol).GetType();
                table.Columns.Add(column.Name);
              
                
            }
            
            foreach (var item in query)
            {
                var RowsList = new List<object> { };
                foreach (var row in item.GetType().GetProperties())
                {
                    RowsList.Add(row.GetValue(item));
                }
                table.Rows.Add(RowsList.ToArray());
            }

            workSheet.InsertDataTable(table,
                new InsertDataTableOptions()
                {
                    ColumnHeaders = true
                });
            var filename = Guid.NewGuid();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop).Replace("\\","/");
            workBook.Save(path+"/"+filename+".xlsx");
        }
    }
}
