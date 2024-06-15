namespace ResultSetInterpreter.Services.Interfaces;

public interface IFilePickerService
{
    public Task<Stream?> PickFileAsync();
}