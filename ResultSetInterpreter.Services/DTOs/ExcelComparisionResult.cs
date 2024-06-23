namespace ResultSetIntrepreter.Services;

public class ExcelComparisionResult
{
    public List<ExcelComparisonValue> Results { get; set; } = new();
    public bool IsValid => Results.Count == 0;
}