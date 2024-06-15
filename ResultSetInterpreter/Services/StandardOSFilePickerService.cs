using ResultSetInterpreter.Services.Interfaces;

namespace ResultSetInterpreter.Services;

public class StandardOSFilePickerService : IFilePickerService
{
    public async Task<Stream?> PickFileAsync()
    {
        var fileResult = await FilePicker.PickAsync(new PickOptions()
        {
            PickerTitle = "Please select your Excel file",
            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>()
            {
                { DevicePlatform.WinUI, new []{ "xls", "xlsx", "XLS", "XLSX"}}
            })
            
        });
        
        if (fileResult == null)
        {
            return null;
        }

        return await fileResult.OpenReadAsync();
    }
}