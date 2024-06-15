namespace ResultSetInterpreter.Models.ExcelCSharp;

public class ExcelCSharpValue
{
    public ExcelCSharpColumnNameType CSharpColumnNameType { get; set; } = new();
    public object? Value { get; set; }
}