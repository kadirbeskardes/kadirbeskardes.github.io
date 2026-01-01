namespace Personal.Services.Interfaces
{
    /// <summary>
    /// Çoklu dil desteği için çeviri servis interface'i
    /// </summary>
    public interface ILocalizationService
    {
        /// <summary>
        /// Mevcut kültür kodu
        /// </summary>
        string CurrentCulture { get; }

        /// <summary>
        /// Çeviriler yüklendi mi?
        /// </summary>
        bool IsLoaded { get; }

        /// <summary>
        /// Çeviri anahtarına göre değer getirir
        /// </summary>
        /// <param name="translationKey">Çeviri anahtarı</param>
        /// <returns>Çevrilmiş metin</returns>
        string this[string translationKey] { get; }

        /// <summary>
        /// Çeviri anahtarına göre değer getirir
        /// </summary>
        /// <param name="translationKey">Çeviri anahtarı</param>
        /// <returns>Çevrilmiş metin</returns>
        string Get(string translationKey);

        /// <summary>
        /// Belirtilen kültür için çevirileri yükler
        /// </summary>
        /// <param name="cultureCode">Kültür kodu (örn: tr-TR, en-US)</param>
        Task LoadTranslationsAsync(string cultureCode);
    }
}
