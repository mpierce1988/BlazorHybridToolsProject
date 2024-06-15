namespace ResultSetInterpreter.Models.ExcelCSharp;

public class ExcelCSharpRow
{
    public int RowNumber { get; set; } = -1;
    public List<ExcelCSharpValue> Values { get; set; } = new();
}