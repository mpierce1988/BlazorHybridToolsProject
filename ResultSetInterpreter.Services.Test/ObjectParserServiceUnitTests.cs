using ResultSetInterpreter.Models.DTOs.InsertStatementToCSharp;
using ResultSetInterpreter.Models.ObjectDefinition;

namespace ResultSetInterpreter.Services.Test;

public class ObjectParserServiceUnitTests
{
    // TODO replace with mock
    private readonly ObjectParserService _objectParserService =
        new(new StringToObjectDefinitionParser(), new ObjectDefinitionPrinter());

    [Fact]
    public async Task ObjectDefinitionToCSharpCode_WhenCalledWithClassDefinitionAndData_ReturnsCSharpCode()
    {
        // Arrange
        string idColumnName = "Id";
        string idColumnDataType = "INT";
        string nameColumnName = "Name";
        string nameColumnDataType = "NVARCHAR(50)";

        string idCSharpDataType = "Int32";
        string nameCSharpDataType = "String";

        int idValue = 1;
        string nameValue = "TestName";

        string createAndInsertStatement =
            @$"CREATE TABLE #TestTable ({idColumnName} {idColumnDataType}, {nameColumnName} {nameColumnDataType});

INSERT INTO #TestTable ({idColumnName}, {nameColumnName})
VALUES
({idValue}, {nameValue});";

        string expectedResult = @$"public class TestTable {{
public {idCSharpDataType} {idColumnName} {{ get; set; }}
public {nameCSharpDataType} {nameColumnName} {{ get; set; }}
}}

List<TestTable> TestTableList = new() {{
new TestTable() {{
Id = {idValue},
Name = ""{nameValue}""
}}
}};";
        
        // Act
        ParseInsertStatementToCSharpRequest request = new()
        {
            InsertStatement = createAndInsertStatement,
            PrintType = ObjectDefinitionPrintType.ClassDefinitionAndData
        };
        ParseInsertStatementToCSharpResponse result = await _objectParserService.ParseInsertStatementToCSharpAsync(request);
        
        // Assert
        Assert.Equal(expectedResult, result.CSharpCode);
    }
    
    [Fact]
    public async Task ObjectDefinitionToCSharpCode_WhenCalledWithClassOnly_ReturnsClassOnlyCode()
    {
        // Arrange
        string idColumnName = "Id";
        string idColumnDataType = "INT";
        string nameColumnName = "Name";
        string nameColumnDataType = "NVARCHAR(50)";

        string idCSharpDataType = "Int32";
        string nameCSharpDataType = "String";

        int idValue = 1;
        string nameValue = "TestName";

        string createAndInsertStatement =
            @$"CREATE TABLE #TestTable ({idColumnName} {idColumnDataType}, {nameColumnName} {nameColumnDataType});

INSERT INTO #TestTable ({idColumnName}, {nameColumnName})
VALUES
({idValue}, {nameValue});";

        string expectedResult = @$"public class TestTable {{
public {idCSharpDataType} {idColumnName} {{ get; set; }}
public {nameCSharpDataType} {nameColumnName} {{ get; set; }}
}}";
        
        // Act
        ParseInsertStatementToCSharpRequest request = new()
        {
            InsertStatement = createAndInsertStatement,
            PrintType = ObjectDefinitionPrintType.ClassDefinition
        };
        ParseInsertStatementToCSharpResponse result = await _objectParserService.ParseInsertStatementToCSharpAsync(request);
        
        // Assert
        Assert.Equal(expectedResult, result.CSharpCode);
    }
    
    [Fact]
    public async Task ObjectDefinitionToCSharpCode_WhenCalledWithDataOnly_ReturnsDataOnlyCode()
    {
        // Arrange
        string idColumnName = "Id";
        string idColumnDataType = "INT";
        string nameColumnName = "Name";
        string nameColumnDataType = "NVARCHAR(50)";

        int idValue = 1;
        string nameValue = "TestName";

        string createAndInsertStatement =
            @$"CREATE TABLE #TestTable ({idColumnName} {idColumnDataType}, {nameColumnName} {nameColumnDataType});

INSERT INTO #TestTable ({idColumnName}, {nameColumnName})
VALUES
({idValue}, {nameValue});";

        string expectedResult = @$"List<TestTable> TestTableList = new() {{
new TestTable() {{
Id = {idValue},
Name = ""{nameValue}""
}}
}};";
        
        // Act
        ParseInsertStatementToCSharpRequest request = new()
        {
            InsertStatement = createAndInsertStatement,
            PrintType = ObjectDefinitionPrintType.Data
        };
        ParseInsertStatementToCSharpResponse result = await _objectParserService.ParseInsertStatementToCSharpAsync(request);
        
        // Assert
        Assert.Equal(expectedResult, result.CSharpCode);
    }
}