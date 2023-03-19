using HtmlAgilityPack;

namespace NZRubbishCollection.Shared.Services.ScrapingService;

/// <summary>
/// Service for web scraping
/// </summary>
public class ScrapingService : IScrapingService
{
    /// <summary>
    /// Get the HtmlDocument to parse
    /// </summary>
    /// <param name="url">Url to get</param>
    /// <returns>An HtmlDocument</returns>
    public HtmlDocument GetHtmlDocument(string url)
    {
        var web = new HtmlWeb();
        var doc = web.Load(url);
        return doc;
    }
}
