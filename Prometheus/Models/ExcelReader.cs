using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using Excel = Microsoft.Office.Interop.Excel;

namespace Domino.Models
{
    public class ExcelReader
    {
        private static Excel.Workbook OpenBook(Excel.Workbooks books, string fileName, bool readOnly, bool editable,
bool updateLinks)
        {
            
            Excel.Workbook book = books.Open(
                fileName, updateLinks, readOnly,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, editable, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing);
            return book;
        }

        private static void ReleaseRCM(object o)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(o);
            }
            catch
            {
            }
            finally
            {
                o = null;
            }
        }

        private static bool WholeLineEmpty(List<string> line)
        {
            bool ret = true;
            foreach (var item in line)
            {
                if (!string.IsNullOrEmpty(item.Trim()))
                {
                    ret = false;
                }
            }
            return ret;
        }

        public static List<List<string>> RetrieveDataFromExcel(string wholefn,string sheetname)
        {
            var data = new List<List<string>>();
            Excel.Application excel = null;
            Excel.Workbook wkb = null;
            Excel.Workbooks books = null;

            try
            {
                
                excel = new Excel.Application();
                excel.DisplayAlerts = false;
                books = excel.Workbooks;
                wkb = OpenBook(books, wholefn, true, false, false);

                Excel.Worksheet sheet = wkb.Sheets[sheetname] as Excel.Worksheet;

                var excelRange = sheet.UsedRange;
                object[,] valueArray = (object[,])excelRange.get_Value(
                Excel.XlRangeValueDataType.xlRangeValueDefault);
                var totalrows = sheet.UsedRange.Rows.Count;
                var totalcols = sheet.UsedRange.Columns.Count;

                for (int row = 1; row <= totalrows; ++row)
                {
                    var line = new List<string>();
                    for (int col = 1; col <= totalcols; ++col)
                    {
                        if (valueArray[row, col] == null)
                        {
                            line.Add(string.Empty);
                        }
                        else
                        {
                            line.Add(valueArray[row, col].ToString().Trim().Replace("'",""));
                        }
                    }
                    //if (!WholeLineEmpty(line))
                    //{
                        data.Add(line);
                    //}
                }

                
                wkb.Close();
                excel.Quit();

                Marshal.ReleaseComObject(sheet);
                Marshal.ReleaseComObject(wkb);
                Marshal.ReleaseComObject(books);
                Marshal.ReleaseComObject(excel);

                return data;
            }
            catch (Exception ex)
            {
                data.Clear();
                return data;
            }
            finally
            {
                if (wkb != null)
                    ReleaseRCM(wkb);
                if (excel != null)
                    ReleaseRCM(excel);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}