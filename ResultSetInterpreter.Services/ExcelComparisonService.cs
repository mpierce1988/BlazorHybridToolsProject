using ResultSetInterpreter.Models.DTOs.ExcelComparison;
using ResultSetInterpreter.Models.Workbook;
using ResultSetInterpreter.Services.Interfaces;
using ResultSetIntrepreter.Services.DTOs;

namespace ResultSetIntrepreter.Services;

public class ExcelComparisonService : IExcelComparisonService
{
    #region Fields
    
    private readonly IExcelWorkbookParser _excelWorkbookParser;
    
    #endregion
    
    #region Constructors
    
    public ExcelComparisonService(IExcelWorkbookParser excelWorkbookParser)
    {
        _excelWorkbookParser = excelWorkbookParser;
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>Compares two Excel files and returns the differences</summary>
    /// <param name="controlValue">The control Excel file</param>
    /// <param name="testValue">The test Excel file</param>
    /// <returns>The comparison result</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the control or test value is null or the control and test have different number of sheets
    /// or rows or columns
    /// </exception>
    public async Task<ExcelComparisionResponse> CompareExcelFilesAsync(Stream controlValue, Stream testValue)
    {
        // Create a new ExcelComparisonResult
        ExcelComparisionResponse response = new();
        
        try
        {
            // Validate streams are not null
            if (controlValue == null || testValue == null) 
                throw new ArgumentException("Streams cannot be null");

            // Parse the control and test files
            Workbook controlWorkbook = await _excelWorkbookParser.ParseExcel(controlValue);
            Workbook testWorkbook = await _excelWorkbookParser.ParseExcel(testValue);

            // Validate the workbooks have the same number of sheets
            EnsureWorkbooksAreSimilar(controlWorkbook, testWorkbook);
            
            // Compare the control and test workbooks
            for (int i = 0; i < controlWorkbook.Sheets.Count; i++)
            {
                Sheet controlSheet = controlWorkbook.Sheets[i];
                Sheet testSheet = testWorkbook.Sheets[i];

                CompareSheets(controlSheet, testSheet, i, response);
            }
        }
        catch (Exception e)
        {
            response.AddException(new HandledException(e.Message));
        }

        return response;
    }
    
    #endregion
    
    #region Private Methods

    /// <summary>
    /// Compares the control and test sheets, adding any differences to the response 
    /// </summary>
    /// <param name="controlSheet">The control sheet to compare</param>
    /// <param name="testSheet">The test sheet to compare</param>
    /// <param name="workbookIndex">The index of the workbook</param>
    /// <param name="response">The response to add the comparison results to</param>
    private void CompareSheets(Sheet controlSheet, Sheet testSheet, int workbookIndex, 
        ExcelComparisionResponse response)
    {
        if (controlSheet.Cells == null && testSheet.Cells == null)
        {
            // if both are null, skip to the next sheet
            return;
        }

        // Compare the control and test cells
        for (int j = 0; j < controlSheet.Cells!.GetLength(0); j++)
        {
            for (int k = 0; k < controlSheet.Cells!.GetLength(1); k++)
            {
                AddComparisonResultIfInvalid(testSheet, controlSheet, workbookIndex, 
                    j, k, response);
            }
        }
    }

    /// <summary>
    /// Adds a comparison result to the response if the control and test values are not equal 
    /// </summary>
    /// <param name="testSheet">The test sheet to compare</param>
    /// <param name="controlSheet">The control sheet to compare</param>
    /// <param name="sheetIndex">The index of the sheet</param>
    /// <param name="rowIndex">The index of the row</param>
    /// <param name="columnIndex">The index of the column</param>
    /// <param name="response">The response to add the comparison result to</param>
    private void AddComparisonResultIfInvalid(Sheet testSheet, Sheet controlSheet, int sheetIndex, int rowIndex,
        int columnIndex, ExcelComparisionResponse response)
    {
        object? currentControlValue = controlSheet.Cells?[rowIndex, columnIndex];
        object? currentTestValue = testSheet.Cells![rowIndex, columnIndex];

        // If the control and test values are not equal, add the comparison to the result
        if (!ObjectsAreEqual(currentControlValue, currentTestValue))
        {
            response.Results.Add(new ExcelComparisonValue
            {
                SheetName = controlSheet.Name ?? testSheet.Name ?? $"Sheet{sheetIndex + 1}",
                RowLocation = $"{rowIndex}",
                ColumnLocation = $"{columnIndex}",
                ControlValue = currentControlValue,
                TestValue = currentTestValue
            });
        }
    }
    
    /// <summary>Ensures the control and test workbooks are similar enough to compare</summary>
    /// <param name="controlWorkbook">The control workbook to compare</param>
    /// <param name="testWorkbook">The test workbook to compare</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the control and test workbooks have different number of sheets, rows, columns, or missing cells
    /// </exception>
    private void EnsureWorkbooksAreSimilar(Workbook controlWorkbook, Workbook testWorkbook)
    {
        if(controlWorkbook.Sheets.Count != testWorkbook.Sheets.Count)
        {
            throw new ArgumentException("Control and test workbooks must have the same number of sheets");
        }

        // Validate the workbook sheets are similar
        for (int i = 0; i < controlWorkbook.Sheets.Count; i++)
        {
            if (controlWorkbook.Sheets[i].Name != testWorkbook.Sheets[i].Name)
            {
                throw new ArgumentException("Control and test sheets must have the same names");
            }
            
            // Validate the sheet cells are either both null, or both not null
            if (controlWorkbook.Sheets[i].Cells == null && testWorkbook.Sheets[i].Cells == null)
            {
                // Go on to the next sheet
                continue;
            }

            if (controlWorkbook.Sheets[i].Cells == null)
            {
                throw new ArgumentException($"Control sheet ({controlWorkbook.Sheets[i].Name}) cells " +
                                            $"cannot be null if test sheet cells are not null");
            }

            if (testWorkbook.Sheets[i].Cells == null)
            {
                throw new ArgumentException($"Test sheet ({testWorkbook.Sheets[i].Name})  cells " +
                                            $"cannot be null if control sheet cells are not null");
            }
            
            // Validate the workbook sheets have the same number of rows
            if (controlWorkbook.Sheets[i].Cells!.GetLength(0) 
                != testWorkbook.Sheets[i].Cells!.GetLength(0))
            {
                throw new ArgumentException("Control and test sheets must have the same number of rows");
            }
            
            // Validate the workbook sheets have the same number of columns
            if (controlWorkbook.Sheets[i].Cells!.GetLength(1) 
                != testWorkbook.Sheets[i].Cells!.GetLength(1))
            {
                throw new ArgumentException("Control and test sheets must have the same number of columns");
            }
        }
    }

    /// <summary>Determines if two objects are equal, handling nulls</summary>
    /// <param name="currentControlValue">The current control value to compare</param>
    /// <param name="currentTestValue">The current test value to compare</param>
    /// <returns>True if the objects are equal, false otherwise</returns>
    private bool ObjectsAreEqual(object? currentControlValue, object? currentTestValue)
    {
        // If both are null, they are equal
        if(currentControlValue == null && currentTestValue == null)
        {
            return true;
        }
        
        // If one is null and the other is not, they are not equal
        if(currentControlValue == null || currentTestValue == null)
        {
            return false;
        }
        
        return currentControlValue.Equals(currentTestValue);
    }
    
    #endregion
}