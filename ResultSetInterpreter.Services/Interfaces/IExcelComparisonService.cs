using ResultSetInterpreter.Models.DTOs.ExcelComparison;

namespace ResultSetIntrepreter.Services.Interfaces;

public interface IExcelComparisonService
{
    public Task<ExcelComparisonResponse> CompareExcelFilesAsync(ExcelComparisonRequest request);
}