using Microsoft.JSInterop;
using Personal.Models;
using Personal.Services.Interfaces;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;

namespace Personal.Services
{
    /// <summary>
    /// Proje verilerini yönetme servisi
    /// </summary>
    public class ProjectService : IProjectService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;
        private List<ProjectDetail>? _cachedProjects;
        private string _currentCultureCode = "tr";

        /// <inheritdoc />
        public string CurrentCulture => _currentCultureCode;

        public ProjectService(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
        }

        /// <inheritdoc />
        public async Task<List<ProjectDetail>> GetProjectListingsAsync()
        {
            await LoadProjectsIfNeededAsync();
            return _cachedProjects?.Select(project => new ProjectDetail
            {
                Id = project.Id,
                Title = project.Title,
                ShortDescription = project.ShortDescription,
                ImageUrl = project.ImageUrl,
                Technologies = project.Technologies,
                CreatedDate = project.CreatedDate,
                GitHubUrl = project.GitHubUrl,
                Category = project.Category
            }).ToList() ?? new List<ProjectDetail>();
        }

        /// <inheritdoc />
        public async Task<ProjectDetail?> GetProjectDetailAsync(int id)
        {
            await LoadProjectsIfNeededAsync();
            return _cachedProjects?.FirstOrDefault(project => project.Id == id);
        }

        private async Task LoadProjectsIfNeededAsync()
        {
            if (_cachedProjects == null)
            {
                try
                {
                    _currentCultureCode = CultureInfo.CurrentUICulture.Name.StartsWith("en") ? "en" : "tr";
                    
                    // Önce localStorage'dan kontrol et (admin panelinden yapılan değişiklikler)
                    try
                    {
                        var savedProjectsJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", AdminSettings.ProjectsStorageKey);
                        
                        if (!string.IsNullOrEmpty(savedProjectsJson))
                        {
                            _cachedProjects = JsonSerializer.Deserialize<List<ProjectDetail>>(savedProjectsJson, new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });
                            return;
                        }
                    }
                    catch
                    {
                        // localStorage erişilemezse devam et
                    }
                    
                    // localStorage boşsa JSON dosyasından yükle
                    var projectsFromFile = await _httpClient.GetFromJsonAsync<List<ProjectDetail>>($"data/projects.{_currentCultureCode}.json");
                    _cachedProjects = projectsFromFile ?? new List<ProjectDetail>();
                }
                catch
                {
                    _cachedProjects = new List<ProjectDetail>();
                }
            }
        }
    }
}
