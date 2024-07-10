using System.ComponentModel.DataAnnotations;

namespace ResultSetIntrepreter.Services.DTOs;

public class HandledException
{
    public string? Message { get; set; }
    public List<ValidationResult>? ValidationResults { get; set; }
    public bool IsValidationResult => ValidationResults != null && ValidationResults.Count > 0;
    
    public HandledException()
    {
        
    }
    
    public HandledException(string message)
    {
        Message = message;
    }

    public HandledException(List<ValidationResult> validationResults)
    {
        Message = "Validation Failed";
        ValidationResults = validationResults;
    }
}