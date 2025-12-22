using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using Personal;
using Personal.Services;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<MarkdownService>();
builder.Services.AddScoped<CultureService>();
builder.Services.AddScoped<LocalizationService>();
builder.Services.AddScoped<AdminService>();

// Add localization services
builder.Services.AddLocalization();

var host = builder.Build();

// Set culture from localStorage before app starts
string cultureName = "tr-TR";
try
{
    var jsRuntime = host.Services.GetRequiredService<IJSRuntime>();
    cultureName = await jsRuntime.InvokeAsync<string>("blazorCulture.get") ?? "tr-TR";

    var culture = new CultureInfo(cultureName);
    CultureInfo.DefaultThreadCurrentCulture = culture;
    CultureInfo.DefaultThreadCurrentUICulture = culture;
}
catch
{
    // If localStorage is not accessible, use default Turkish culture
    var culture = new CultureInfo("tr-TR");
    CultureInfo.DefaultThreadCurrentCulture = culture;
    CultureInfo.DefaultThreadCurrentUICulture = culture;
}

// Load translations
var localizationService = host.Services.GetRequiredService<LocalizationService>();
await localizationService.LoadTranslationsAsync(cultureName);

await host.RunAsync();
