using ResultSetInterpreter.Models.Workbook;
using ResultSetIntrepreter.Services;
using ResultSetIntrepreter.Services.DTOs;

namespace ResultSetInterpreter.Models.DTOs.ExcelComparison;

public class ExcelComparisonResponse : BaseResponse
{
    public List<ExcelComparisonValue> Results { get; set; } = new();
    public bool IsIdentical => Results.Count == 0;
}