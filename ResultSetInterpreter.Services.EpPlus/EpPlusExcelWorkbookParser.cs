using OfficeOpenXml;
using ResultSetInterpreter.Models.Workbook;
using ResultSetIntrepreter.Services;

namespace ResultSetInterpreter.Services.EpPlus;

public class EpPlusExcelWorkbookParser : IExcelWorkbookParser
{
    #region Public Methods
    
    /// <summary>
    /// Parses an Excel file into a Workbook object
    /// </summary>
    /// <param name="stream">
    /// The stream of the Excel file to parse
    /// </param>
    /// <returns>
    /// The parsed Workbook object
    /// </returns>
    public async Task<Workbook> ParseExcel(Stream stream)
    {
        Workbook workbook = new();
        
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        
        using var package = new ExcelPackage(stream);
        
        foreach (var worksheet in package.Workbook.Worksheets)
        {
            // Ensure the worksheet is not empty
            if (worksheet.Dimension.Columns == 0 && worksheet.Dimension.Rows == 0)
            {
                continue;
            }
            
            var sheet = GetSheetFromWorksheet(worksheet);

            workbook.Sheets.Add(sheet);
        }
        
        return workbook;
    }
    
    #endregion
    
    #region Private Methods

    /// <summary>
    /// Converts an EpPlus ExcelWorksheet to a Sheet object
    /// </summary>
    /// <param name="worksheet">
    /// The worksheet to convert
    /// </param>
    /// <returns>
    /// The converted Sheet object
    /// </returns>
    private Sheet GetSheetFromWorksheet(ExcelWorksheet worksheet)
    {
        Sheet sheet = new()
        {
            Name = worksheet.Name
        };
            
        // object[,] cells = new object[worksheet.Dimension.Rows, worksheet.Dimension.Columns];
        sheet.Cells = new object[worksheet.Dimension.Rows, worksheet.Dimension.Columns];
            
        for (int i = 1; i <= worksheet.Dimension.Rows; i++)
        {
            for (int j = 1; j <= worksheet.Dimension.Columns; j++)
            {
                sheet.Cells[i - 1, j - 1] = worksheet.Cells[i, j].Value;
            }
        }

        //sheet.Cells = cells;
        return sheet;
    }
    
    #endregion
}