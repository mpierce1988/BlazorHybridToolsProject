using OfficeOpenXml;
using ResultSetInterpreter.Models.ExcelCSharp;
using ResultSetInterpreter.Services.Interfaces;
using ResultSetInterpreter.Services.Utilities;

namespace ResultSetInterpreter.Services;

public class EPPlusExcelCSharpWorkbookParser : IExcelCSharpWorkbookParser
{
    public async Task<ExcelCSharpWorkbook> ParseExcelCSharpWorkbookAsync(Stream excelFile)
    {
        var excelWorkbook = new ExcelCSharpWorkbook();

        await Task.Run(() =>
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using(var package = new ExcelPackage(excelFile))
            {
                foreach (var sheet in package.Workbook.Worksheets)
                {
                    var excelSheet = new ExcelCSharpSheet();
                    excelSheet.Name = sheet.Name;
                    excelSheet.Rows = new List<ExcelCSharpRow>();

                    List<ExcelCSharpColumnNameType> columnNameTypes = new();
                    int columnEnd = ExcelCSharpUtility.GetEndColumnIndex(sheet);

                    // loop through header row to get column names and types
                    for (int column = sheet.Dimension.Start.Column; column <= columnEnd; column++)
                    {
                        // Validate that there is a value in the cell. If not, break this loop
                        if (sheet.Cells[1, column] == null)
                        {
                            break;
                        }
                        
                        string columnName = sheet.Cells[1, column].Value.ToString() ?? sheet.Cells[1, column].ToString();
                        
                        var columnType = ExcelCSharpUtility.GetColumnType(sheet, column);

                        columnNameTypes.Add(new ExcelCSharpColumnNameType
                        {
                            ColumnName = columnName,
                            Type = columnType
                        });
                    }

                    int rowEnd = ExcelCSharpUtility.GetEndRowIndex(sheet);
                    // loop through rows to get values
                    for (int i = sheet.Dimension.Start.Row + 1; i <= rowEnd; i++)
                    {
                        var excelRow = new ExcelCSharpRow();
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
            }
        });

        return excelWorkbook;
    }
}