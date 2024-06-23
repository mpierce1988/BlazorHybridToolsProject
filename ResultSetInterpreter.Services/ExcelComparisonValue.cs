namespace ResultSetIntrepreter.Services;

public class ExcelComparisonValue
{
    public string SheetName { get; set; } = string.Empty;
    public string RowLocation { get; set; } = string.Empty;
    public string ColumnLocation { get; set; } = string.Empty;
    public object? ControlValue { get; set; }
    public object? TestValue { get; set; }
}