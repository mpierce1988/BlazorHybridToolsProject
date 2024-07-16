namespace ResultSetInterpreter.Models.DTOs.ExcelComparison;

public class ExcelComparisonRequest
{
    public Stream? ControlFile { get; set; }
    public Stream? TestFile { get; set; }
}