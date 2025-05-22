using Markdig;
using Personal.Models;
using System.Net.Http.Json;

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
        private List<ProjectDetail>? _projects;

        public ProjectService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<ProjectListing>> GetProjectListingsAsync()
        {
            await LoadProjectsIfNeededAsync();
            return _projects?.Select(p => new ProjectListing
            {
                Id = p.Id,
                Title = p.Title,
                ShortDescription = p.ShortDescription,
                ImageUrl = p.ImageUrl,
                Technologies = p.Technologies,
                CreatedDate = p.CreatedDate
            }).ToList() ?? new List<ProjectListing>();
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
                    _projects = await _http.GetFromJsonAsync<List<ProjectDetail>>("data/projects.json");
                }
                catch
                {
                    _projects = new List<ProjectDetail>();
                }
            }
        }
    }
}
