using System.Globalization;
using Microsoft.JSInterop;

namespace Personal.Services
{
    public class CultureService
    {
        private readonly IJSRuntime _jsRuntime;

        public CultureService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public string CurrentCulture => CultureInfo.CurrentUICulture.Name;

        public async Task SetCultureAsync(string cultureName)
        {
            var culture = new CultureInfo(cultureName);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            await _jsRuntime.InvokeVoidAsync("blazorCulture.set", cultureName);
            
            // Reload the page to apply the new culture
            await _jsRuntime.InvokeVoidAsync("location.reload");
        }
    }
}