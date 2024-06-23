using ResultSetInterpreter.Models.Workbook;
using ResultSetInterpreter.Services.EpPlus;
using ResultSetInterpreter.Services.Test.Utilities;
using ResultSetIntrepreter.Services;

namespace ResultSetInterpreter.Services.Test;

public class ExcelWorkbookParserUnitTests
{
    private readonly IExcelWorkbookParser _workbookParser;

    private const string TwoColumnsTwoRowsFileName = "TwoColumnsTwoRows.xlsx";
    private const string UserTracking = "User_Tracking.xlsx";

    public ExcelWorkbookParserUnitTests()
    {
        _workbookParser = new EpPlusExcelWorkbookParser();
    }

    [Fact]
    public async Task OneSheetTwoColumnsTwoRows_ReturnsCorrectNumberOfColumnsAndRows()
    {
        // Arrange
        await using var stream = File.OpenRead(TestUtility.GetSamplePath(TwoColumnsTwoRowsFileName));
        
        // Act
        Workbook workbook = await _workbookParser.ParseExcel(stream);
        Sheet firstSheet = workbook.Sheets[0];
        
        // Assert
        Assert.NotNull(firstSheet);
        Assert.NotNull(firstSheet.Cells);
        Assert.Equal(2, firstSheet.Cells.GetLength(0));
        Assert.Equal(2, firstSheet.Cells.GetLength(1));
    }
    
    [Fact]
    public async Task TwoSheetVariousColumnsAndRows_ReturnsCorrectNumberOfColumnsAndRows()
    {
        // Arrange
        await using var stream = File.OpenRead(TestUtility.GetSamplePath(UserTracking));
        int userSheetNumRows = 4;
        int userSheetNumColumns = 5;
        int trackingSheetNumRows = 6;
        int trackingSheetNumColumns = 4;
        
        // Act
        Workbook workbook = await _workbookParser.ParseExcel(stream);
        Sheet userSheet = workbook.Sheets[0];
        Sheet trackingSheet = workbook.Sheets[1];
        
        // Assert
        Assert.NotNull(userSheet);
        Assert.NotNull(userSheet.Cells);
        Assert.Equal(userSheetNumRows, userSheet.Cells.GetLength(0));
        Assert.Equal(userSheetNumColumns, userSheet.Cells.GetLength(1));
        
        Assert.NotNull(trackingSheet);
        Assert.NotNull(trackingSheet.Cells);
        Assert.Equal(trackingSheetNumRows, trackingSheet.Cells.GetLength(0));
        Assert.Equal(trackingSheetNumColumns, trackingSheet.Cells.GetLength(1));
    }

    [Fact]
    public async Task TwoNamedSheets_ReturnsCorrectNames()
    {
        // Arrange
        await using var stream = File.OpenRead(TestUtility.GetSamplePath(UserTracking));
        int numberSheets = 2;
        string firstSheetName = "User";
        string secondSheetName = "Tracking";
        
        // Act
        Workbook workbook = await _workbookParser.ParseExcel(stream);
        
        // Assert
        Assert.Equal(numberSheets, workbook.Sheets.Count);
        Assert.Equal(firstSheetName, workbook.Sheets[0].Name);
        Assert.Equal(secondSheetName, workbook.Sheets[1].Name);
    }
}