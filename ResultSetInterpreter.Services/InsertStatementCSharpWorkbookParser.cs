using System.Collections.Specialized;
using System.Text.RegularExpressions;
using ResultSetInterpreter.Models.ExcelCSharp;
using ResultSetInterpreter.Services.Interfaces;

namespace ResultSetIntrepreter.Services;

public class InsertStatementCSharpWorkbookParser : IInsertStatementCSharpWorkbookParser
{
    public async Task<ExcelCSharpWorkbook> ParseInsertStatementCSharpWorkbookAsync(string insertStatement)
    {
        ExcelCSharpWorkbook workbook = new();
        
        // Find each unique table name in the insert statement
        var tableNames = GetTableNames(insertStatement);

        foreach (String tableName in tableNames)
        {
            // Create a new ExcelCSharpSheet
            ExcelCSharpSheet sheet = new();
            sheet.Name = tableName;
            
            // Get the column names and data types from the insert statement
            OrderedDictionary columnNamesAndType = GetColumnNamesAndType(insertStatement, tableName);
            

            workbook.Sheets.Add(sheet);
        }


        return workbook;
    }

    private OrderedDictionary GetColumnNamesAndType(string insertStatement, string tableName)
    {
        OrderedDictionary columnNameAndType = new();
        
        // Get column names and data types as a single string, after the DECLARE #tableName statement
        // Match on this regex and save the columns string to a group called "columns"
        var matches = Regex.Matches(insertStatement, $@"CREATE TABLE #{tableName} (?<columns>.+)");

        if (matches.Count > 0)
        {
            // There should only be one match
            var columns = matches[0].Groups["columns"].Value;
            
            // Extract column names and data types
            var columnNames = Regex.Matches(columns, @"(?<column>\w+)\s+(?<type>\w+(\(\d+\))?)");

            foreach (Match columnMatch in columnNames)
            {
                var columnName = columnMatch.Groups["column"].Value;
                var columnType = columnMatch.Groups["type"].Value;
            }
        }

        return columnNameAndType;
    }

    private List<string> GetTableNames(string insertStatement)
    {
        var tableNames = new List<string>();
        var matches = Regex.Matches(insertStatement, @"DECLARE #(?<table>\w+)");
        foreach (Match match in matches)
        {
            var tableName = match.Groups["table"].Value;
            if (!tableNames.Contains(tableName))
            {
                tableNames.Add(tableName);
            }
        }

        return tableNames;
    }
}