using System.Collections.Specialized;
using System.Text.RegularExpressions;
using ResultSetInterpreter.Models.ObjectDefinition;
using ResultSetIntrepreter.Services.Interfaces;

namespace ResultSetIntrepreter.Services;

public class StringToObjectDefinitionParser : IStringToObjectDefinitionParser
{
    public async Task<ObjectDefinition> ParseInsertStatementAsync(string insertStatement)
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
        string? insertDataStatement = statements.Count() > 1 ? string.Concat("INSERT INTO", statements[1]) : null;
        
        // Remove new line or return characters from the insert statement
        // insertStatement = insertStatement.Replace("\n", " ").Replace("\r", " ");

        // Get the table name and column names with types from the temple table create statement
        //string tableCreateMatchRegex = @"CREATE TABLE #(?<tableName>\w+)\s*\((?<columns>.+?)\);";
        RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline;

        Regex tableCreateRegex = new Regex(@"^CREATE TABLE #(?<tableName>\w+)\s*\(\s?(?<columns>[\s\S]*?)\);?$", options);
        
        Match tableCreateMatch = tableCreateRegex.Match(tableDefinition);
        
        if(tableCreateMatch.Success)
        {
            result.Name = tableCreateMatch.Groups["tableName"].Value.Trim();

            string columnsText = tableCreateMatch.Groups["columns"].Value.Trim();

            if (!string.IsNullOrWhiteSpace(columnsText))
            {
                result.Properties = GetProperties(columnsText);
            }
        }
        
        // Get the Insert statement
        Regex insertRegex = new Regex(@$"INSERT INTO #{result.Name}\s\((?<columns>.+?)\)\s?\n?VALUES\s*?\((?<values>[\s\S]+)\);?", options);

        if (insertDataStatement is not null)
        {
           Match insertMatch = insertRegex.Match(insertDataStatement);

            if (insertMatch.Success)
            {
                // Get the columns to determine the order of the values
                string columnsText = insertMatch.Groups["columns"].Value.Trim();
                // split the columns text by comma
                string[] columns = columnsText.Split(',');
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
                
                // Get the values string
                string valuesString = insertMatch.Groups["values"].Value.Trim();
                
                // remove new line and return characters
                valuesString = valuesString.Replace("\n", " ").Replace("\r", " ");
                
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
                        Property prop = orderedProperties[i];
                        string? value = values[i].Trim();
                        // Remove leading and trailing apostrophes
                        value = value.Trim('\'');
                        value = value.Trim('(');
                        value = value.Trim(')');
                        propertyValues.Add(prop, value);
                    }
                
                    result.Objects.Add(propertyValues);
                }
            }
        
        }
        
        return result;
    }

    private List<Property> GetProperties(string columnsText)
    {
        List<Property> result = new();
        
        // Regex tp extract column names and their data types
        // Really, this is two separate Regex expressions seperated by an OR | operator

        Regex columnRegex = new Regex(@"\[(?<columnName>[^\]]+)\]\s(?<dataType>[^,]+)(?:,|$)|(?<columnName>\w+)\s(?<dataType>[^,]+)(?:,|$)");

        foreach (Match columnMatch in columnRegex.Matches(columnsText))
        {
            string columnName = columnMatch.Groups["columnName"].Value.Trim();
            string dataType = columnMatch.Groups["dataType"].Value.Trim();

            Property property = new();
            property.Name = columnName;
            property.Type = GetCSharpType(dataType);                
                
            result.Add(property);
        }

        return result;
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
        
        // The default type is string, as this can be used for to represent any data type
        Type result = typeof(string);

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
}