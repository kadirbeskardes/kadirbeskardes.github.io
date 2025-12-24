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

        public async Task SetCultureAsync(string targetCultureName)
        {
            var targetCulture = new CultureInfo(targetCultureName);
            CultureInfo.DefaultThreadCurrentCulture = targetCulture;
            CultureInfo.DefaultThreadCurrentUICulture = targetCulture;

            await _jsRuntime.InvokeVoidAsync("blazorCulture.set", targetCultureName);
            
            // Reload the page to apply the new culture
            await _jsRuntime.InvokeVoidAsync("location.reload");
        }
    }
}