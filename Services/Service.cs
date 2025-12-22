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
        public string ToHtml(string markdown)
        {
            if (string.IsNullOrEmpty(markdown))
                return string.Empty;

            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();

            return Markdig.Markdown.ToHtml(markdown, pipeline);
        }
    }
    public class ProjectService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _jsRuntime;
        private List<ProjectDetail>? _projects;
        private string _currentCulture = "tr";
        private bool _isLoaded = false;
        public string CurrentCulture => _currentCulture;

        public ProjectService(HttpClient http, IJSRuntime jsRuntime)
        {
            _http = http;
            _jsRuntime = jsRuntime;
        }

        public async Task<List<ProjectDetail>> GetProjectListingsAsync()
        {
            await LoadProjectsIfNeededAsync();
            return _projects?.Select(p => new ProjectDetail
            {
                Id = p.Id,
                Title = p.Title,
                ShortDescription = p.ShortDescription,
                ImageUrl = p.ImageUrl,
                Technologies = p.Technologies,
                CreatedDate = p.CreatedDate,
                GitHubUrl = p.GitHubUrl,
                Category = p.Category
            }).ToList() ?? new List<ProjectDetail>();
        }

        public async Task<ProjectDetail?> GetProjectDetailAsync(int id)
        {
            await LoadProjectsIfNeededAsync();
            return _projects?.FirstOrDefault(p => p.Id == id);
        }

        private async Task LoadProjectsIfNeededAsync()
        {
            if (_projects == null)
            {
                try
                {
                    _currentCulture = CultureInfo.CurrentUICulture.Name.StartsWith("en") ? "en" : "tr";
                    
                    // Önce localStorage'dan kontrol et (admin panelinden yapılan değişiklikler)
                    try
                    {
                        var savedJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", AdminSettings.ProjectsStorageKey);
                        
                        if (!string.IsNullOrEmpty(savedJson))
                        {
                            _projects = JsonSerializer.Deserialize<List<ProjectDetail>>(savedJson, new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });
                            _isLoaded = true;
                            return;
                        }
                    }
                    catch
                    {
                        // localStorage erişilemezse devam et
                    }
                    
                    // localStorage boşsa JSON dosyasından yükle
                    var projects = await _http.GetFromJsonAsync<List<ProjectDetail>>($"data/projects.{_currentCulture}.json");
                    _projects = projects ?? new List<ProjectDetail>();
                    _isLoaded = true;
                }
                catch
                {
                    _projects = new List<ProjectDetail>();
                }
            }
        }
    }
}