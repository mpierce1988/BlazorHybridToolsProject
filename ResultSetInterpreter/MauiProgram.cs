using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using ResultSetInterpreter.Services;
using ResultSetInterpreter.Services.EpPlus;
using ResultSetInterpreter.Services.Interfaces;
using ResultSetIntrepreter.Services;
using ResultSetIntrepreter.Services.Interfaces;

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
        builder.Services.AddScoped<IStringToObjectDefinitionParser, StringToObjectDefinitionParser>();
        builder.Services.AddScoped<IObjectDefinitionPrinter, ObjectDefinitionPrinter>();
        builder.Services.AddScoped<IObjectParserService, ObjectParserService>();
        builder.Services.AddScoped<IExcelWorkbookParser, EpPlusExcelWorkbookParser>();
        builder.Services.AddScoped<IExcelComparisonService, ExcelComparisonService>();
        builder.Services.AddScoped<IBenchmarkService, BenchmarkService>();

        // Currently, there is a bug with Microsoft's implementation of FilePicker when running on MacOS.
        // There is a workaround implementation I found on Github, and implemented as MacOSFilePickerService
        // Use compiler directives to dynamically choose the correct FilePickerService depending on the platform
#if MACCATALYST

        builder.Services.AddScoped<IFilePickerService, MacOSFilePickerService>();
#else
        builder.Services.AddScoped<IFilePickerService, StandardOSFilePickerService>();
#endif

        builder.Services.AddMudServices();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}