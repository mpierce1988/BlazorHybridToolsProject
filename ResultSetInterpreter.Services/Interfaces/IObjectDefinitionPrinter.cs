using ResultSetInterpreter.Models.ObjectDefinition;

namespace ResultSetIntrepreter.Services.Interfaces;

public interface IObjectDefinitionPrinter
{
    public Task<string> ClassDefinitionToCSharpCodeAsync(ObjectDefinition definition);
    public Task<string> DataToCSharpListCodeAsync(ObjectDefinition definition);
    public Task<string> ObjectDefinitionToCSharpCode(ObjectDefinition definition);
}