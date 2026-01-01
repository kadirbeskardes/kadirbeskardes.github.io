using Personal.Models;

namespace Personal.Services.Interfaces
{
    /// <summary>
    /// Admin paneli işlemleri için servis interface'i
    /// </summary>
    public interface IAdminService
    {
        #region Config Loading

        /// <summary>
        /// Config dosyasını yükler
        /// </summary>
        Task LoadConfigAsync();

        #endregion

        #region Authentication

        /// <summary>
        /// Kullanıcının oturum açıp açmadığını ve oturumun geçerli olup olmadığını kontrol eder
        /// </summary>
        Task<bool> IsAuthenticatedAsync();

        /// <summary>
        /// Giriş denemesi bilgilerini getirir
        /// </summary>
        Task<LoginAttemptInfo> GetLoginAttemptsAsync();

        /// <summary>
        /// Giriş yapar ve session token oluşturur
        /// </summary>
        /// <param name="password">Şifre</param>
        /// <returns>Başarı durumu ve mesaj</returns>
        Task<(bool Success, string Message)> LoginAsync(string password);

        /// <summary>
        /// Oturumu sonlandırır
        /// </summary>
        Task LogoutAsync();

        /// <summary>
        /// Oturum süresini uzatır
        /// </summary>
        Task ExtendSessionAsync();

        /// <summary>
        /// Kalan oturum süresini dakika cinsinden döndürür
        /// </summary>
        int GetRemainingSessionMinutes();

        #endregion

        #region Project Management

        /// <summary>
        /// Tüm projeleri getirir
        /// </summary>
        Task<List<ProjectDetail>> GetProjectsAsync();

        /// <summary>
        /// Projeleri kaydeder
        /// </summary>
        /// <param name="projects">Proje listesi</param>
        Task SaveProjectsAsync(List<ProjectDetail> projects);

        /// <summary>
        /// Yeni proje ekler
        /// </summary>
        /// <param name="project">Eklenecek proje</param>
        /// <returns>Başarı durumu</returns>
        Task<bool> AddProjectAsync(ProjectDetail project);

        /// <summary>
        /// Projeyi günceller
        /// </summary>
        /// <param name="project">Güncellenecek proje</param>
        /// <returns>Başarı durumu</returns>
        Task<bool> UpdateProjectAsync(ProjectDetail project);

        /// <summary>
        /// Projeyi siler
        /// </summary>
        /// <param name="id">Silinecek proje ID'si</param>
        /// <returns>Başarı durumu</returns>
        Task<bool> DeleteProjectAsync(int id);

        /// <summary>
        /// Belirtilen ID'ye sahip projeyi getirir
        /// </summary>
        /// <param name="id">Proje ID'si</param>
        Task<ProjectDetail?> GetProjectByIdAsync(int id);

        /// <summary>
        /// Projeleri JSON olarak dışa aktarır
        /// </summary>
        Task<string> ExportProjectsJsonAsync();

        /// <summary>
        /// Projeleri orijinal haline sıfırlar
        /// </summary>
        Task ResetToOriginalAsync();

        #endregion
    }
}
