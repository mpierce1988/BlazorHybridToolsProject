namespace ResultSetIntrepreter.Services.DTOs;

public class BaseResponse
{
    public List<HandledException> Errors { get; set; } = new();
    public bool IsValid => Errors.Count == 0;

    public void AddException(HandledException exception)
    {
        Errors.Add(exception);
    }
}