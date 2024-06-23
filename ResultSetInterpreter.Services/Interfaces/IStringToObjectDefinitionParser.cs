using ResultSetInterpreter.Models.ObjectDefinition;

namespace ResultSetIntrepreter.Services.Interfaces;

public interface IStringToObjectDefinitionParser
{
    public Task<ObjectDefinition> ParseInsertStatementAsync(string insertStatement);
}