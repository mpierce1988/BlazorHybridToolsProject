using FluentAssertions;
using ResultSetInterpreter.Models.ExcelCSharp;
using ResultSetInterpreter.Services.Interfaces;
using ResultSetInterpreter.Services.Test.Utilities;
using ResultSetIntrepreter.Services;

namespace ResultSetInterpreter.Services.Test;

public class ExcelCSharpWorkbookoParserUnitTests
{
    private readonly IExcelCSharpWorkbookParser _workbookParser;
    
    public ExcelCSharpWorkbookoParserUnitTests()
    {

        _workbookParser = new EpPlusExcelCSharpWorkbookParser();
    }

    [Fact]
    public async Task ReceiveUserTrackingSample_CorrectlyParsedResult()
    {
        string expectedJson = "{\"Sheets\":[{\"Name\":\"User\",\"Rows\":[{\"RowNumber\":2,\"Values\":[{\"CSharpColumnNameType\":{\"ColumnName\":\"UserID\",\"Type\":\"System.Int32?\"},\"Value\":1.0},{\"CSharpColumnNameType\":{\"ColumnName\":\"FirstName\",\"Type\":\"System.String?\"},\"Value\":\"Tim\"},{\"CSharpColumnNameType\":{\"ColumnName\":\"LastName\",\"Type\":\"System.String?\"},\"Value\":\"Robinson\"},{\"CSharpColumnNameType\":{\"ColumnName\":\"Email\",\"Type\":\"System.String?\"},\"Value\":\"tim@robinson.com\"},{\"CSharpColumnNameType\":{\"ColumnName\":\"DOB\",\"Type\":\"System.DateTime?\"},\"Value\":26647.0}]},{\"RowNumber\":3,\"Values\":[{\"CSharpColumnNameType\":{\"ColumnName\":\"UserID\",\"Type\":\"System.Int32?\"},\"Value\":2.0},{\"CSharpColumnNameType\":{\"ColumnName\":\"FirstName\",\"Type\":\"System.String?\"},\"Value\":\"John\"},{\"CSharpColumnNameType\":{\"ColumnName\":\"LastName\",\"Type\":\"System.String?\"},\"Value\":\"Titor\"},{\"CSharpColumnNameType\":{\"ColumnName\":\"Email\",\"Type\":\"System.String?\"},\"Value\":\"john@titor.com\"},{\"CSharpColumnNameType\":{\"ColumnName\":\"DOB\",\"Type\":\"System.DateTime?\"},\"Value\":30445.0}]},{\"RowNumber\":4,\"Values\":[{\"CSharpColumnNameType\":{\"ColumnName\":\"UserID\",\"Type\":\"System.Int32?\"},\"Value\":3.0},{\"CSharpColumnNameType\":{\"ColumnName\":\"FirstName\",\"Type\":\"System.String?\"},\"Value\":\"Steve\"},{\"CSharpColumnNameType\":{\"ColumnName\":\"LastName\",\"Type\":\"System.String?\"},\"Value\":\"Rogers\"},{\"CSharpColumnNameType\":{\"ColumnName\":\"Email\",\"Type\":\"System.String?\"},\"Value\":\"steve.rogers@avengers.org\"},{\"CSharpColumnNameType\":{\"ColumnName\":\"DOB\",\"Type\":\"System.DateTime?\"},\"Value\":6760.0}]}]},{\"Name\":\"Tracking\",\"Rows\":[{\"RowNumber\":2,\"Values\":[{\"CSharpColumnNameType\":{\"ColumnName\":\"TrackingID\",\"Type\":\"System.Int32?\"},\"Value\":1.0},{\"CSharpColumnNameType\":{\"ColumnName\":\"OrderID\",\"Type\":\"System.Int32?\"},\"Value\":35.0},{\"CSharpColumnNameType\":{\"ColumnName\":\"IsActive\",\"Type\":\"System.String?\"},\"Value\":true},{\"CSharpColumnNameType\":{\"ColumnName\":\"DeliveryDate\",\"Type\":\"System.DateTime?\"},\"Value\":44899.0}]},{\"RowNumber\":3,\"Values\":[{\"CSharpColumnNameType\":{\"ColumnName\":\"TrackingID\",\"Type\":\"System.Int32?\"},\"Value\":2.0},{\"CSharpColumnNameType\":{\"ColumnName\":\"OrderID\",\"Type\":\"System.Int32?\"},\"Value\":54.0},{\"CSharpColumnNameType\":{\"ColumnName\":\"IsActive\",\"Type\":\"System.String?\"},\"Value\":false},{\"CSharpColumnNameType\":{\"ColumnName\":\"DeliveryDate\",\"Type\":\"System.DateTime?\"},\"Value\":44948.0}]},{\"RowNumber\":4,\"Values\":[{\"CSharpColumnNameType\":{\"ColumnName\":\"TrackingID\",\"Type\":\"System.Int32?\"},\"Value\":3.0},{\"CSharpColumnNameType\":{\"ColumnName\":\"OrderID\",\"Type\":\"System.Int32?\"},\"Value\":76.0},{\"CSharpColumnNameType\":{\"ColumnName\":\"IsActive\",\"Type\":\"System.String?\"},\"Value\":true},{\"CSharpColumnNameType\":{\"ColumnName\":\"DeliveryDate\",\"Type\":\"System.DateTime?\"},\"Value\":45044.0}]},{\"RowNumber\":5,\"Values\":[{\"CSharpColumnNameType\":{\"ColumnName\":\"TrackingID\",\"Type\":\"System.Int32?\"},\"Value\":4.0},{\"CSharpColumnNameType\":{\"ColumnName\":\"OrderID\",\"Type\":\"System.Int32?\"},\"Value\":89.0},{\"CSharpColumnNameType\":{\"ColumnName\":\"IsActive\",\"Type\":\"System.String?\"},\"Value\":true},{\"CSharpColumnNameType\":{\"ColumnName\":\"DeliveryDate\",\"Type\":\"System.DateTime?\"},\"Value\":45050.0}]},{\"RowNumber\":6,\"Values\":[{\"CSharpColumnNameType\":{\"ColumnName\":\"TrackingID\",\"Type\":\"System.Int32?\"},\"Value\":5.0},{\"CSharpColumnNameType\":{\"ColumnName\":\"OrderID\",\"Type\":\"System.Int32?\"},\"Value\":93.0},{\"CSharpColumnNameType\":{\"ColumnName\":\"IsActive\",\"Type\":\"System.String?\"},\"Value\":false},{\"CSharpColumnNameType\":{\"ColumnName\":\"DeliveryDate\",\"Type\":\"System.DateTime?\"},\"Value\":45292.0}]}]}]}";

      //ExcelCSharpWorkbook expectedResult = TestUtility.DeserializeFromJSon<ExcelCSharpWorkbook>(json);
      string filePath = TestUtility.GetSamplePath("User_Tracking.xlsx");
      await using Stream fileStream = File.OpenRead(filePath);
      
      // Act
      ExcelCSharpWorkbook result = await _workbookParser.ParseExcelCSharpWorkbookAsync(fileStream);

      string resultJson = TestUtility.SerializeToJson<ExcelCSharpWorkbook>(result);

      Assert.Equal(expectedJson, resultJson);

    }


}