using Moq;
using ResultSetInterpreter.Models.DTOs.ExcelComparison;
using ResultSetInterpreter.Models.Workbook;

namespace ResultSetInterpreter.Services.Test;

public class ExcelComparisonServiceUnitTests
{
    #region Mock Workbooks

    private Workbook _controlWorkbook = new Workbook()
    {
        Sheets = new()
        {
            new Sheet()
            {
                Name = "User",
                Cells = new object[,]
                {
                    { "ID", "First Name", "Last Name", "Email", "DOB" },
                    { 1, "Tim", "Robinson", "tim@robinson.com", DateTime.Parse("1972-12-14") },
                    { 2, "John", "Titor", "john@titor.com", DateTime.Parse("1983-05-09") },
                }
            },
            new Sheet()
            {
                Name = "Tracking",
                Cells = new Object[,]
                {
                    { "TrackingID", "OrderID", "IsActive", "DeliveryDate" },
                    { 1, 35, true, DateTime.Parse("2021-12-14") },
                    { 2, 36, false, DateTime.Parse("2021-12-15") },
                    { 3, 37, true, DateTime.Parse("2021-12-16") },
                }
            }
        }
    };
    
    private Workbook _userFirstNameOrderIdDifferent = new Workbook()
    {
        Sheets = new()
        {
            new Sheet()
            {
                Name = "User",
                Cells = new object[,]
                {
                    { "ID", "First Name", "Last Name", "Email", "DOB" },
                    { 1, "Timothy", "Robinson", "tim@robinson.com", DateTime.Parse("1972-12-14") }, // First Name changed
                    { 2, "John", "Titor", "john@titor.com", DateTime.Parse("1983-05-09") },
                }
            },
            new Sheet()
            {
                Name = "Tracking",
                Cells = new Object[,]
                {
                    { "TrackingID", "OrderID", "IsActive", "DeliveryDate" },
                    { 1, 45, true, DateTime.Parse("2021-12-14") }, // Order ID changed
                    { 2, 36, false, DateTime.Parse("2021-12-15") },
                    { 3, 37, true, DateTime.Parse("2021-12-16") },
                }
            }
        }
    };
    
    private Workbook _userFirstNameDifferent = new Workbook()
    {
        Sheets = new()
        {
            new Sheet()
            {
                Name = "User",
                Cells = new object[,]
                {
                    { "ID", "First Name", "Last Name", "Email", "DOB" },
                    { 1, "Timothy", "Robinson", "tim@robinson.com", DateTime.Parse("1972-12-14") },
                    { 2, "John", "Titor", "john@titor.com", DateTime.Parse("1983-05-09") },
                }
            },
            new Sheet()
            {
                Name = "Tracking",
                Cells = new Object[,]
                {
                    { "TrackingID", "OrderID", "IsActive", "DeliveryDate" },
                    { 1, 35, true, DateTime.Parse("2021-12-14") },
                    { 2, 36, false, DateTime.Parse("2021-12-15") },
                    { 3, 37, true, DateTime.Parse("2021-12-16") },
                }
            }
        }
    };
    
    private Workbook _additionalRow = new Workbook()
    {
        Sheets = new()
        {
            new Sheet()
            {
                Name = "User",
                Cells = new object[,]
                {
                    { "ID", "First Name", "Last Name", "Email", "DOB" },
                    { 1, "Tim", "Robinson", "tim@robinson.com", DateTime.Parse("1972-12-14") },
                    { 2, "John", "Titor", "john@titor.com", DateTime.Parse("1983-05-09") },
                    { 3, "Jane", "Doe", "jane@dow.com", DateTime.Parse("1976-03-24") }, 
                }
            },
            new Sheet()
            {
                Name = "Tracking",
                Cells = new Object[,]
                {
                    { "TrackingID", "OrderID", "IsActive", "DeliveryDate" },
                    { 1, 35, true, DateTime.Parse("2021-12-14") },
                    { 2, 36, false, DateTime.Parse("2021-12-15") },
                    { 3, 37, true, DateTime.Parse("2021-12-16") },
                }
            }
        }
    };
    
    private Workbook _differentSheetName = new Workbook()
    {
        Sheets = new()
        {
            new Sheet()
            {
                Name = "User",
                Cells = new object[,]
                {
                    { "ID", "First Name", "Last Name", "Email", "DOB" },
                    { 1, "Tim", "Robinson", "tim@robinson.com", DateTime.Parse("1972-12-14") },
                    { 2, "John", "Titor", "john@titor.com", DateTime.Parse("1983-05-09") },
                }
            },
            new Sheet()
            {
                Name = "Trackings",
                Cells = new Object[,]
                {
                    { "TrackingID", "OrderID", "IsActive", "DeliveryDate" },
                    { 1, 35, true, DateTime.Parse("2021-12-14") },
                    { 2, 36, false, DateTime.Parse("2021-12-15") },
                    { 3, 37, true, DateTime.Parse("2021-12-16") },
                }
            }
        }
    };
    
    private Workbook _additionalColumn = new Workbook()
    {
        Sheets = new()
        {
            new Sheet()
            {
                Name = "User",
                Cells = new object[,]
                {
                    { "ID", "First Name", "Last Name", "Email", "DOB", "IsConfirmed" },
                    { 1, "Tim", "Robinson", "tim@robinson.com", DateTime.Parse("1972-12-14"), "true" },
                    { 2, "John", "Titor", "john@titor.com", DateTime.Parse("1983-05-09"), "false" },
                }
            },
            new Sheet()
            {
                Name = "Trackings",
                Cells = new Object[,]
                {
                    { "TrackingID", "OrderID", "IsActive", "DeliveryDate" },
                    { 1, 35, true, DateTime.Parse("2021-12-14") },
                    { 2, 36, false, DateTime.Parse("2021-12-15") },
                    { 3, 37, true, DateTime.Parse("2021-12-16") },
                }
            }
        }
    };
    
    private Workbook _extraSheet = new Workbook()
    {
        Sheets = new()
        {
            new Sheet()
            {
                Name = "User",
                Cells = new object[,]
                {
                    { "ID", "First Name", "Last Name", "Email", "DOB" },
                    { 1, "Tim", "Robinson", "tim@robinson.com", DateTime.Parse("1972-12-14") },
                    { 2, "John", "Titor", "john@titor.com", DateTime.Parse("1983-05-09") },
                }
            },
            new Sheet()
            {
                Name = "Tracking",
                Cells = new Object[,]
                {
                    { "TrackingID", "OrderID", "IsActive", "DeliveryDate" },
                    { 1, 35, true, DateTime.Parse("2021-12-14") },
                    { 2, 36, false, DateTime.Parse("2021-12-15") },
                    { 3, 37, true, DateTime.Parse("2021-12-16") },
                }
            }, new Sheet()
            {
                Name = "Order",
                Cells = new object?[,]
                {
                    {"OrderId", "UserId", "TrackingId"},
                    {"1", "1", "1"},
                    {"2", "1", "2"},
                    {"3", "2", "3"},
                    {"4", "3", "5"},
                    {"5", "6", "7"},
                    {"6", "1", "9"},
                }
            }
        }
    };
    
    #endregion
    
    // Filename constants
    // TODO move to a constants class
    private const string ControlFileName = "User_Tracking.xlsx";
    private const string UserTrackingOrder = "User_Tracking_Order.xlsx";
    private const string UserTrackings = "User_Trackings.xlsx";
    private const string FirstNameChangedFileName = "User_Tracking_FirstNameChanged.xlsx";
    private const string FirstNameOrderIdChangedFileName = "User_Tracking_FirstNameOrderIDChanged.xlsx";
    private const string UserIsConfirmedTrackingFileName = "UserIsConfirmed_Tracking.xlsx";
    private const string ExtraUserTrackingFileName = "ExtraUser_Tracking.xlsx";

    [Fact]
    public async Task CompareSameExcelFiles_ReturnsEqual()
    {
        // Arrange
        
        await using var controlValue = File.OpenRead(TestUtility.GetSamplePath(ControlFileName));
        await using var testValue = File.OpenRead(TestUtility.GetSamplePath(ControlFileName));

        // Avoid warning about Access to disposed closure by assigning to a variable
        Stream controlValueStream = controlValue;
        Stream testValueStream = testValue;
        
        // Moq the parser
        var mockParser = new Mock<IExcelWorkbookParser>();
        mockParser.Setup(p => p.ParseExcel(controlValueStream)).ReturnsAsync(_controlWorkbook);
        mockParser.Setup(p => p.ParseExcel(testValueStream)).ReturnsAsync(_controlWorkbook);
        
        ExcelComparisonService comparisonService = new ExcelComparisonService(mockParser.Object);
        ExcelComparisonRequest request = new()
        {
            ControlFile = controlValueStream,
            TestFile = testValueStream
        };
        
        // Act
        ExcelComparisonResponse response = await comparisonService.CompareExcelFilesAsync(request);
        
        // Assert
        Assert.True(response.IsIdentical);
    }

    [Fact]
    public async Task FirstStreamIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        await using var testValue = File.OpenRead(TestUtility.GetSamplePath(ControlFileName));
        Stream testValueStream = testValue;

        ExcelComparisonRequest request = new()
        {
            ControlFile = null,
            TestFile = testValueStream
        };
        
        // Set up moq
        var mockParser = new Mock<IExcelWorkbookParser>();
        mockParser.Setup(p => p.ParseExcel(testValueStream)).ReturnsAsync(_controlWorkbook);
        ExcelComparisonService comparisonService = new ExcelComparisonService(mockParser.Object);
        
        // Act
        var result = await comparisonService.CompareExcelFilesAsync(request);
        
        // Act and Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task SecondStreamIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        await using Stream controlValue = File.OpenRead(TestUtility.GetSamplePath(ControlFileName));
        // Avoid warning about Access to disposed closure by assigning to a variable
        Stream controlValueStream = controlValue;
        
        ExcelComparisonRequest request = new()
        {
            ControlFile = controlValueStream,
            TestFile = null
        };
        
        // Set up moq
        var mockParser = new Mock<IExcelWorkbookParser>();
        mockParser.Setup(p => p.ParseExcel(controlValueStream)).ReturnsAsync(_controlWorkbook);
        ExcelComparisonService comparisonService = new ExcelComparisonService(mockParser.Object);
        
        // Act
        var result = await comparisonService.CompareExcelFilesAsync(request);
        
        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task TwoWorkbooksDifferentNumberSheets_ThrowsArgumentException()
    {
        // Arrange
        await using Stream controlValue = File.OpenRead(TestUtility.GetSamplePath(ControlFileName));
        await using Stream testValue = File.OpenRead(TestUtility.GetSamplePath(UserTrackingOrder));

        ExcelComparisonRequest request = new()
        {
            ControlFile = controlValue,
            TestFile = testValue
        };
        
        // Set up moq
        var mockParser = new Mock<IExcelWorkbookParser>();
        mockParser.Setup(p => p.ParseExcel(request.ControlFile)).ReturnsAsync(_controlWorkbook);
        mockParser.Setup(p => p.ParseExcel(request.TestFile)).ReturnsAsync(_extraSheet);
        
        ExcelComparisonService comparisonService = new ExcelComparisonService(mockParser.Object);
        
        // Act
        ExcelComparisonResponse result = await comparisonService.CompareExcelFilesAsync(request);
        
        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task TwoWorkbooksDifferentSheetNames_ThrowsArgumentException()
    {
        // Arrange
        await using Stream controlValue = File.OpenRead(TestUtility.GetSamplePath(ControlFileName));
        await using Stream testValue = File.OpenRead(TestUtility.GetSamplePath(UserTrackings));
        
        ExcelComparisonRequest request = new()
        {
            ControlFile = controlValue,
            TestFile = testValue
        };
        
        // Set up moq
        var mockParser = new Mock<IExcelWorkbookParser>();
        mockParser.Setup(p => p.ParseExcel(request.ControlFile)).ReturnsAsync(_controlWorkbook);
        mockParser.Setup(p => p.ParseExcel(request.TestFile)).ReturnsAsync(_differentSheetName);
        
        ExcelComparisonService comparisonService = new ExcelComparisonService(mockParser.Object);
        
        // Act
        var result = await comparisonService.CompareExcelFilesAsync(request);
        
        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task TwoWorkbooksDifferentNumberColumns_ThrowsArgumentException()
    {
        // Arrange
        await using Stream controlValue = File.OpenRead(TestUtility.GetSamplePath(ControlFileName));
        await using Stream testValue = File.OpenRead(TestUtility.GetSamplePath(UserIsConfirmedTrackingFileName));
        
        ExcelComparisonRequest request = new()
        {
            ControlFile = controlValue,
            TestFile = testValue
        };
        
        // Set up moq
        var mockParser = new Mock<IExcelWorkbookParser>();
        mockParser.Setup(p => p.ParseExcel(request.ControlFile)).ReturnsAsync(_controlWorkbook);
        mockParser.Setup(p => p.ParseExcel(request.TestFile)).ReturnsAsync(_additionalColumn);
        
        ExcelComparisonService comparisonService = new ExcelComparisonService(mockParser.Object);
        
        // Act
        var result = await comparisonService.CompareExcelFilesAsync(request);
        
        // Assert
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public async Task TwoWorkbooksDifferentNumberRows_ThrowsArgumentException()
    {
        // Arrange
        await using Stream controlValue = File.OpenRead(TestUtility.GetSamplePath(ControlFileName));
        await using Stream testValue = File.OpenRead(TestUtility.GetSamplePath(ExtraUserTrackingFileName));
        
        ExcelComparisonRequest request = new()
        {
            ControlFile = controlValue,
            TestFile = testValue
        };
        
        // Set up moq
        var mockParser = new Mock<IExcelWorkbookParser>();
        mockParser.Setup(p => p.ParseExcel(request.ControlFile)).ReturnsAsync(_controlWorkbook);
        mockParser.Setup(p => p.ParseExcel(request.TestFile)).ReturnsAsync(_additionalRow);
        
        ExcelComparisonService comparisonService = new ExcelComparisonService(mockParser.Object);
        
        // Act
        var result = await comparisonService.CompareExcelFilesAsync(request);
        
        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task UserFirstNameDifferent_ReturnsOneDifference()
    {
        // arrange
        await using Stream controlValue = File.OpenRead(TestUtility.GetSamplePath(ControlFileName));
        await using Stream testValue = File.OpenRead(TestUtility.GetSamplePath(FirstNameChangedFileName));
        int numberOfDifferences = 1;
        
        ExcelComparisonRequest request = new()
        {
            ControlFile = controlValue,
            TestFile = testValue
        };
        
        // Set up moq
        var mockParser = new Mock<IExcelWorkbookParser>();
        mockParser.Setup(p => p.ParseExcel(request.ControlFile)).ReturnsAsync(_controlWorkbook);
        mockParser.Setup(p => p.ParseExcel(request.TestFile)).ReturnsAsync(_userFirstNameDifferent);
        
        ExcelComparisonService comparisonService = new ExcelComparisonService(mockParser.Object);
        
        // Act
        ExcelComparisonResponse response = await comparisonService.CompareExcelFilesAsync(request);
        
        // Assert
        Assert.False(response.IsIdentical);
        Assert.Equal(numberOfDifferences, response.Results.Count());
    }

    [Fact]
    public async Task UserFirstNameOrderIdDifferent_ReturnsTwoDifferences()
    {
        // Arrange
        await using Stream controlValue = File.OpenRead(TestUtility.GetSamplePath(ControlFileName));
        await using Stream testValue = File.OpenRead(TestUtility.GetSamplePath(FirstNameOrderIdChangedFileName));
        int expectedNumberOfDifferences = 2;
        
        ExcelComparisonRequest request = new()
        {
            ControlFile = controlValue,
            TestFile = testValue
        };
        
        // Set up moq
        var mockParser = new Mock<IExcelWorkbookParser>();
        mockParser.Setup(p => p.ParseExcel(request.ControlFile)).ReturnsAsync(_controlWorkbook);
        mockParser.Setup(p => p.ParseExcel(request.TestFile)).ReturnsAsync(_userFirstNameOrderIdDifferent);
        
        ExcelComparisonService comparisonService = new ExcelComparisonService(mockParser.Object);
        
        // Act
        ExcelComparisonResponse response = await comparisonService.CompareExcelFilesAsync(request);
        
        // Assert
        Assert.False(response.IsIdentical);
        Assert.Equal(expectedNumberOfDifferences, response.Results.Count);
    }
}