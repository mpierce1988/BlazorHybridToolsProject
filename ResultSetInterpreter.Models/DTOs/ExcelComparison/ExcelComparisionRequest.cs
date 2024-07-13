namespace ResultSetInterpreter.Models.DTOs.ExcelComparison;

public class ExcelComparisionRequest
{
    public Stream ControlFile { get; set; }
    public Stream TestFile { get; set; }
}