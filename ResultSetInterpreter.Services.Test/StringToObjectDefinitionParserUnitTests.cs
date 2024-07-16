using System.Collections.Specialized;
using ResultSetInterpreter.Models.ObjectDefinition;

namespace ResultSetInterpreter.Services.Test;

public class StringToObjectDefinitionParserUnitTests
{
    private readonly StringToObjectDefinitionParser _definitionParser = new();

    [Fact]
    public void ParseInsertStatementAsync_CreateTableName_RetrieveCorrectName()
    {
        // Arrange
        string tableName = "TestTable";
        string tempTableDeclaration = $"CREATE TABLE #{tableName} (Id INT, Name NVARCHAR(50));";
        
        // Act
        ObjectDefinition definition = _definitionParser.ParseInsertStatement(tempTableDeclaration);
        
        Assert.Equal(tableName, definition.Name);
    }

    [Fact]
    public void ParseInsertStatementAsync_CreateTableIntIDColumn_ReturnCorrectProperty()
    {
        // Arrange
        string columnName = "Id";
        string columnType = "INT";
        string cSharpDataType = "system.int32";
        string tempTableDeclaration = $"CREATE TABLE #TestTable ({columnName} {columnType});";
        
        // Act
        ObjectDefinition definition =  _definitionParser.ParseInsertStatement(tempTableDeclaration);
        
        // Assert
        Assert.Single(definition.Properties);
        Assert.NotNull(definition.Properties.First());
        Assert.Equal(columnName, definition.Properties.First().Name);
        Assert.NotNull(definition.Properties.First().Type);
        Assert.Equal(cSharpDataType.ToLowerInvariant(), definition.Properties.First().Type!.ToString().ToLowerInvariant());
    }

    [Fact]
    public void ParseInsertStatementAsync_CreateTableNameNVarCharColumn_ReturnsCorrectProperties()
    {
        // Arrange
        string columnName = "Name";
        string columnType = "NVARCHAR(50)";
        string cSharpDataType = "System.String";
        int totalProperties = 2;
        string tempTableDeclaration = @$"CREATE TABLE #TestTable (Id INT, {columnName} {columnType});";
        
        // Act
        ObjectDefinition definition = _definitionParser.ParseInsertStatement(tempTableDeclaration);
        
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
    public void ParseInsertStatementAsync_InsertOneRowOneColumn_ReturnsCorrectData()
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
        ObjectDefinition definition = _definitionParser.ParseInsertStatement(createAndPopulateStatement);
        
        // Assert
        Assert.Equal(expectedNumProperties, definition.Properties.Count);
        Assert.Equal(expectedNumRowsData, definition.Objects.Count);
        Property property = definition.Properties.First(prop => prop.Name == columnName);
        Assert.NotNull(property);
        Assert.Equal(cSharpDataType, property.Type!.ToString());
        Assert.NotNull(definition.Objects[0]);
        Assert.NotNull(definition.Objects[0][property]);
        string resultValue = definition.Objects[0][property]!.ToString() ?? "";
        Assert.Equal(testValue, int.Parse(resultValue));
    }

    [Fact]
    public void ParseInsertStatementAsync_InsertOneRowTwoColumns_ReturnsCorrectData()
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
            @$"CREATE TABLE #TestTable ({idColumnName} {idColumnDataType}, {nameColumnName} {nameColumnDataType});

INSERT INTO #TestTable ({idColumnName}, {nameColumnName})
VALUES
({idValue}, {nameValue});";
        
        // Act
        ObjectDefinition definition = _definitionParser.ParseInsertStatement(createAndInsertStatement);
        
        // Assert
        Assert.Equal(numExpectedProperties, definition.Properties.Count);
        Assert.Equal(numExpectedDataRows, definition.Objects.Count);
        
        // Check Id property
        Property idProperty = definition.Properties.First(prop => prop.Name == idColumnName);
        Assert.NotNull(idProperty);
        Assert.Equal(idCSharpDataType, idProperty.Type!.ToString());
        // Check Id value in first row
        Assert.Equal(idValue, int.Parse(definition.Objects[0][idProperty]!.ToString() ?? ""));
        
        // Check Name property
        Property nameProperty = definition.Properties.First(prop => prop.Name == nameColumnName);
        Assert.NotNull(nameProperty);
        Assert.Equal(nameCSharpDataType, nameProperty.Type!.ToString());
        // Check Name value in first row
        Assert.Equal(nameValue, definition.Objects[0][nameProperty]);
        
    }

    [Fact]
    public void ParseInsertStatementAsync_ThreeColumnsThreeRows_ReturnsCorrectResult()
    {
        // Arrange

        string createAndInsertStatement = @"CREATE TABLE #Users (UserID INT, UserName NVARCHAR(100), Email NVARCHAR(100));

-- Inserting mock data into the #Users table
INSERT INTO #Users (UserID, UserName, Email) 
VALUES
(1, 'JohnDoe', 'john.doe@example.com'),
(2, 'JaneSmith', 'jane.smith@example.com'),
(3, 'MikeBrown', 'mike.brown@example.com');";

        Property userIdProperty = new Property
        {
            Name = "UserID",
            Type = typeof(int)
        };

        Property userNameProperty = new Property
        {
            Name = "UserName",
            Type = typeof(string)
        };

        Property userEmailProperty = new Property
        {
            Name = "Email",
            Type = typeof(string)
        };

        ObjectDefinition expectedResult = new()
        {
            Name = "Users",
            Properties = new()
            {
                userIdProperty,
                userNameProperty,
                userEmailProperty
            },
            Objects = new()
            {
                new()
                {
                    {userIdProperty, "1"},
                    {userNameProperty, "JohnDoe"},
                    {userEmailProperty, "john.doe@example.com"}
                },
                new()
                {
                    {userIdProperty, "2"},
                    {userNameProperty, "JaneSmith"},
                    {userEmailProperty, "jane.smith@example.com"}
                },
                new ()
                {
                    {userIdProperty, "3"},
                    {userNameProperty, "MikeBrown"},
                    {userEmailProperty, "mike.brown@example.com"}
                }
            }
        };
        
        // Act
        ObjectDefinition actualResult = _definitionParser.ParseInsertStatement(createAndInsertStatement);
        
        // Assert
        string expectedJson = JsonConvert.SerializeObject(expectedResult);
        string actualJson = JsonConvert.SerializeObject(actualResult);
        Assert.Equal(expectedJson, actualJson);
    }

    [Fact]
    public void ParseInsertStatement_TableDeclarationWithBracketsAndSpaces_ReturnsCorrectResult()
    {
        // Arrange
        string createTableStatement = "CREATE TABLE #temptable ( [SettingID] int, [Constant] nvarchar(50), [Description] nvarchar(1000), [Value] nvarchar(1000) )";
        ObjectDefinition expectedResult = new()
        {
            Name = "temptable",
            Properties = new()
            {
              new Property()
              {
                  Name = "SettingID",
                  Type = typeof(Int32)
              },
              new Property()
              {
                  Name = "Constant",
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
        ObjectDefinition actualResult = _definitionParser.ParseInsertStatement(createTableStatement);

        // Assert
        Assert.Equivalent(expectedResult, actualResult);
	}
    
    [Fact]
    public void ParseInsertStatement_TableWithOneRowDataBracketsSpaces_ReturnsCorrectResult()
    {
        // Arrange
        string createTableStatement = @"CREATE TABLE #temptable ( [SettingID] int, [Constant] nvarchar(50), [Description] nvarchar(1000), [Value] nvarchar(1000) )

INSERT INTO #temptable ( [SettingID], [Constant], [Description], [Value] )
VALUES
(1, 'Test', 'Test Description', 'Test Value');";
        
        Property settingIdProperty = new Property
        {
            Name = "SettingID",
            Type = typeof(int)
        };
        
        Property constantProperty = new Property
        {
            Name = "Constant",
            Type = typeof(string)
        };
        
        Property descriptionProperty = new Property
        {
            Name = "Description",
            Type = typeof(string)
        };
        
        Property valueProperty = new Property
        {
            Name = "Value",
            Type = typeof(string)
        };
        
        ObjectDefinition expectedResult = new()
        {
            Name = "temptable",
            Properties = new()
            {
                settingIdProperty,
                constantProperty,
                descriptionProperty,
                valueProperty
            },
            Objects = new List<OrderedDictionary>()
            {
                new OrderedDictionary()
                {
                    {settingIdProperty, "1"},
                    {constantProperty, "Test"},
                    {descriptionProperty, "Test Description"},
                    {valueProperty, "Test Value"}
                }
            }
        };

        // Act
        ObjectDefinition actualResult = _definitionParser.ParseInsertStatement(createTableStatement);

        // Assert
        // Assert.Equivalent(expectedResult, actualResult);
        string expectedJson = JsonConvert.SerializeObject(expectedResult);
        string actualJson = JsonConvert.SerializeObject(actualResult);
        
        Assert.Equal(expectedJson, actualJson);
    }

    [Fact]
    public void ParseInsertStatement_TableWithTwoRowsDataBracketsSpaces_ReturnsCorrectResult()
    {
        // Arrange
        string createTableStatement = @"CREATE TABLE #temptable ( [SettingID] int, [Constant] nvarchar(50), [Description] nvarchar(1000), [Value] nvarchar(1000) )

INSERT INTO #temptable ( [SettingID], [Constant], [Description], [Value] )
VALUES
(1, 'Test', 'Test Description', 'Test Value'),
(2, 'SecondTest', 'Second Description', 'Second Value');";
        
        Property settingIdProperty = new Property
        {
            Name = "SettingID",
            Type = typeof(int)
        };
        
        Property constantProperty = new Property
        {
            Name = "Constant",
            Type = typeof(string)
        };
        
        Property descriptionProperty = new Property
        {
            Name = "Description",
            Type = typeof(string)
        };
        
        Property valueProperty = new Property
        {
            Name = "Value",
            Type = typeof(string)
        };
        
        ObjectDefinition expectedResult = new()
        {
            Name = "temptable",
            Properties = new()
            {
                settingIdProperty,
                constantProperty,
                descriptionProperty,
                valueProperty
            },
            Objects = new List<OrderedDictionary>()
            {
                new OrderedDictionary()
                {
                    {settingIdProperty, "1"},
                    {constantProperty, "Test"},
                    {descriptionProperty, "Test Description"},
                    {valueProperty, "Test Value"}
                },
                new OrderedDictionary()
                {
                    {settingIdProperty, "2"},
                    {constantProperty, "SecondTest"},
                    {descriptionProperty, "Second Description"},
                    {valueProperty, "Second Value"}
                }
            }
        };

        // Act
        ObjectDefinition actualResult = _definitionParser.ParseInsertStatement(createTableStatement);

        // Assert
        // Assert.Equivalent(expectedResult, actualResult);
        string expectedJson = JsonConvert.SerializeObject(expectedResult);
        string actualJson = JsonConvert.SerializeObject(actualResult);
        
        Assert.Equal(expectedJson, actualJson);
        
    }

    [Fact]
    public void ParseInsertStatement_TableWithDataNoSpaces_ReturnsCorrectResult()
    {
        // Arrange
        string createTableStatement = @"-- Create temporary table
CREATE TABLE #TempTable (
    [ID] INT,
    [Name] NVARCHAR(50),
    [Age] INT
);

-- Insert statement
INSERT INTO #TempTable ([ID], [Name], [Age])
VALUES
    (1, 'John Doe', 30),
    (2, 'Jane Smith', 25);";
        Property idProperty = new Property
        {
            Name = "ID",
            Type = typeof(int)
        };
        
        Property nameProperty = new Property
        {
            Name = "Name",
            Type = typeof(string)
        };
        
        Property ageProperty = new Property
        {
            Name = "Age",
            Type = typeof(int)
        };
        
        ObjectDefinition expectedResult = new()
        {
            Name = "TempTable",
            Properties = new()
            {
                idProperty,
                nameProperty,
                ageProperty
            },
            Objects = new List<OrderedDictionary>()
            {
                new OrderedDictionary()
                {
                    {idProperty, "1"},
                    {nameProperty, "John Doe"},
                    {ageProperty, "30"}
                },
                new OrderedDictionary()
                {
                    {idProperty, "2"},
                    {nameProperty, "Jane Smith"},
                    {ageProperty, "25"}
                }
            }
        };
        
        // Act
        ObjectDefinition actualResult = _definitionParser.ParseInsertStatement(createTableStatement);
        
        // Assert
        string expectedResultJson = JsonConvert.SerializeObject(expectedResult);
        string actualResultJson = JsonConvert.SerializeObject(actualResult);
        
        Assert.Equal(expectedResultJson, actualResultJson);
    }
    
    [Fact]
    public void ParseInsertStatement_TableWithStringPrefixedByN_ReturnsCorrectResult()
    {
        // Arrange
        string createTableStatement = @"-- Create temporary table
CREATE TABLE #TempTable (
    [ID] INT,
    [Name] NVARCHAR(50),
    [Age] INT
);

-- Insert statement
INSERT INTO #TempTable ([ID], [Name], [Age])
VALUES
    (1, N'John Doe', 30),
    (2, N'Jane Smith', 25);";
        Property idProperty = new Property
        {
            Name = "ID",
            Type = typeof(int)
        };
        
        Property nameProperty = new Property
        {
            Name = "Name",
            Type = typeof(string)
        };
        
        Property ageProperty = new Property
        {
            Name = "Age",
            Type = typeof(int)
        };
        
        ObjectDefinition expectedResult = new()
        {
            Name = "TempTable",
            Properties = new()
            {
                idProperty,
                nameProperty,
                ageProperty
            },
            Objects = new List<OrderedDictionary>()
            {
                new OrderedDictionary()
                {
                    {idProperty, "1"},
                    {nameProperty, "John Doe"},
                    {ageProperty, "30"}
                },
                new OrderedDictionary()
                {
                    {idProperty, "2"},
                    {nameProperty, "Jane Smith"},
                    {ageProperty, "25"}
                }
            }
        };
        
        // Act
        ObjectDefinition actualResult = _definitionParser.ParseInsertStatement(createTableStatement);
        
        // Assert
        string expectedResultJson = JsonConvert.SerializeObject(expectedResult);
        string actualResultJson = JsonConvert.SerializeObject(actualResult);
        
        Assert.Equal(expectedResultJson, actualResultJson);
    }
}