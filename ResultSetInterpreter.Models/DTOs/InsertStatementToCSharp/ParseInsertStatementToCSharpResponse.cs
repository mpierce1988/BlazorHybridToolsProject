using ResultSetIntrepreter.Services.DTOs;

namespace ResultSetInterpreter.Models.DTOs.InsertStatementToCSharp;

public class ParseInsertStatementToCSharpResponse : BaseResponse
{
    public string? CSharpCode { get; set; }
}