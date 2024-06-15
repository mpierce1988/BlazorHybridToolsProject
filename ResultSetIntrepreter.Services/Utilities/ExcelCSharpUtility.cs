using System.Globalization;
using OfficeOpenXml;
using ResultSetInterpreter.Models.ExcelCSharp;

namespace ResultSetInterpreter.Services.Utilities;

public static class ExcelCSharpUtility
{
    public static int GetEndRowIndex(ExcelWorksheet sheet)
    {
        // loop through the rows, checking the first column, for the last row with a value
        int rowEnd = 0;
        
        for (int row = sheet.Dimension.Start.Row; row <= sheet.Dimension.End.Row; row++)
        {
            if (sheet.Cells[row, 1].Value == null || sheet.Cells[row, 1].Value.ToString() == string.Empty)
            {
                break;
            }
            rowEnd = row;
        }

        return rowEnd;
    }

    public static int GetEndColumnIndex(ExcelWorksheet sheet)
    {
        // loop through the coluns to determine the last column with a value
        int columnEnd = 0;
        for (int column = sheet.Dimension.Start.Column; column <= sheet.Dimension.End.Column; column++)
        {
            if (sheet.Cells[1, column].Value == null || sheet.Cells[1, column].Value.ToString() == string.Empty)
            {
                break;
            }
            columnEnd = column;
        }

        return columnEnd;
    }

    public static string GetColumnType(ExcelWorksheet sheet, int column)
    {
        string columnType = "System.String?";// default to string
                        
        string? numberFormat = sheet.Cells[2, column].Style.Numberformat.Format;

        // if the number format is null, default to string
        switch (numberFormat ?? "@")
        {
            case "General":
                columnType = "System.Int32?";
                break;
            case "@":
                columnType = "System.String?";
                break;
            case "M/d/yyyy":
                columnType = "System.DateTime?";
                break;
        }

        return columnType;
    }

    public static DateTime ParseDate(string dateValue)
    {
        long num = long.Parse(dateValue);
        return DateTime.FromOADate(num); 
    }

    public static string ParseDateToString(ExcelCSharpValue value)
    {
        return ParseDate(value.Value?.ToString() ?? "0").ToString(CultureInfo.InvariantCulture);
    }
}