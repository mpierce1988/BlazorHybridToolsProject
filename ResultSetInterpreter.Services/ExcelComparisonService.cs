using ResultSetInterpreter.Services.Interfaces;

namespace ResultSetIntrepreter.Services;

public class ExcelComparisonService : IExcelComparisonService
{
    private readonly IExcelWorkbookParser _excelWorkbookParser;
    
    public ExcelComparisonService(IExcelWorkbookParser excelWorkbookParser)
    {
        _excelWorkbookParser = excelWorkbookParser;
    }

    public async Task<ExcelComparisionResult> CompareExcelFilesAsync(Stream controlValue, Stream testValue)
    {
        // Validate streams are not null
        if (controlValue == null || testValue == null)
        {
            throw new ArgumentNullException("Streams cannot be null");
        }
        
        // Parse the control and test files
        Workbook controlWorkbook = await _excelWorkbookParser.ParseExcel(controlValue);
        Workbook testWorkbook = await _excelWorkbookParser.ParseExcel(testValue);
        
        // Validate the workbooks have the same number of sheets
        if(controlWorkbook.Sheets.Count != testWorkbook.Sheets.Count)
        {
            throw new ArgumentException("Control and test workbooks must have the same number of sheets");
        }
        
        // Validate the workbook sheets have the same names
        for (int i = 0; i < controlWorkbook.Sheets.Count; i++)
        {
            if (controlWorkbook.Sheets[i].Name != testWorkbook.Sheets[i].Name)
            {
                throw new ArgumentException("Control and test sheets must have the same names");
            }
        }
        
        // Create a new ExcelComparisonResult
        ExcelComparisionResult result = new();
        
        // Compare the control and test workbooks
        for (int i = 0; i < controlWorkbook.Sheets.Count; i++)
        {
            Sheet controlSheet = controlWorkbook.Sheets[i];
            Sheet testSheet = testWorkbook.Sheets[i];

            if (controlSheet.Cells == null && testSheet.Cells == null)
            {
                // if both are null, skip to the next sheet
                continue;
            }
            
            // Ensure the control and test sheets do not individually have null Cells
            if (controlSheet.Cells == null || testSheet.Cells == null)
            {
                throw new ArgumentException("Control and test sheets must both contain cells");
            }
            
            // Ensure the control and test sheets have the same number of rows and columns
            if (controlSheet.Cells.GetLength(0) != testSheet.Cells.GetLength(0) || controlSheet.Cells.GetLength(1) != testSheet.Cells.GetLength(1))
            {
                throw new ArgumentException("Control and test sheets must have the same number of rows and columns");
            }
            
            // Compare the control and test cells
            for (int j = 0; j < controlSheet.Cells.GetLength(0); j++)
            {
                for (int k = 0; k < controlSheet.Cells.GetLength(1); k++)
                {
                    object? currentControlValue = controlSheet.Cells[j, k];
                    object? currentTestValue = testSheet.Cells[j, k];
                    
                    // If the control and test values are not equal, add the comparison to the result
                    if (!ObjectsAreEqual(currentControlValue, currentTestValue))
                    {
                        result.Results.Add(new ExcelComparisonValue
                        {
                            SheetName = controlSheet.Name ?? testSheet.Name ?? $"Sheet{i + 1}",
                            RowLocation = $"{j}",
                            ColumnLocation = $"{k}",
                            ControlValue = currentControlValue,
                            TestValue = currentTestValue
                        });
                    }
                }
            }
        }

        return result;
    }

    private bool ObjectsAreEqual(object? currentControlValue, object? currentTestValue)
    {
        if(currentControlValue == null && currentTestValue == null)
        {
            return true;
        }
        
        if(currentControlValue == null || currentTestValue == null)
        {
            return false;
        }
        
        return currentControlValue.Equals(currentTestValue);
    }
}

public class ExcelComparisonValue
{
    public string SheetName { get; set; } = string.Empty;
    public string RowLocation { get; set; } = string.Empty;
    public string ColumnLocation { get; set; } = string.Empty;
    public object? ControlValue { get; set; }
    public object? TestValue { get; set; }
}

public class ExcelComparisionResult
{
    public List<ExcelComparisonValue> Results { get; set; } = new();
    public bool IsValid => Results.Count == 0;
}

public class ExcelComparisionRequest
{
    public Stream ControlFile { get; set; }
    public Stream TestFile { get; set; }
}

public class Workbook
{
    public List<Sheet> Sheets { get; set; } = new();
}

public class Sheet
{
    // Create a property to represent the sheet name
    public string? Name { get; set; }
    
    // Create a property to represent the sheet cells
    public object?[,]? Cells { get; set; }
}