using ResultSetInterpreter.Models.ObjectDefinition;

namespace ResultSetIntrepreter.Services.DTOs;

public class ParseInsertStatementToCSharpRequest
{
    public string? InsertStatement { get; set; }
    public ObjectDefinitionPrintType PrintType { get; set; } = ObjectDefinitionPrintType.ClassDefinitionAndData;
}