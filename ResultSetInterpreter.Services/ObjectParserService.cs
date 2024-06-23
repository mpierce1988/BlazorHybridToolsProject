using ResultSetInterpreter.Models.ObjectDefinition;
using ResultSetIntrepreter.Services.Interfaces;

namespace ResultSetIntrepreter.Services;

public class ObjectParserService : IObjectParserService
{
    private readonly IStringToObjectDefinitionParser _stringToObjectDefinitionParser;
    private readonly IObjectDefinitionPrinter _objectDefinitionPrinter;
    
    public ObjectParserService(IStringToObjectDefinitionParser stringToObjectDefinitionParser, IObjectDefinitionPrinter objectDefinitionPrinter)
    {
        _stringToObjectDefinitionParser = stringToObjectDefinitionParser;
        _objectDefinitionPrinter = objectDefinitionPrinter;
    }
    
    public async Task<ParseInsertStatementToCSharpResponse> ParseInsertStatementToCSharpAsync(ParseInsertStatementToCSharpRequest request)
    {
        ParseInsertStatementToCSharpResponse result = new();
        
        ObjectDefinition definition = await _stringToObjectDefinitionParser.ParseInsertStatementAsync(request.InsertStatement!);

        switch (request.PrintType)
        {
            case ObjectDefinitionPrintType.ClassDefinition:
                result.CSharpCode = await _objectDefinitionPrinter.ClassDefinitionToCSharpCodeAsync(definition);
                break;
            case ObjectDefinitionPrintType.Data:
                result.CSharpCode = await _objectDefinitionPrinter.DataToCSharpListCodeAsync(definition);
                break;
            case ObjectDefinitionPrintType.ClassDefinitionAndData:
            default:
                result.CSharpCode = await _objectDefinitionPrinter.ObjectDefinitionToCSharpCode(definition);
                break;
        }

        return result;
    }
}