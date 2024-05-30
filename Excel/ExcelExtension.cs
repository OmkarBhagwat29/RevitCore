using ClosedXML.Excel;

namespace RevitCore.Excel
{
    public static class ExcelExtension
    {
        public static List<IXLWorksheet> GetWorksheets(this XLWorkbook _workbook) => _workbook.Worksheets.ToList();

        public static string ReadCell(this IXLWorksheet worksheet, int rowIndex, int ColumnIndex)
        {
            IXLCell cell = worksheet.Cell(rowIndex, ColumnIndex);

            if (cell.TryGetValue(out string data))
                return data;
            else
                return string.Empty;
        }
    }
}
