namespace Personal.Services.Interfaces
{
    /// <summary>
    /// Kültür değişikliği için servis interface'i
    /// </summary>
    public interface ICultureService
    {
        /// <summary>
        /// Mevcut kültür adı
        /// </summary>
        string CurrentCulture { get; }

        /// <summary>
        /// Kültürü değiştirir ve sayfayı yeniler
        /// </summary>
        /// <param name="targetCultureName">Hedef kültür adı (örn: tr-TR, en-US)</param>
        Task SetCultureAsync(string targetCultureName);
    }
}
