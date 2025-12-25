using Personal.Models;

namespace Personal.Services.Interfaces
{
    /// <summary>
    /// Proje verilerini yönetmek için servis interface'i
    /// </summary>
    public interface IProjectService
    {
        /// <summary>
        /// Mevcut kültür kodu
        /// </summary>
        string CurrentCulture { get; }

        /// <summary>
        /// Tüm projeleri listeler
        /// </summary>
        /// <returns>Proje listesi</returns>
        Task<List<ProjectDetail>> GetProjectListingsAsync();

        /// <summary>
        /// Belirtilen ID'ye sahip projenin detaylarını getirir
        /// </summary>
        /// <param name="id">Proje ID'si</param>
        /// <returns>Proje detayları veya null</returns>
        Task<ProjectDetail?> GetProjectDetailAsync(int id);
    }
}
