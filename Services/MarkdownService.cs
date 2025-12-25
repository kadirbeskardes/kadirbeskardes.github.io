using Markdig;
using Personal.Services.Interfaces;

namespace Personal.Services
{
    /// <summary>
    /// Markdown içeriklerini HTML'e dönüştürme servisi
    /// </summary>
    public class MarkdownService : IMarkdownService
    {
        /// <inheritdoc />
        public string ConvertMarkdownToHtml(string markdownContent)
        {
            if (string.IsNullOrEmpty(markdownContent))
                return string.Empty;

            var markdownPipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();

            return Markdown.ToHtml(markdownContent, markdownPipeline);
        }
    }
}
