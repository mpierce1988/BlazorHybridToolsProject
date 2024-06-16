namespace ResultSetInterpreter.Services.Interfaces;

public interface IFilePickerService
{
    public Task<Stream?> PickFileAsync();
    public Task<(Stream?, string)> PickFileAndNameAsync();
}