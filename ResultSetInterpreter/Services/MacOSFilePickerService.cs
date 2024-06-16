using LukeMauiFilePicker;
using IFilePickerService = ResultSetInterpreter.Services.Interfaces.IFilePickerService;

namespace ResultSetInterpreter.Services;

public class MacOSFilePickerService : IFilePickerService
{
    private readonly LukeMauiFilePicker.FilePickerService _customFilePicker = new();

    public async Task<Stream?> PickFileAsync()
    {
        var fileResult = await RequestFileResult();
        
        if (fileResult == null)
        {
            return null;
        }

        return await fileResult.OpenReadAsync();
    }

    public async Task<(Stream?, string)> PickFileAndNameAsync()
    {
        var fileResult = await RequestFileResult();
        
        if (fileResult == null)
        {
            return (null, string.Empty);
        }

        return (await fileResult.OpenReadAsync(), fileResult.FileName);
    }

    private async Task<IPickFile?> RequestFileResult()
    {
        var fileResult = await _customFilePicker.PickFileAsync( 
        
            "Please select your Excel file",
            new Dictionary<DevicePlatform, IEnumerable<string>>()
            {
                { DevicePlatform.MacCatalyst, new []{ "xls", "xlsx", "XLS", "XLSX"}}
            }
        );
        return fileResult;
    }
}