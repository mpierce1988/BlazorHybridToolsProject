using OfficeOpenXml;
using ResultSetInterpreter.Models.Workbook;
using ResultSetIntrepreter.Services;

namespace ResultSetInterpreter.Services.EpPlus;

public class EpPlusExcelWorkbookParser : IExcelWorkbookParser
{
    public async Task<Workbook> ParseExcel(Stream stream)
    {
        Workbook workbook = new();
        
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        
        using var package = new ExcelPackage(stream);
        
        foreach (var worksheet in package.Workbook.Worksheets)
        {
            // Ensure the worksheet is not empty
            if (worksheet.Dimension.Columns == null && worksheet.Dimension.Rows == null)
            {
                continue;
            }
            
            Sheet sheet = new()
            {
                Name = worksheet.Name
            };
            
            object[,] cells = new object[worksheet.Dimension.Rows, worksheet.Dimension.Columns];
            
            for (int i = 1; i <= worksheet.Dimension.Rows; i++)
            {
                for (int j = 1; j <= worksheet.Dimension.Columns; j++)
                {
                    cells[i - 1, j - 1] = worksheet.Cells[i, j].Value;
                }
            }

            sheet.Cells = cells;
            
            workbook.Sheets.Add(sheet);
        }
        
        return workbook;
    }
}