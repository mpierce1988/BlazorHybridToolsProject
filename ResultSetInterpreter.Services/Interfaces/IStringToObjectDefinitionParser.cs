using ResultSetInterpreter.Models.ObjectDefinition;

namespace ResultSetIntrepreter.Services.Interfaces;

public interface IStringToObjectDefinitionParser
{
    public ObjectDefinition ParseInsertStatement(string insertStatement);
}