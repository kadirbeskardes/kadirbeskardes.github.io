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

        public async Task<string> GetCultureAsync()
        {
            try
            {
                // First check URL parameter
                var currentUrl = await _jsRuntime.InvokeAsync<string>("eval", "window.location.search");
                if (currentUrl.Contains("culture=en-US"))
                {
                    return "en-US";
                }
                if (currentUrl.Contains("culture=tr-TR"))
                {
                    return "tr-TR";
                }
                
                // Then check localStorage
                var culture = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "BlazorCulture");
                return !string.IsNullOrEmpty(culture) ? culture : "tr-TR";
            }
            catch
            {
                return "tr-TR";
            }
        }

        public async Task SetCultureAsync(string culture)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "BlazorCulture", culture);
                
                // Redirect to same page with culture parameter
                var currentPath = await _jsRuntime.InvokeAsync<string>("eval", "window.location.pathname");
                var newUrl = $"{currentPath}?culture={culture}";
                
                Console.WriteLine($"Redirecting to: {newUrl}");
                
                await _jsRuntime.InvokeVoidAsync("eval", $"window.location.href = '{newUrl}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting culture: {ex.Message}");
                // Fallback reload
                await _jsRuntime.InvokeVoidAsync("eval", "window.location.reload(true)");
            }
        }
    }
}