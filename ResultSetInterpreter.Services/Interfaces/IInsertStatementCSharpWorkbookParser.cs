using ResultSetInterpreter.Models.ExcelCSharp;

namespace ResultSetInterpreter.Services.Interfaces;

public interface IInsertStatementCSharpWorkbookParser
{
    public Task<ExcelCSharpWorkbook> ParseInsertStatementCSharpWorkbookAsync(string insertStatement);
}