using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using ResultSetInterpreter.Services;
using ResultSetInterpreter.Services.EpPlus;
using ResultSetInterpreter.Services.Interfaces;
using ResultSetIntrepreter.Services;

namespace ResultSetInterpreter;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddScoped<IExcelWorkbookParser, EpPlusExcelWorkbookParser>();
        builder.Services.AddScoped<IExcelComparisonService, ExcelComparisonService>();
        builder.Services.AddScoped<IBenchmarkService, BenchmarkService>();
        builder.Services.AddScoped<IFilePickerService, MacOSFilePickerService>();
        builder.Services.AddScoped<IExcelCSharpWorkbookParser, EpPlusExcelCSharpWorkbookParser>();
        builder.Services.AddScoped<IExcelToCSharpService, ExcelCSharpService>();

        builder.Services.AddMudServices();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}