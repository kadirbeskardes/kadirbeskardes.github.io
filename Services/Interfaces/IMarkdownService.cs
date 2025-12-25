namespace Personal.Services.Interfaces
{
    /// <summary>
    /// Markdown içeriklerini HTML'e dönüştürmek için servis interface'i
    /// </summary>
    public interface IMarkdownService
    {
        /// <summary>
        /// Markdown içeriğini HTML'e dönüştürür
        /// </summary>
        /// <param name="markdownContent">Dönüştürülecek markdown içeriği</param>
        /// <returns>HTML formatında içerik</returns>
        string ConvertMarkdownToHtml(string markdownContent);
    }
}
