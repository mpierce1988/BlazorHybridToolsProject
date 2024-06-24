using ResultSetInterpreter.Models.ObjectDefinition;
using ResultSetIntrepreter.Services;

namespace ResultSetInterpreter.Services.Test;

public class StringToObjectDefinitionParserUnitTests
{
    private readonly StringToObjectDefinitionParser _definitionParser = new();

    [Fact]
    public async Task ParseInsertStatementAsync_CreateTableName_RetrieveCorrectName()
    {
        // Arrange
        string tableName = "TestTable";
        string tempTableDeclaration = $"CREATE TABLE #{tableName} (Id INT, Name NVARCHAR(50));";
        
        // Act
        ObjectDefinition definition = await _definitionParser.ParseInsertStatementAsync(tempTableDeclaration);
        
        Assert.Equal(tableName, definition.Name);
    }

    [Fact]
    public async Task ParseInsertStatementAsync_CreateTableIntIDColumn_ReturnCorrectProperty()
    {
        // Arrange
        string columnName = "Id";
        string columnType = "INT";
        string cSharpDataType = "system.int32";
        string tempTableDeclaration = $"CREATE TABLE #TestTable ({columnName} {columnType});";
        
        // Act
        ObjectDefinition definition = await _definitionParser.ParseInsertStatementAsync(tempTableDeclaration);
        
        // Assert
        Assert.Equal(1, definition.Properties.Count);
        Assert.NotNull(definition.Properties.First());
        Assert.Equal(columnName, definition.Properties.First().Name);
        Assert.NotNull(definition.Properties.First().Type);
        Assert.Equal(cSharpDataType.ToLowerInvariant(), definition.Properties.First().Type!.ToString().ToLowerInvariant());
    }

    [Fact]
    public async Task ParseInsertStatementAsync_CreateTableNameNVarCharColumn_ReturnsCorrectProperties()
    {
        // Arrange
        string columnName = "Name";
        string columnType = "NVARCHAR(50)";
        string cSharpDataType = "System.String";
        int totalProperties = 2;
        string tempTableDeclaration = @$"CREATE TABLE #TestTable (Id INT, {columnName} 
{columnType});";
        
        // Act
        ObjectDefinition definition = await _definitionParser.ParseInsertStatementAsync(tempTableDeclaration);
        
        // Assert
        Assert.Equal(totalProperties, definition.Properties.Count);
        Assert.Contains(definition.Properties, property => property.Name == columnName);
        Assert.NotNull(definition.Properties.First(prop => prop.Name == columnName).Type);
        Assert.Equal(cSharpDataType.ToLowerInvariant(), 
            definition.Properties.First(prop => prop.Name == columnName)
                .Type!.ToString().ToLowerInvariant());
    }
    
    // TODO rest of data type tests

    [Fact]
    public async Task ParseInsertStatementAsync_InsertOneRowOneColumn_ReturnsCorrectData()
    {
        // Arrange
        string columnName = "Id";
        string columnDataType = "INT";
        int testValue = 1;
        string cSharpDataType = "System.Int32";
        int expectedNumProperties = 1;
        int expectedNumRowsData = 1;
        
        string createAndPopulateStatement = $@"CREATE TABLE #TestTable ({columnName} {columnDataType});

INSERT INTO #TestTable ({columnName}) VALUES ({testValue});";
        
        // Act
        ObjectDefinition definition = await _definitionParser.ParseInsertStatementAsync(createAndPopulateStatement);
        
        // Assert
        Assert.Equal(expectedNumProperties, definition.Properties.Count);
        Assert.Equal(expectedNumRowsData, definition.Objects.Count);
        Property property = definition.Properties.First(prop => prop.Name == columnName);
        Assert.NotNull(definition.Objects[0]);
        Assert.NotNull(definition.Objects[0][property]);
        string resultValue = definition.Objects[0][property]!.ToString() ?? "";
        Assert.Equal(testValue, int.Parse(resultValue));
    }

    [Fact]
    public async Task ParseInsertStatementAsync_InsertOneRowTwoColumns_ReturnsCorrectData()
    {
        // Arrange
        int numExpectedProperties = 2;
        string idColumnName = "Id";
        string idColumnDataType = "INT";
        string nameColumnName = "Name";
        string nameColumnDataType = "NVARCHAR(50)";
        
        string idCSharpDataType = "System.Int32";
        string nameCSharpDataType = "System.String";

        int numExpectedDataRows = 1;
        int idValue = 1;
        string nameValue = "TestName";

        string createAndInsertStatement =
            @$"CREATE TABLE #TestTable ({idColumnName} {idColumnDataType}, {nameColumnName} {nameColumnDataType})

INSERT INTO #TestTable ({idColumnName}, {nameColumnName})
VALUES
({idValue}, {nameValue});";
        
        // Act
        ObjectDefinition definition = await _definitionParser.ParseInsertStatementAsync(createAndInsertStatement);
        
        // Assert
        Assert.Equal(numExpectedProperties, definition.Properties.Count);
        Assert.Equal(numExpectedDataRows, definition.Objects.Count);
        Property idProperty = definition.Properties.First(prop => prop.Name == idColumnName);
        Assert.Equal(idValue, int.Parse(definition.Objects[0][idProperty]!.ToString() ?? ""));
        Property nameProperty = definition.Properties.First(prop => prop.Name == nameColumnName);
        Assert.Equal(nameValue, definition.Objects[0][nameProperty]);
        
    }

    [Fact]
    public async Task ParseInsertStatementAsync_ThreeColumnsThreeRows_ReturnsCorrectResult()
    {
        // Arrange

        string createAndInsertStatement = @"CREATE TABLE #Users (
    UserID INT,
    UserName NVARCHAR(100),
    Email NVARCHAR(100)
);

-- Inserting mock data into the #Users table
INSERT INTO #Users (UserID, UserName, Email) VALUES
(1, 'JohnDoe', 'john.doe@example.com'),
(2, 'JaneSmith', 'jane.smith@example.com'),
(3, 'MikeBrown', 'mike.brown@example.com');";

        Property UserIdProperty = new Property
        {
            Name = "UserID",
            Type = typeof(int)
        };

        Property UserNameProperty = new Property
        {
            Name = "UserName",
            Type = typeof(string)
        };

        Property UserEmailProperty = new Property
        {
            Name = "Email",
            Type = typeof(string)
        };

        ObjectDefinition expectedResult = new()
        {
            Name = "Users",
            Properties = new()
            {
                UserIdProperty,
                UserNameProperty,
                UserEmailProperty
            },
            Objects = new()
            {
                new()
                {
                    {UserIdProperty, "1"},
                    {UserNameProperty, "JohnDoe"},
                    {UserEmailProperty, "john.doe@example.com"}
                },
                new()
                {
                    {UserIdProperty, "2"},
                    {UserNameProperty, "JaneSmith"},
                    {UserEmailProperty, "jane.smith@example.com"}
                },
                new ()
                {
                    {UserIdProperty, "3"},
                    {UserNameProperty, "MikeBrown"},
                    {UserEmailProperty, "mike.brown@example.com"}
                }
            }
        };
        
        // Act
        ObjectDefinition actualResult = await _definitionParser.ParseInsertStatementAsync(createAndInsertStatement);
        
        // Assert
        string expectedJson = Newtonsoft.Json.JsonConvert.SerializeObject(expectedResult);
        string actualJson = Newtonsoft.Json.JsonConvert.SerializeObject(actualResult);
        Assert.Equal(expectedJson, actualJson);
    }

    [Fact]
    public async Task ParseInsertStatement_TableDeclarationWithBracketsAndSpaces_ReturnsCorrectResult()
    {
        // Arrange
        string createTableStatement = "CREATE TABLE #temptable ( [SystemSettingID] int, [SystemSettingConstant] nvarchar(50), [Description] nvarchar(1000), [Value] nvarchar(1000) )";
        ObjectDefinition expectedResult = new()
        {
            Name = "temptable",
            Properties = new()
            {
              new Property()
              {
                  Name = "SystemSettingID",
                  Type = typeof(Int32)
              },
              new Property()
              {
                  Name = "SystemSettingConstant",
                  Type = typeof(string)
              },
              new Property()
              {
                  Name = "Description",
                  Type = typeof(string)
              },
              new Property()
              {
                  Name = "Value",
                  Type = typeof(string)
              }
            }
        };

        // Act
        ObjectDefinition actualResult = await _definitionParser.ParseInsertStatementAsync(createTableStatement);

        // Assert
        Assert.Equal(expectedResult, actualResult);

	}
}