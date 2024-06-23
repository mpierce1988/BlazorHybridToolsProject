namespace ResultSetIntrepreter.Services.Interfaces;

public interface IObjectParserService
{
    public Task<ParseInsertStatementToCSharpResponse> ParseInsertStatementToCSharpAsync(ParseInsertStatementToCSharpRequest request);
}