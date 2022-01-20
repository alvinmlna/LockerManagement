using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace LockerManagementSystem.App_Class.Helpers
{
    public class ExportToExcelHelper
    {
        public void ExportToExcel<T>(List<T> model, string filename = "Exported Data")
        {
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Exported Data");
            workSheet.DefaultRowHeight = 12;
            //Header of table  
            // 

            var header = typeof(T).GetProperties()
                        .Select(property => property.Name)
                        .ToArray();

            for (int i = 0; i <= header.Count() - 1; i++)
            {
                var rowColumn = i + 1;
                workSheet.Cells[1, rowColumn].Value = header[i];
                workSheet.Cells[1, rowColumn].Style.Font.Bold = true;
                workSheet.Cells[1, rowColumn].Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                workSheet.Cells[1, rowColumn].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
                workSheet.Cells[1, rowColumn].Style.Font.Color.SetColor(Color.White);
            }

            int recordIndex = 2;
            foreach (var gmds in model)
            {
                var valueOfEntity = gmds.GetType().GetProperties().Select(x => x.GetValue(gmds, null)).ToArray();

                for (int i = 0; i <= valueOfEntity.Count() - 1; i++)
                {
                    var rowColumn = i + 1;

                    if (valueOfEntity[i] != null)
                    {
                        DateTime dateTime;
                        string value = "";

                        value = valueOfEntity[i].ToString();

                         //var datetime = DateTime.ParseExact(value, "dd/MM/yyyy", null);

                        if (DateTime.TryParse(value,
                            System.Globalization.CultureInfo.GetCultureInfo("id-ID"), 
                            System.Globalization.DateTimeStyles.None, out dateTime))
                        {
                            DateTime v = DateTime.Parse(value, new CultureInfo("id-ID"));

                            workSheet.Cells[recordIndex, rowColumn].Style.Numberformat.Format = "dd-mm-yyy";
                            workSheet.Cells[recordIndex, rowColumn].Value = GetExcelDecimalValueForDate(v);
                        }
                        else
                        {
                            workSheet.Cells[recordIndex, rowColumn].Value = value;
                        }
                    }
                }
                recordIndex++;
            }
            string excelName = filename + "_" + DateTime.Now.ToShortDateString();


            using (var memoryStream = new MemoryStream())
            {
                HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + excelName + ".xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();
            }
        }


        public static void Export<T>(List<T> model, string filename = "Exported Data")
        {
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Exported Data");
            workSheet.DefaultRowHeight = 12;
            //Header of table  
            // 

            var header = typeof(T).GetProperties()
                        .Select(property => property.Name)
                        .ToArray();

            for (int i = 0; i <= header.Count() - 1; i++)
            {
                var rowColumn = i + 1;
                workSheet.Cells[1, rowColumn].Value = header[i];
                workSheet.Cells[1, rowColumn].Style.Font.Bold = true;
                workSheet.Cells[1, rowColumn].Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                workSheet.Cells[1, rowColumn].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
                workSheet.Cells[1, rowColumn].Style.Font.Color.SetColor(Color.White);
            }

            int recordIndex = 2;
            foreach (var gmds in model)
            {
                var valueOfEntity = gmds.GetType().GetProperties().Select(x => x.GetValue(gmds, null)).ToArray();

                for (int i = 0; i <= valueOfEntity.Count() - 1; i++)
                {
                    var rowColumn = i + 1;

                    if (valueOfEntity[i] != null)
                    {
                        DateTime dateTime;
                        string value = "";

                        value = valueOfEntity[i].ToString();

                        if (DateTime.TryParse(value, out dateTime))
                        {
                            DateTime v = DateTime.Parse(value, new CultureInfo("en-US"));

                            workSheet.Cells[recordIndex, rowColumn].Style.Numberformat.Format = "dd-mm-yyy";
                            workSheet.Cells[recordIndex, rowColumn].Value = GetExcelDecimalValueForDate(v);
                        }
                        else
                        {
                            workSheet.Cells[recordIndex, rowColumn].Value = value;
                        }
                    }
                }
                recordIndex++;
            }
            string excelName = filename + "_" + DateTime.Now.ToShortDateString();


            using (var memoryStream = new MemoryStream())
            {
                HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + excelName + ".xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();
            }
        }



        public static decimal GetExcelDecimalValueForDate(DateTime date)
        {
            DateTime start = new DateTime(1900, 1, 1);
            TimeSpan diff = date - start;
            return diff.Days + 2;
        }
    }

}