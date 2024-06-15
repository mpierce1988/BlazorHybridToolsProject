using ResultSetInterpreter.Models.ExcelCSharp;

namespace ResultSetInterpreter.Services.Interfaces;

public interface IExcelCSharpWorkbookParser
{
    public Task<ExcelCSharpWorkbook> ParseExcelCSharpWorkbookAsync(Stream excelStream);
}