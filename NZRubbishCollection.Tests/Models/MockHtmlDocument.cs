namespace NZRubbishCollection.Tests.Models;

/// <summary>
/// Used for mocking a HTML response from loading a webpage
/// </summary>
public class MockHtmlDocument
{
    /// <summary>
    /// Gets or sets the request url to be matched for mocking
    /// </summary>
    public string? RequestUrl { get; set; }

    /// <summary>
    /// Gets or sets the HTML of the webpage to return
    /// </summary>
    public string? HtmlString { get; set; }
}
