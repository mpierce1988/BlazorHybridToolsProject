using ResultSetInterpreter.Models.ObjectDefinition;

namespace ResultSetIntrepreter.Services.Interfaces;

public interface IObjectDefinitionPrinter
{
    public Task<string> ClassDefinitionToCodeAsync(ObjectDefinition definition);
    public Task<string> ObjectsToListCodeAsync(ObjectDefinition definition);
    public Task<string> ObjectDefinitionToCSharpCode(ObjectDefinition definition);
}