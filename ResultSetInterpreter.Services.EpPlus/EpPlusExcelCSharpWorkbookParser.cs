using OfficeOpenXml;
using ResultSetInterpreter.Models.ExcelCSharp;
using ResultSetInterpreter.Services.Interfaces;

namespace ResultSetInterpreter.Services.EpPlus;

public class EpPlusExcelCSharpWorkbookParser : IExcelCSharpWorkbookParser
{
    public async Task<ExcelCSharpWorkbook> ParseExcelCSharpWorkbookAsync(Stream excelFile)
    {
        var excelWorkbook = new ExcelCSharpWorkbook();

        await Task.Run(() =>
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(excelFile);
            
            foreach (var sheet in package.Workbook.Worksheets)
            {
                var excelSheet = new ExcelCSharpSheet();
                excelSheet.Name = sheet.Name;
                excelSheet.Rows = new List<ExcelCSharpRow>();

                List<ExcelCSharpColumnNameType> columnNameTypes = new();
                int columnEnd = GetEndColumnIndex(sheet);

                // loop through header row to get column names and types
                for (int column = sheet.Dimension.Start.Column; column <= columnEnd; column++)
                {
                    string columnName = sheet.Cells[1, column].Value.ToString() ?? sheet.Cells[1, column].ToString();
                        
                    var columnType = GetColumnType(sheet, column);

                    columnNameTypes.Add(new ExcelCSharpColumnNameType
                    {
                        ColumnName = columnName,
                        Type = columnType
                    });
                }

                int rowEnd = GetEndRowIndex(sheet);
                
                // loop through rows to get values
                for (int i = sheet.Dimension.Start.Row + 1; i <= rowEnd; i++)
                {
                    ExcelCSharpRow excelRow = new();
                    excelRow.RowNumber = i;
                    excelRow.Values = new List<ExcelCSharpValue>();

                    for (int j = sheet.Dimension.Start.Column; j <= columnEnd; j++)
                    {
                        var cellValue = sheet.Cells[i, j].Value;
                        var columnNameType = columnNameTypes[j - 1];
                            
                        excelRow.Values.Add(new ExcelCSharpValue
                        {
                            CSharpColumnNameType = columnNameType,
                            Value = cellValue
                        });
                    }

                    excelSheet.Rows.Add(excelRow);
                }
                    
                // Attach sheet to workbook
                excelWorkbook.Sheets.Add(excelSheet);
            }
        });

        return excelWorkbook;
    }
    
    private int GetEndRowIndex(ExcelWorksheet sheet)
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

    private int GetEndColumnIndex(ExcelWorksheet sheet)
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

    private string GetColumnType(ExcelWorksheet sheet, int column)
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
}