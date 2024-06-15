namespace ResultSetInterpreter.Services.Interfaces;

public interface IExcelToCSharpService
{
    public Task<string> ConvertExcelToCSharpAsync(Stream excelStream);
}