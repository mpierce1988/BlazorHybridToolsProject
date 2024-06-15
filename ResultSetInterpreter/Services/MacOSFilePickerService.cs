using ResultSetInterpreter.Services.Interfaces;

namespace ResultSetInterpreter.Services;

public class MacOSFilePickerService : IFilePickerService
{
    private readonly LukeMauiFilePicker.FilePickerService _customFilePicker = new();

    public async Task<Stream?> PickFileAsync()
    {
        var fileResult = await _customFilePicker.PickFileAsync( 
        
            "Please select your Excel file",
            new Dictionary<DevicePlatform, IEnumerable<string>>()
            {
                { DevicePlatform.MacCatalyst, new []{ "xls", "xlsx", "XLS", "XLSX"}}
            }
        );
        
        if (fileResult == null)
        {
            return null;
        }

        return await fileResult.OpenReadAsync();
    }
}