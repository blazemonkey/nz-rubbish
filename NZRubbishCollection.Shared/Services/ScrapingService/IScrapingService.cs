using HtmlAgilityPack;

namespace NZRubbishCollection.Shared.Services.ScrapingService;

/// <summary>
/// Interface for ScrapingService
/// </summary>
public interface IScrapingService
{
    /// <summary>
    /// Get the HtmlDocument to parse
    /// </summary>
    /// <param name="url">Url to get</param>
    /// <returns>An HtmlDocument</returns>
    HtmlDocument GetHtmlDocument(string url);
}
