using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using ResultSetInterpreter.Services.EpPlus;
using ResultSetInterpreter.Services.Interfaces;
using ResultSetInterpreter.Web.Data;
using ResultSetIntrepreter.Services;
using ResultSetIntrepreter.Services.Interfaces;

namespace ResultSetInterpreter.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddSingleton<WeatherForecastService>();
        
        builder.Services.AddScoped<IStringToObjectDefinitionParser, StringToObjectDefinitionParser>();
        builder.Services.AddScoped<IObjectDefinitionPrinter, ObjectDefinitionPrinter>();
        builder.Services.AddScoped<IObjectParserService, ObjectParserService>();
        builder.Services.AddScoped<IExcelWorkbookParser, EpPlusExcelWorkbookParser>();
        builder.Services.AddScoped<IExcelComparisonService, ExcelComparisonService>();
        builder.Services.AddScoped<IBenchmarkService, BenchmarkService>();

        builder.Services.AddMudServices();
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        app.Run();
    }
}