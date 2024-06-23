using ResultSetInterpreter.Models.Workbook;

namespace ResultSetIntrepreter.Services;

public interface IExcelWorkbookParser
{
    public Task<Workbook> ParseExcel(Stream stream);
}