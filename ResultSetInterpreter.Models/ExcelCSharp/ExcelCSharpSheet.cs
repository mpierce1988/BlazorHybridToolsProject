namespace ResultSetInterpreter.Models.ExcelCSharp;

public class ExcelCSharpSheet
{
    public string Name { get; set; } = string.Empty;
    public List<ExcelCSharpRow> Rows { get; set; } = new();
        
}