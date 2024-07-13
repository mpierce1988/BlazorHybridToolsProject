using ResultSetInterpreter.Models.DTOs.InsertStatementToCSharp;
using ResultSetIntrepreter.Services.DTOs;

namespace ResultSetIntrepreter.Services.Interfaces;

public interface IObjectParserService
{
    public Task<ParseInsertStatementToCSharpResponse> ParseInsertStatementToCSharpAsync(ParseInsertStatementToCSharpRequest request);
}