namespace ResultSetIntrepreter.Services;

public class ExcelComparisionRequest
{
    public Stream ControlFile { get; set; }
    public Stream TestFile { get; set; }
}