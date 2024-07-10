namespace ResultSetIntrepreter.Services.DTOs;

public class ExcelComparisionRequest
{
    public Stream ControlFile { get; set; }
    public Stream TestFile { get; set; }
}