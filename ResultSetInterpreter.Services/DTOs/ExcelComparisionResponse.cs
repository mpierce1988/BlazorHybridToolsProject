namespace ResultSetIntrepreter.Services.DTOs;

public class ExcelComparisionResponse : BaseResponse
{
    public List<ExcelComparisonValue> Results { get; set; } = new();
    public bool IsIdentical => Results.Count == 0;
}