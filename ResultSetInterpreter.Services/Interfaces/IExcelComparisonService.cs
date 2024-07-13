using ResultSetInterpreter.Models.DTOs.ExcelComparison;
using ResultSetIntrepreter.Services;
using ResultSetIntrepreter.Services.DTOs;

namespace ResultSetInterpreter.Services.Interfaces;

public interface IExcelComparisonService
{
    public Task<ExcelComparisionResponse> CompareExcelFilesAsync(Stream controlValue, Stream testValue);
}