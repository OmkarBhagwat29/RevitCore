using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitCore.Excel
{
    public class ExcelHelper
    {
        private readonly XLWorkbook _excelWorkbook;
        public ExcelHelper(string _excelFilePath)
        {
            try
            {
                _excelWorkbook = new XLWorkbook(_excelFilePath);
            }
            catch (Exception)
            {

            }
        }

        public XLWorkbook GetWorkbook() => _excelWorkbook;

        

        public IXLWorksheet GetWorksheet(string sheetName) => _excelWorkbook.Worksheet(sheetName);

       
    }
}
