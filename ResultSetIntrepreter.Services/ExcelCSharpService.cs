using System.Globalization;
using System.Text;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ResultSetInterpreter.Models.ExcelCSharp;
using ResultSetInterpreter.Services.Interfaces;
using ResultSetInterpreter.Services.Utilities;

namespace ResultSetInterpreter.Services;

public class ExcelCSharpService : IExcelToCSharpService
{
    private readonly IExcelCSharpWorkbookParser _excelCSharpWorkbookParser;
    
    public ExcelCSharpService(IExcelCSharpWorkbookParser excelCSharpWorkbookParser)
    {
        _excelCSharpWorkbookParser = excelCSharpWorkbookParser;
    }
    
    #region Public Methods
    public async Task<String> ConvertExcelToCSharpAsync(Stream excelStream)
    {
        ExcelCSharpWorkbook excelWorkbook = await _excelCSharpWorkbookParser.ParseExcelCSharpWorkbookAsync(excelStream);
        
        StringBuilder codeBuilder = new();

        foreach (ExcelCSharpSheet sheet in excelWorkbook.Sheets)
        {
            if (!IsDataInSheet(sheet))
            {
                continue;
            }
            
            CreateClassDeclaration(codeBuilder, sheet);
            
            // Add line break between class declaration and list of class objects
            codeBuilder.AppendLine();
            
            // Create list of class objects
            codeBuilder = CreateClassList(codeBuilder, sheet);
        }

        return codeBuilder.ToString();
    }

    #endregion
    
    #region Private Methods
    
    private void CreateClassDeclaration(StringBuilder codeBuilder, ExcelCSharpSheet sheet)
    {
        codeBuilder.AppendLine($"public class {sheet.Name}");
        codeBuilder.AppendLine("{");

            
        foreach (var value in sheet.Rows[1].Values)
        {
            codeBuilder.AppendLine($"   public {value.CSharpColumnNameType.Type} {value.CSharpColumnNameType.ColumnName} {{ get; set; }}");
        }

        codeBuilder.AppendLine("}");
    }
    
    private StringBuilder CreateClassList(StringBuilder codeBuilder, ExcelCSharpSheet sheet)
    {
        codeBuilder.AppendLine($"public List<{sheet.Name}> {sheet.Name}List = new List<{sheet.Name}>() {{");

        foreach (var row in sheet.Rows)
        {
            codeBuilder.AppendLine($"   new {sheet.Name} {{");
            foreach (var value in row.Values)
            {
                CreatePropertyAssignment(codeBuilder, value);
            }

            // remove last trailing comma
            int propertyLastCommaIndex = codeBuilder.ToString().LastIndexOf(',');
            codeBuilder = codeBuilder.Remove(propertyLastCommaIndex, 1);
            codeBuilder.AppendLine("    },");
        }
        // remove last trailing comma
        int sheetLastCommaIndex = codeBuilder.ToString().LastIndexOf(',');
        codeBuilder = codeBuilder.Remove(sheetLastCommaIndex, 1);
        codeBuilder.AppendLine("};");
        return codeBuilder;
    }
    
    private void CreatePropertyAssignment(StringBuilder codeBuilder, ExcelCSharpValue value)
    {
        if (value.CSharpColumnNameType.Type == "System.DateTime?")
        {
            codeBuilder.AppendLine($"       {value.CSharpColumnNameType.ColumnName} = DateTime.Parse(\"{ExcelCSharpUtility.ParseDateToString(value)}\", CultureInfo.InvariantCulture),");
        }
        else if (value.CSharpColumnNameType.Type == "System.Int32?")
        {
            codeBuilder.AppendLine($"       {value.CSharpColumnNameType.ColumnName} = {int.Parse(value.Value?.ToString() ?? "0")},");
        }
        else
        {
            codeBuilder.AppendLine($"       {value.CSharpColumnNameType.ColumnName} = \"{value.Value?.ToString()}\",");
        }
    }
    
    /// <summary>
    /// Validates the ExcelCSharpSheet has at least one data row
    /// </summary>
    /// <param name="sheet">ExcelCSharpSheet to validate</param>
    /// <returns>True if the ExcelCSharpSheet has data</returns>
    private bool IsDataInSheet(ExcelCSharpSheet sheet)
    {
        return sheet.Rows.Count > 2;
    }
    
    #endregion
    
}



