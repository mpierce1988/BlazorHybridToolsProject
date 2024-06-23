using ResultSetInterpreter.Models.ObjectDefinition;
using ResultSetIntrepreter.Services;
using ResultSetIntrepreter.Services.Interfaces;

namespace ResultSetInterpreter.Services.Test;

public class ObjectParserServiceUnitTests
{
    private readonly ObjectParserService _objectParserService =
        new(new StringToObjectDefinitionParser(), new ObjectDefinitionPrinter());

    [Fact]
    public async Task ObjectDefinitionToCSharpCode_WhenCalledWithClassDefinitionAndData_ReturnsCSharpCode()
    {
        // Arrange
        int numExpectedProperties = 2;
        string idColumnName = "Id";
        string idColumnDataType = "INT";
        string nameColumnName = "Name";
        string nameColumnDataType = "NVARCHAR(50)";

        string idCSharpDataType = "Int32";
        string nameCSharpDataType = "String";

        int numExpectedDataRows = 1;
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
}