using ResultSetInterpreter.Models.ObjectDefinition;

namespace ResultSetInterpreter.Models.DTOs.InsertStatementToCSharp;

public class ParseInsertStatementToCSharpRequest
{
    public string? InsertStatement { get; set; }
    public ObjectDefinitionPrintType PrintType { get; set; } = ObjectDefinitionPrintType.ClassDefinitionAndData;
}