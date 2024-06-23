using ResultSetInterpreter.Models.ObjectDefinition;
using ResultSetIntrepreter.Services;
using ResultSetIntrepreter.Services.Interfaces;
using Xunit.Sdk;

namespace ResultSetInterpreter.Services.Test;

public class ObjectDefinitionPrinterUnitTests
{
    private readonly IObjectDefinitionPrinter _printer;
    
    public ObjectDefinitionPrinterUnitTests()
    {
        _printer = new ObjectDefinitionPrinter();
    }

    [Fact]
    public async Task ClassDefinitionToCSharpCodeAsync_ObjectDefinitionWithName_ReturnsCorrectName()
    {
        // Arrange
        string className = "TestClass";

        ObjectDefinition definition = new()
        {
            Name = className
        };

        string expectedResult = $"public class {className}";

        // Act
        string result = await _printer.ClassDefinitionToCSharpCodeAsync(definition);
        
        // Assert
        Assert.Contains(expectedResult, result);
    }

    [Fact]
    public async Task ClassDefinitionToCSharpCodeAsync_ObjectDefinitionWithOneProperty_ReturnsProperty()
    {
        // Arrange
        string className = "TestClass";
        string propertyName = "Id";
        Type propertyType = typeof(int);
        
        ObjectDefinition definition = new()
        {
            Name = className,
            Properties = new List<Property>
            {
                new Property()
                {
                    Name = propertyName,
                    Type = propertyType
                }
            }
        };

        string expectedResult = $"public class {className} {{" + Environment.NewLine + 
                                $"public {propertyType.Name} {propertyName} " + "{ get; set; }" + Environment.NewLine +
                                "}";
        
        // Act
        string result = await _printer.ClassDefinitionToCSharpCodeAsync(definition);
        
        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task ClassDefinitionToCSharpCodeAsync_ObjectDefinitionWithThreeMixedPoperties_ReturnsCorrectResult()
    {
        // Arrange
        string className = "TestClass";

        string idPropertyName = "Id";
        Type idPropertyType = typeof(int);
        
        string isActivePropertyName = "IsActive";
        Type isActivePropertyTYpe = typeof(bool);
        
        string createdDatePropertyName = "CreatedDate";
        Type createdDatePropertyType = typeof(DateTime);

        ObjectDefinition definition = new()
        {
            Name = className,
            Properties = new()
            {
                new Property()
                {
                    Name = idPropertyName,
                    Type = idPropertyType
                },
                new Property()
                {
                    Name = isActivePropertyName,
                    Type = isActivePropertyTYpe
                },
                new Property()
                {
                    Name = createdDatePropertyName,
                    Type = createdDatePropertyType
                }
            }
        };

        string expectedResult = $"public class {className} {{" + Environment.NewLine +
                                $"public {idPropertyType.Name} {idPropertyName} " + "{ get; set; }" +
                                Environment.NewLine +
                                $"public {isActivePropertyTYpe.Name} {isActivePropertyName} " + "{ get; set; }" +
                                Environment.NewLine +
                                $"public {createdDatePropertyType.Name} {createdDatePropertyName} " + "{ get; set; }" +
                                Environment.NewLine +
                                "}";
        
        // Act
        string result = await _printer.ClassDefinitionToCSharpCodeAsync(definition);
        
        
        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task DataToCSharpListCodeAsync_OnePropertyOneDataItem_ReturnsExpectedResult()
    {
        // Arrange
        string className = "TestClass";
        string propertyName = "Id";
        Type propertyType = typeof(int);
        int dataValue = 1;
        Property idProperty = new()
        {
            Name = propertyName,
            Type = propertyType
        };

        ObjectDefinition definition = new()
        {
            Name = className,
            Properties = new()
            {
                idProperty
            },
            Objects = new()
            {
                new()
                {
                    { idProperty, dataValue.ToString() }
                }
            }
        };
        
        string expectedResult = $"List<{className}> {className}List = new() {{" + Environment.NewLine +
                                $"new {className}() {{" + Environment.NewLine +
                                $"{propertyName} = {dataValue}" + Environment.NewLine +
                                "}" + Environment.NewLine +
                                "};";
        
        // Act
        string result = await _printer.DataToCSharpListCodeAsync(definition);
        
        // Assert
        Assert.Contains(expectedResult, result);
    }
    
    [Fact]
    public async Task DataToCSharpListCodeAsync_TwoPropertiesTwoDataItems_ReturnsExpectedResult()
    {
        // Arrange
        string className = "TestClass";
        
        string firstPropertyName = "Id";
        Type firstPropertyType = typeof(int);
        
        Property firstProperty = new()
        {
            Name = firstPropertyName,
            Type = firstPropertyType
        };
        
        string secondPropertyName = "IsActive";
        Type secondPropertyType = typeof(bool);
        
        Property secondProperty = new()
        {
            Name = secondPropertyName,
            Type = secondPropertyType
        };
        
        int firstObjectFirstPropertyValue = 1;
        bool firstObjectSecondPropertyValue = true;
        
        int secondObjectFirstPropertyValue = 2;
        bool secondObjectSecondPropertyValue = false;
        
        ObjectDefinition definition = new()
        {
            Name = className,
            Properties = new()
            {
                firstProperty, secondProperty
            },
            Objects = new()
            {
                new()
                {
                    { firstProperty, firstObjectFirstPropertyValue.ToString() },
                    { secondProperty, firstObjectSecondPropertyValue.ToString() }
                },
                new ()
                {
                    { firstProperty, secondObjectFirstPropertyValue.ToString() },
                    { secondProperty, secondObjectSecondPropertyValue.ToString() }
                }
            }
        };
        
        string expectedResult = $"List<{className}> {className}List = new() {{" + Environment.NewLine +
                                $"new {className}() {{" + Environment.NewLine +
                                $"{firstPropertyName} = {firstObjectFirstPropertyValue}," + Environment.NewLine +
                                $"{secondPropertyName} = {firstObjectSecondPropertyValue}" + Environment.NewLine + 
                                "}," + Environment.NewLine +
                                $"new {className}() {{" + Environment.NewLine +
                                $"{firstPropertyName} = {secondObjectFirstPropertyValue}," + Environment.NewLine + 
                                $"{secondPropertyName} = {secondObjectSecondPropertyValue}" + Environment.NewLine + 
                                "}" + Environment.NewLine +
                                "};";
        
        // Act
        string result = await _printer.DataToCSharpListCodeAsync(definition);
        
        // Assert
        Assert.Contains(expectedResult, result);
    }

    [Fact]
    public async Task DataToCSharpListCodeAsync_StringProperty_ResultIsEncapsulatedInQuotes()
    {
        // Arrange
        string className = "TestClass";
        string propertyName = "FirstName";
        Type propertyType = typeof(string);
        string dataValue = "Test";
        Property property = new()
        {
            Name = propertyName,
            Type = propertyType
        };

        ObjectDefinition definition = new()
        {
            Name = className,
            Properties = new()
            {
                property
            },
            Objects = new()
            {
                new()
                {
                    { property, dataValue.ToString() }
                }
            }
        };
        
        string expectedResult = $"List<{className}> {className}List = new() {{" + Environment.NewLine +
                                $"new {className}() {{" + Environment.NewLine +
                                $"{propertyName} = \"{dataValue}\"" + Environment.NewLine +
                                "}" + Environment.NewLine +
                                "};";
        
        // Act
        string result = await _printer.DataToCSharpListCodeAsync(definition);
        
        // Assert
        Assert.Contains(expectedResult, result);
    }
}