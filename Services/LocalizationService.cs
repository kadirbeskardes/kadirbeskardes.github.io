using Personal.Services.Interfaces;
using System.Globalization;
using System.Net.Http.Json;

namespace Personal.Services
{
    /// <summary>
    /// Çoklu dil desteği için çeviri servisi
    /// </summary>
    public class LocalizationService : ILocalizationService
    {
        private readonly HttpClient _httpClient;
        private Dictionary<string, string> _translationDictionary = new();
        private string _currentCultureCode = "tr";
        private bool _areTranslationsLoaded = false;

        public LocalizationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string CurrentCulture => _currentCultureCode;

        public async Task LoadTranslationsAsync(string cultureCode)
        {
            try
            {
                _currentCultureCode = cultureCode.StartsWith("en") ? "en" : "tr";
                var loadedTranslations = await _httpClient.GetFromJsonAsync<Dictionary<string, string>>($"data/translations.{_currentCultureCode}.json");
                _translationDictionary = loadedTranslations ?? new Dictionary<string, string>();
                _areTranslationsLoaded = true;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error loading translations: {exception.Message}");
                _translationDictionary = new Dictionary<string, string>();
            }
        }

        public string this[string translationKey]
        {
            get
            {
                if (_translationDictionary.TryGetValue(translationKey, out var translatedValue))
                {
                    return translatedValue;
                }
                return translationKey; // Return key if not found
            }
        }

        public string Get(string translationKey)
        {
            return this[translationKey];
        }

        public bool IsLoaded => _areTranslationsLoaded;
    }
}
