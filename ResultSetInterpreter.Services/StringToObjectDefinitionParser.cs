using System.Collections.Specialized;
using System.Text.RegularExpressions;
using ResultSetInterpreter.Models.ObjectDefinition;
using ResultSetIntrepreter.Services.Interfaces;

namespace ResultSetIntrepreter.Services;

public class StringToObjectDefinitionParser : IStringToObjectDefinitionParser
{
    #region Fields
    
    private readonly RegexOptions _regexOptions = RegexOptions.IgnoreCase | RegexOptions.Multiline;
    
    #endregion
    
    #region Constructor
    
    public StringToObjectDefinitionParser()
    {
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Takes a string of an insert statement (CREATE TABLE and INSERT INTO) and returns an
    /// ObjectDefinition that represents the table and data
    /// </summary>
    /// <param name="insertStatement">
    /// The string of the insert statement
    /// </param>
    /// <returns>
    /// ObjectDefinition representing the table and data
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the CREATE TABLE or INSERT INTO statement is invalid
    /// </exception>
    public ObjectDefinition ParseInsertStatement(string insertStatement)
    {
        ObjectDefinition result = new();

        // split insert statement into table definition and insert statement
        string[] statements = insertStatement.Split("INSERT INTO");
        
        if (statements.Count() > 2)
        {
            throw new ArgumentException(
                "Insert Statement contains multiple Table and/or Insert definitions. Only one is allowed");
        }
        
        string tableDefinition = statements[0];
        // Add back in the INSERT INTO that was removed by the split
        string? insertDataStatement = statements.Count() > 1 ? string.Concat("INSERT INTO", statements[1]) : null;

        PopulateTableDefinition(tableDefinition, result);

        if (insertDataStatement is null) return result;

        PopulateDataDefinition(insertDataStatement, result);

        return result;
    }
    
    #endregion
    
    #region Private Methods

    /// <summary>
    /// Takes a string of the insert statement and a result ObjectDefinition and populates the Objects (Data) list
    /// </summary>
    /// <param name="insertDataStatement">
    /// The string of the insert statement
    /// </param>
    /// <param name="result">
    /// The ObjectDefinition to populate
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown if the insert statement is invalid
    /// </exception>
    private void PopulateDataDefinition(string insertDataStatement, ObjectDefinition result)
    {
        // Get the Insert statement
        Regex insertRegex =
            new Regex(@$"INSERT INTO #{result.Name}\s\((?<columns>.+?)\)\s?\n?VALUES\s*?\((?<values>[\s\S]+)\);?",
                _regexOptions);


        Match insertMatch = insertRegex.Match(insertDataStatement);

        if (!insertMatch.Success)
            throw new ArgumentException(
                "Invalid Insert Statement. Must start with INSERT INTO #tableName (column1, column2) VALUES (value1, value2);");


        // Get the columns to determine the order of the values
        string columnsText = insertMatch.Groups["columns"].Value.Trim();
        // split the columns text by comma
        string[] columns = columnsText.Split(',');
        
        var orderedProperties = GetOrderedProperties(columns, result);

        // Get the values string
        string valuesString = insertMatch.Groups["values"].Value.Trim();

        // Split the values by "), (" to get each item
        string[] items = valuesString.Split("),");

        // Loop through each item
        foreach (string item in items)
        {
            // Create a dictionary to hold the property and the value
            OrderedDictionary propertyValues = new();

            // Split the values by comma
            string[] values = item.Split(',');

            // Loop through each ordered property, and grab the corresponding value
            for (int i = 0; i < orderedProperties.Count; i++)
            {
                string value = values[i].Trim();
                
                if(string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Value cannot be empty");
                
                Property prop = orderedProperties[i];

                value = GetTrimmedValue(value, prop);
                
                propertyValues.Add(prop, value);
            }

            result.Objects.Add(propertyValues);
        }
    }

    /// <summary>
    /// Takes a string of the table definition and a result ObjectDefinition and populates the Name and Properties
    /// </summary>
    /// <param name="tableDefinition">
    /// The string of the table definition
    /// </param>
    /// <param name="result">
    /// The ObjectDefinition to populate
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown if the table definition is invalid
    /// </exception>
    private void PopulateTableDefinition(string tableDefinition, ObjectDefinition result)
    {
        Regex tableCreateRegex =
            new Regex(@"^CREATE TABLE #(?<tableName>\w+)\s*\(\s?(?<columns>[\s\S]*?)\);?$", _regexOptions);

        Match tableCreateMatch = tableCreateRegex.Match(tableDefinition);

        if (!tableCreateMatch.Success)
            throw new ArgumentException(
                "Invalid Table Definition. Must start with CREATE TABLE #tableName (column1 datatype, column2 datatype);");


        result.Name = tableCreateMatch.Groups["tableName"].Value.Trim();

        string columnString = tableCreateMatch.Groups["columns"].Value.Trim();

        if (string.IsNullOrEmpty(columnString))
            throw new ArgumentException("Table definition must contain at least one column");

        result.Properties = GetProperties(columnString);
    }

    /// <summary>
    /// Takes a raw value from the insert statement and a Property and returns the value trimmed of any extra characters
    /// </summary>
    /// <param name="value">
    /// The raw value from the insert statement
    /// </param>
    /// <param name="property">
    /// The Property that the value corresponds to
    /// </param>
    /// <returns>
    /// The trimmed value
    /// </returns>
    private string GetTrimmedValue(string value, Property property)
    {
        value = value.Trim();
        // Remove leading and trailing apostrophes
        if (property.Type == typeof(string))
        {
            value = value.TrimStart('N');
        }

        value = value.Trim('\'');
        value = value.TrimStart('(');
        value = value.TrimEnd(')');

        return value;
    }

    /// <summary>
    /// Takes a string of column names and a result ObjectDefinition (containing Properties) and returns
    /// a List of Properties in the order they appear in the columns string
    /// </summary>
    /// <param name="columns">
    /// The string of column names
    /// </param>
    /// <param name="result">
    /// The ObjectDefinition containing the Properties
    /// </param>
    /// <returns>
    /// List of Properties in the order they appear in the columns string
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the column name in the insert statement does not appear as a Property in the ObjectDefinition
    /// </exception>
    private List<Property> GetOrderedProperties(string[] columns, ObjectDefinition result)
    {
        // Create a List to hold the columns in order
        List<Property> orderedProperties = new();

        for (int i = 0; i < columns.Length; i++)
        {
            string columnName = columns[i].Trim();
            // Remove the square brackets, if they are present
            columnName = columnName.Trim('[', ']');
            Property? property = result.Properties.FirstOrDefault(prop => prop.Name == columnName);

            if (property == null)
            {
                throw new ArgumentException(
                    "Insert statement contains a column not declared in the table create statement");
            }

            orderedProperties.Add(property);
        }

        return orderedProperties;
    }

    /// <summary>
    /// Takes a string of column names and data types and returns a List of Properties 
    /// </summary>
    /// <param name="columnsText">
    /// The string of columns and data types.
    /// </param>
    /// <returns>
    /// List of Properties
    /// </returns>
    private List<Property> GetProperties(string columnsText)
    {
        List<Property> properties = new();

        // Regex tp extract column names and their data types
        // Really, this is two separate Regex expressions seperated by an OR | operator,
        // to handle with or without square brackets [ ]

        Regex columnRegex =
            new Regex(
                @"\[(?<columnName>[^\]]+)\]\s(?<dataType>[^,]+)(?:,|$)|(?<columnName>\w+)\s(?<dataType>[^,]+)(?:,|$)");

        foreach (Match columnMatch in columnRegex.Matches(columnsText))
        {
            string columnName = columnMatch.Groups["columnName"].Value.Trim();
            string dataType = columnMatch.Groups["dataType"].Value.Trim();

            Property property = new();
            property.Name = columnName;
            property.Type = GetCSharpType(dataType);

            properties.Add(property);
        }

        return properties;
    }

    /// <summary>
    /// Takes a string Sql Data Type name and returns the equivalent C# Data Type.
    /// Information from https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
    /// </summary>
    /// <param name="sqlType">String name of the Sql Data type</param>
    /// <returns>Corresponding C# Data Type</returns>
    private Type GetCSharpType(string sqlType)
    {
        // The data type might have the length or precision. We need to remove that
        sqlType = sqlType.Split('(')[0].ToLowerInvariant();
        
        Type result;

        switch (sqlType)
        {
            case "bigint":
                result = typeof(Int64);
                break;
            case "binary":
                result = typeof(Byte[]);
                break;
            case "bit":
                result = typeof(bool);
                break;
            case "date":
                result = typeof(DateTime);
                break;
            case "datetime":
                result = typeof(DateTime);
                break;
            case "datetime2":
                result = typeof(DateTime);
                break;
            case "datetimeoffset":
                result = typeof(DateTimeOffset);
                break;
            case "decimal":
                result = typeof(Decimal);
                break;
            case "varbinary(max)":
                result = typeof(Byte[]);
                break;
            case "float":
                result = typeof(Double);
                break;
            case "image":
                result = typeof(Byte[]);
                break;
            case "int":
                result = typeof(Int32);
                break;
            case "money":
                result = typeof(Decimal);
                break;
            case "nchar":
                result = typeof(string);
                break;
            case "ntext":
                result = typeof(string);
                break;
            case "numeric":
                result = typeof(Decimal);
                break;
            case "nvarchar":
                result = typeof(string);
                break;
            case "real":
                result = typeof(Single);
                break;
            case "rowversion":
                result = typeof(Byte[]);
                break;
            case "smalldatetime":
                result = typeof(DateTime);
                break;
            case "smallint":
                result = typeof(Int16);
                break;
            case "smallmoney":
                result = typeof(Decimal);
                break;
            case "sql_variant":
                result = typeof(Object);
                break;
            case "text":
                result = typeof(string);
                break;
            case "time":
                result = typeof(TimeSpan);
                break;
            case "timestamp":
                result = typeof(Byte[]);
                break;
            case "tinyint":
                result = typeof(Byte);
                break;
            case "uniqueidentifier":
                result = typeof(Guid);
                break;
            case "varbinary":
                result = typeof(Byte[]);
                break;
            case "varchar":
                result = typeof(string);
                break;
            case "xml":
                result = typeof(string);
                break;
            default:
                result = typeof(string);
                break;
        }

        return result;
    }
    
    #endregion
}