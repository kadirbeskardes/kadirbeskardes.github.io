using Microsoft.JSInterop;
using Personal.Models;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;

namespace Personal.Services
{
    public class AdminService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly HttpClient _http;
        private List<ProjectDetail>? _adminProjects;
        private AdminSession? _currentSession;
        private bool _configLoaded = false;

        public AdminService(IJSRuntime jsRuntime, HttpClient http)
        {
            _jsRuntime = jsRuntime;
            _http = http;
        }

        #region Config Loading

        /// <summary>
        /// Config dosyasını yükler
        /// </summary>
        public async Task LoadConfigAsync()
        {
            if (_configLoaded) return;

            try
            {
                var config = await _http.GetFromJsonAsync<AdminConfig>("data/admin.config.json");
                if (config != null)
                {
                    AdminSettings.LoadConfig(config);
                    _configLoaded = true;
                }
            }
            catch
            {
                // Config dosyası bulunamazsa varsayılan değerler kullanılır
                var defaultConfig = new AdminConfig();
                AdminSettings.LoadConfig(defaultConfig);
                _configLoaded = true;
            }
        }

        #endregion

        #region Authentication

        /// <summary>
        /// Kullanıcının oturum açıp açmadığını ve oturumun geçerli olup olmadığını kontrol eder
        /// </summary>
        public async Task<bool> IsAuthenticatedAsync()
        {
            try
            {
                await LoadConfigAsync();

                var sessionJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", AdminSettings.AdminStorageKey);
                
                if (string.IsNullOrEmpty(sessionJson))
                    return false;

                var session = JsonSerializer.Deserialize<AdminSession>(sessionJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (session == null || !session.IsValid)
                {
                    await LogoutAsync();
                    return false;
                }

                _currentSession = session;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Giriş denemesi bilgilerini getirir
        /// </summary>
        public async Task<LoginAttemptInfo> GetLoginAttemptsAsync()
        {
            try
            {
                var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", AdminSettings.LoginAttemptsKey);
                
                if (!string.IsNullOrEmpty(json))
                {
                    var info = JsonSerializer.Deserialize<LoginAttemptInfo>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    if (info != null)
                    {
                        // Kilitleme süresi dolduysa sıfırla
                        if (info.LockoutUntil.HasValue && DateTime.UtcNow >= info.LockoutUntil.Value)
                        {
                            info = new LoginAttemptInfo();
                            await SaveLoginAttemptsAsync(info);
                        }
                        return info;
                    }
                }
            }
            catch { }

            return new LoginAttemptInfo();
        }

        /// <summary>
        /// Giriş denemesi bilgilerini kaydeder
        /// </summary>
        private async Task SaveLoginAttemptsAsync(LoginAttemptInfo info)
        {
            var json = JsonSerializer.Serialize(info);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", AdminSettings.LoginAttemptsKey, json);
        }

        /// <summary>
        /// Giriş yapar ve session token oluşturur
        /// </summary>
        public async Task<(bool Success, string Message)> LoginAsync(string password)
        {
            // Config'i yükle
            await LoadConfigAsync();

            // Kilitleme kontrolü
            var attempts = await GetLoginAttemptsAsync();
            
            if (attempts.IsLockedOut)
            {
                var remainingMinutes = (int)Math.Ceiling((attempts.LockoutUntil!.Value - DateTime.UtcNow).TotalMinutes);
                return (false, $"Çok fazla başarısız deneme. {remainingMinutes} dakika sonra tekrar deneyin.");
            }

            // Şifre doğrulama
            if (AdminSettings.VerifyPassword(password))
            {
                // Başarılı giriş - session oluştur
                var session = new AdminSession
                {
                    Token = AdminSettings.GenerateSessionToken(),
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(AdminSettings.SessionTimeoutMinutes)
                };

                var sessionJson = JsonSerializer.Serialize(session);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", AdminSettings.AdminStorageKey, sessionJson);
                
                // Giriş denemelerini sıfırla
                await SaveLoginAttemptsAsync(new LoginAttemptInfo());
                
                _currentSession = session;
                return (true, "Giriş başarılı.");
            }
            else
            {
                // Başarısız giriş - deneme sayısını artır
                attempts.AttemptCount++;
                attempts.LastAttempt = DateTime.UtcNow;

                if (attempts.AttemptCount >= AdminSettings.MaxLoginAttempts)
                {
                    attempts.LockoutUntil = DateTime.UtcNow.AddMinutes(AdminSettings.LockoutMinutes);
                    await SaveLoginAttemptsAsync(attempts);
                    return (false, $"Çok fazla başarısız deneme. {AdminSettings.LockoutMinutes} dakika boyunca giriş yapılamaz.");
                }

                await SaveLoginAttemptsAsync(attempts);
                return (false, $"Hatalı şifre. Kalan deneme: {attempts.RemainingAttempts}");
            }
        }

        /// <summary>
        /// Oturumu sonlandırır
        /// </summary>
        public async Task LogoutAsync()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", AdminSettings.AdminStorageKey);
            _currentSession = null;
        }

        /// <summary>
        /// Oturum süresini uzatır
        /// </summary>
        public async Task ExtendSessionAsync()
        {
            if (_currentSession != null && _currentSession.IsValid)
            {
                _currentSession.ExpiresAt = DateTime.UtcNow.AddMinutes(AdminSettings.SessionTimeoutMinutes);
                var sessionJson = JsonSerializer.Serialize(_currentSession);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", AdminSettings.AdminStorageKey, sessionJson);
            }
        }

        /// <summary>
        /// Kalan oturum süresini dakika cinsinden döndürür
        /// </summary>
        public int GetRemainingSessionMinutes()
        {
            if (_currentSession == null || !_currentSession.IsValid)
                return 0;

            return (int)Math.Max(0, (_currentSession.ExpiresAt - DateTime.UtcNow).TotalMinutes);
        }

        #endregion

        #region Project Management

        public async Task<List<ProjectDetail>> GetProjectsAsync()
        {
            try
            {
                // Önce localStorage'dan kontrol et
                var savedJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", AdminSettings.ProjectsStorageKey);
                
                if (!string.IsNullOrEmpty(savedJson))
                {
                    _adminProjects = JsonSerializer.Deserialize<List<ProjectDetail>>(savedJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else
                {
                    // localStorage boşsa, JSON dosyasından yükle
                    var currentCulture = CultureInfo.CurrentUICulture.Name.StartsWith("en") ? "en" : "tr";
                    _adminProjects = await _http.GetFromJsonAsync<List<ProjectDetail>>($"data/projects.{currentCulture}.json");
                    
                    // localStorage'a kaydet
                    if (_adminProjects != null)
                    {
                        await SaveProjectsAsync(_adminProjects);
                    }
                }

                return _adminProjects ?? new List<ProjectDetail>();
            }
            catch
            {
                return new List<ProjectDetail>();
            }
        }

        public async Task SaveProjectsAsync(List<ProjectDetail> projects)
        {
            _adminProjects = projects;
            var json = JsonSerializer.Serialize(projects, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", AdminSettings.ProjectsStorageKey, json);
        }

        public async Task<bool> AddProjectAsync(ProjectDetail project)
        {
            var projects = await GetProjectsAsync();
            
            // Yeni ID ata
            project.Id = projects.Any() ? projects.Max(p => p.Id) + 1 : 1;
            project.CreatedDate = DateTime.Now;
            
            projects.Add(project);
            await SaveProjectsAsync(projects);
            return true;
        }

        public async Task<bool> UpdateProjectAsync(ProjectDetail project)
        {
            var projects = await GetProjectsAsync();
            var index = projects.FindIndex(p => p.Id == project.Id);
            
            if (index >= 0)
            {
                projects[index] = project;
                await SaveProjectsAsync(projects);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteProjectAsync(int id)
        {
            var projects = await GetProjectsAsync();
            var project = projects.FirstOrDefault(p => p.Id == id);
            
            if (project != null)
            {
                projects.Remove(project);
                await SaveProjectsAsync(projects);
                return true;
            }
            return false;
        }

        public async Task<ProjectDetail?> GetProjectByIdAsync(int id)
        {
            var projects = await GetProjectsAsync();
            return projects.FirstOrDefault(p => p.Id == id);
        }

        public async Task<string> ExportProjectsJsonAsync()
        {
            var projects = await GetProjectsAsync();
            return JsonSerializer.Serialize(projects, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }

        public async Task ResetToOriginalAsync()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", AdminSettings.ProjectsStorageKey);
            _adminProjects = null;
        }

        #endregion
    }
}
