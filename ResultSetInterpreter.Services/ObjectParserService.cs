using ResultSetInterpreter.Models.DTOs.InsertStatementToCSharp;
using ResultSetInterpreter.Models.ObjectDefinition;
using ResultSetIntrepreter.Services.DTOs;
using ResultSetIntrepreter.Services.Interfaces;

namespace ResultSetIntrepreter.Services;

public class ObjectParserService : IObjectParserService
{
    #region Fields
    
    private readonly IStringToObjectDefinitionParser _stringToObjectDefinitionParser;
    private readonly IObjectDefinitionPrinter _objectDefinitionPrinter;
    
    #endregion
    
    #region Constructors
    
    public ObjectParserService(IStringToObjectDefinitionParser stringToObjectDefinitionParser, IObjectDefinitionPrinter objectDefinitionPrinter)
    {
        _stringToObjectDefinitionParser = stringToObjectDefinitionParser;
        _objectDefinitionPrinter = objectDefinitionPrinter;
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>Parses an INSERT statement into C# code</summary>
    /// <param name="request">The request containing the INSERT statement to parse</param>
    /// <returns>The response containing the C# code</returns>
    public async Task<ParseInsertStatementToCSharpResponse> ParseInsertStatementToCSharpAsync(ParseInsertStatementToCSharpRequest request)
    {
        ParseInsertStatementToCSharpResponse result = new();
        
        try
        {
            ObjectDefinition definition =
                _stringToObjectDefinitionParser.ParseInsertStatement(request.InsertStatement!);

            switch (request.PrintType)
            {
                case ObjectDefinitionPrintType.ClassDefinition:
                    result.CSharpCode = await _objectDefinitionPrinter.ClassDefinitionToCodeAsync(definition);
                    break;
                case ObjectDefinitionPrintType.Data:
                    result.CSharpCode = await _objectDefinitionPrinter.ObjectsToListCodeAsync(definition);
                    break;
                case ObjectDefinitionPrintType.ClassDefinitionAndData:
                default:
                    result.CSharpCode = await _objectDefinitionPrinter.ObjectDefinitionToCSharpCode(definition);
                    break;
            }
        }
        catch (Exception e)
        {
            result.AddException(new HandledException(e.Message));
        }

        return result;
    }
    
    #endregion
}