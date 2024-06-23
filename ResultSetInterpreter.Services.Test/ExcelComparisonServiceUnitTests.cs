using ResultSetInterpreter.Services.EpPlus;
using ResultSetInterpreter.Services.Interfaces;
using ResultSetInterpreter.Services.Test.Utilities;
using ResultSetIntrepreter.Services;

namespace ResultSetInterpreter.Services.Test;

public class ExcelComparisonServiceUnitTests
{
    private readonly IExcelComparisonService _comparisonService;
    
    // Filename constants
    // TODO move to a constants class
    private const string ControlFileName = "User_Tracking.xlsx";
    private const string UserTrackingOrder = "User_Tracking_Order.xlsx";
    private const string UserTrackings = "User_Trackings.xlsx";
    private const string FirstNameChangedFileName = "User_Tracking_FirstNameChanged.xlsx";
    private const string FirstNameOrderIdChangedFileName = "User_Tracking_FirstNameOrderIDChanged.xlsx";
    private const string UserIsConfirmedTrackingFileName = "UserIsConfirmed_Tracking.xlsx";
    private const string ExtraUserTrackingFileName = "ExtraUser_Tracking.xlsx";

    public ExcelComparisonServiceUnitTests()
    {
        IExcelWorkbookParser parser = new EpPlusExcelWorkbookParser();
        _comparisonService = new ExcelComparisonService(parser);
    }

    [Fact]
    public async Task CompareSameExcelFiles_ReturnsEqual()
    {
        // Arrange
        await using var controlValue = File.OpenRead(TestUtility.GetSamplePath(ControlFileName));
        await using var testValue = File.OpenRead(TestUtility.GetSamplePath(ControlFileName));
        
        // Act
        ExcelComparisionResult result = await _comparisonService.CompareExcelFilesAsync(controlValue, testValue);
        
        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task FirstStreamIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Stream? controlValue = null;
        await using var testValue = File.OpenRead(TestUtility.GetSamplePath(ControlFileName));
        
        // Act and Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _comparisonService.CompareExcelFilesAsync(controlValue!, testValue));
    }

    [Fact]
    public async Task SecondStreamIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        await using Stream controlValue = File.OpenRead(TestUtility.GetSamplePath(ControlFileName));
        Stream? testValue = null;
        
        // Act and Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _comparisonService.CompareExcelFilesAsync(controlValue, testValue!));
    }

    [Fact]
    public async Task TwoWorkbooksDifferentNumberSheets_ThrowsArgumentException()
    {
        // Arrange
        await using Stream controlValue = File.OpenRead(TestUtility.GetSamplePath(ControlFileName));
        await using Stream testValue = File.OpenRead(TestUtility.GetSamplePath(UserTrackingOrder));
        
        // Act and Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _comparisonService.CompareExcelFilesAsync(controlValue, testValue));
    }

    [Fact]
    public async Task TwoWorkbooksDifferentSheetNames_ThrowsArgumentException()
    {
        // Arrange
        await using Stream controlValue = File.OpenRead(TestUtility.GetSamplePath(ControlFileName));
        await using Stream testValue = File.OpenRead(TestUtility.GetSamplePath(UserTrackings));
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _comparisonService.CompareExcelFilesAsync(controlValue, testValue));
    }

    [Fact]
    public async Task TwoWorkbooksDifferentNumberColumns_ThrowsArgumentException()
    {
        // Arrange
        await using Stream controlValue = File.OpenRead(TestUtility.GetSamplePath(ControlFileName));
        await using Stream testValue = File.OpenRead(TestUtility.GetSamplePath(UserIsConfirmedTrackingFileName));
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _comparisonService.CompareExcelFilesAsync(controlValue, testValue));
    }
    
    [Fact]
    public async Task TwoWorkbooksDifferentNumberRows_ThrowsArgumentException()
    {
        // Arrange
        await using Stream controlValue = File.OpenRead(TestUtility.GetSamplePath(ControlFileName));
        await using Stream testValue = File.OpenRead(TestUtility.GetSamplePath(ExtraUserTrackingFileName));
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _comparisonService.CompareExcelFilesAsync(controlValue, testValue));
    }

    [Fact]
    public async Task UserFirstNameDifferent_ReturnsOneDifference()
    {
        // arrange
        await using Stream controlValue = File.OpenRead(TestUtility.GetSamplePath(ControlFileName));
        await using Stream testValue = File.OpenRead(TestUtility.GetSamplePath(FirstNameChangedFileName));
        int numberOfDifferences = 1;
        
        // Act
        ExcelComparisionResult result = await _comparisonService.CompareExcelFilesAsync(controlValue, testValue);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(numberOfDifferences, result.Results.Count());
    }

    [Fact]
    public async Task UserFirstNameOrderIdDifferent_ReturnsTwoDifferences()
    {
        // Arrange
        await using Stream controlValue = File.OpenRead(TestUtility.GetSamplePath(ControlFileName));
        await using Stream testValue = File.OpenRead(TestUtility.GetSamplePath(FirstNameOrderIdChangedFileName));
        int expectedNumberOfDifferences = 2;
        
        // Act
        ExcelComparisionResult result = await _comparisonService.CompareExcelFilesAsync(controlValue, testValue);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(expectedNumberOfDifferences, result.Results.Count);
    }
}