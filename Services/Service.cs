using Markdig;
using Microsoft.JSInterop;
using Personal.Models;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Personal.Services
{
    public class MarkdownService
    {
        public string ConvertMarkdownToHtml(string markdownContent)
        {
            if (string.IsNullOrEmpty(markdownContent))
                return string.Empty;

            var markdownPipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();

            return Markdig.Markdown.ToHtml(markdownContent, markdownPipeline);
        }
    }
    public class ProjectService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;
        private List<ProjectDetail>? _cachedProjects;
        private string _currentCultureCode = "tr";
        private bool _isProjectDataLoaded = false;
        public string CurrentCulture => _currentCultureCode;

        public ProjectService(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
        }

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
                            _isProjectDataLoaded = true;
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
                    _isProjectDataLoaded = true;
                }
                catch
                {
                    _cachedProjects = new List<ProjectDetail>();
                }
            }
        }
    }
}