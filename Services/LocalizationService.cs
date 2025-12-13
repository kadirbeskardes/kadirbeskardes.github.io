using System.Globalization;
using System.Net.Http.Json;

namespace Personal.Services
{
    public class LocalizationService
    {
        private readonly HttpClient _httpClient;
        private Dictionary<string, string> _translations = new();
        private string _currentCulture = "tr";
        private bool _isLoaded = false;

        public LocalizationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string CurrentCulture => _currentCulture;

        public async Task LoadTranslationsAsync(string culture)
        {
            try
            {
                _currentCulture = culture.StartsWith("en") ? "en" : "tr";
                var translations = await _httpClient.GetFromJsonAsync<Dictionary<string, string>>($"data/translations.{_currentCulture}.json");
                _translations = translations ?? new Dictionary<string, string>();
                _isLoaded = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading translations: {ex.Message}");
                _translations = new Dictionary<string, string>();
            }
        }

        public string this[string key]
        {
            get
            {
                if (_translations.TryGetValue(key, out var value))
                {
                    return value;
                }
                return key; // Return key if not found
            }
        }

        public string Get(string key)
        {
            return this[key];
        }

        public bool IsLoaded => _isLoaded;
    }
}
