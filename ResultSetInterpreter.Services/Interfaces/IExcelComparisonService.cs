using ResultSetIntrepreter.Services;

namespace ResultSetInterpreter.Services.Interfaces;

public interface IExcelComparisonService
{
    public Task<ExcelComparisionResult> CompareExcelFilesAsync(Stream controlValue, Stream testValue);
}