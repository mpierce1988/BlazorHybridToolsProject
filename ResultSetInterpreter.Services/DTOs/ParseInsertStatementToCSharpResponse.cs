namespace ResultSetIntrepreter.Services.DTOs;

public class ParseInsertStatementToCSharpResponse : BaseResponse
{
    public string? CSharpCode { get; set; }
}