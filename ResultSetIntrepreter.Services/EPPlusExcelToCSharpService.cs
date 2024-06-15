using System.Globalization;
using System.Text;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ResultSetInterpreter.Services.Interfaces;

namespace ResultSetInterpreter.Services;

public class EPPlusExcelToCSharpService : IExcelToCSharpService
{
    public async Task<String> ConvertExcelToCSharpAsync(Stream excelStream)
    {
        var excelWorkbook = await GetSheetValues(excelStream);
        var codeBuilder = new StringBuilder();

        foreach (var sheet in excelWorkbook.Sheets)
        {
            // Ensure there is at least one row of data (not the header)
            if (sheet.Rows.Count < 2)
            {
                continue;
            }
            
            codeBuilder.AppendLine($"public class {sheet.Name}");
            codeBuilder.AppendLine("{");

            
            foreach (var value in sheet.Rows[1].Values)
            {
                codeBuilder.AppendLine($"   public {value.CSharpColumnNameType.Type} {value.CSharpColumnNameType.ColumnName} {{ get; set; }}");
            }

            codeBuilder.AppendLine("}");
            codeBuilder.AppendLine();
            codeBuilder.AppendLine($"public List<{sheet.Name}> {sheet.Name}List = new List<{sheet.Name}>() {{");

            foreach (var row in sheet.Rows)
            {
                codeBuilder.AppendLine($"   new {sheet.Name} {{");
                foreach (var value in row.Values)
                {
                    if (value.CSharpColumnNameType.Type == "System.DateTime?")
                    {
                        codeBuilder.AppendLine($"       {value.CSharpColumnNameType.ColumnName} = DateTime.Parse(\"{ParseDateToString(value)}\", CultureInfo.InvariantCulture),");
                    }
                    else if (value.CSharpColumnNameType.Type == "System.Int32?")
                    {
                        codeBuilder.AppendLine($"       {value.CSharpColumnNameType.ColumnName} = {int.Parse(value.Value?.ToString() ?? "0")},");
                    }
                    else
                    {
                        codeBuilder.AppendLine($"       {value.CSharpColumnNameType.ColumnName} = \"{value.Value?.ToString()}\",");
                    }
                    
                }

                // remove last trailing comma
                int propertyLastCommaIndex = codeBuilder.ToString().LastIndexOf(',');
                codeBuilder = codeBuilder.Remove(propertyLastCommaIndex, 1);
                codeBuilder.AppendLine("    },");
            }
            // remove last trailing comma
            int sheetLastCommaIndex = codeBuilder.ToString().LastIndexOf(',');
            codeBuilder = codeBuilder.Remove(sheetLastCommaIndex, 1);
            codeBuilder.AppendLine("};");
        }

        return codeBuilder.ToString();
    }
    
    private async Task<ExcelCSharpWorkbook> GetSheetValues(Stream excelFile)
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
                    int columnEnd = DetermineColumnEnd(sheet);

                    // loop through header row to get column names and types
                    for (int column = sheet.Dimension.Start.Column; column <= columnEnd; column++)
                    {
                        // Validate that there is a value in the cell. If not, break this loop
                        if (sheet.Cells[1, column] == null)
                        {
                            break;
                        }
                        
                        string columnName = sheet.Cells[1, column].Value.ToString() ?? sheet.Cells[1, column].ToString();
                        
                        var columnType = DetermineColumnType(sheet, column);

                        columnNameTypes.Add(new ExcelCSharpColumnNameType
                        {
                            ColumnName = columnName,
                            Type = columnType
                        });
                    }

                    int rowEnd = DetermineRowEnd(sheet);
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

    private int DetermineRowEnd(ExcelWorksheet sheet)
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

    private int DetermineColumnEnd(ExcelWorksheet sheet)
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

    private string DetermineColumnType(ExcelWorksheet sheet, int column)
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

    private DateTime ParseDate(string dateValue)
    {
        long num = long.Parse(dateValue);
        return DateTime.FromOADate(num); 
    }

    private string ParseDateToString(ExcelCSharpValue value)
    {
        return ParseDate(value.Value?.ToString() ?? "0").ToString(CultureInfo.InvariantCulture);
    }

    public class ExcelCSharpWorkbook
    {
        public List<ExcelCSharpSheet> Sheets { get; set; } = new();
    }
    
    
    public class ExcelCSharpSheet
    {
        public string Name { get; set; } = string.Empty;
        public List<ExcelCSharpRow> Rows { get; set; } = new();
        
    }

    public class ExcelCSharpRow
    {
        public int RowNumber { get; set; } = -1;
        public List<ExcelCSharpValue> Values { get; set; } = new();
    }

    public class ExcelCSharpValue
    {
        public ExcelCSharpColumnNameType CSharpColumnNameType { get; set; } = new();
        public object? Value { get; set; }
    }

    public class ExcelCSharpColumnNameType
    {
        public string ColumnName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}



