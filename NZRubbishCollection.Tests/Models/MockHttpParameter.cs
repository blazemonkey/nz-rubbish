using System.Net;

namespace NZRubbishCollection.Tests.Models;

/// <summary>
/// Used for mocking the responses that are returned by the HttpMessageHandler in HttpClient
/// </summary>
public class MockHttpParameter
{
    /// <summary>
    /// Gets or sets the request url to be matched for mocking
    /// </summary>
    public string? RequestUrl { get; set; }
    /// <summary>
    /// Gets or sets the status code that will be returned
    /// </summary>
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
    /// <summary>
    /// Gets or sets the content of the response
    /// </summary>
    public string ResponseContent { get; set; } = "{}"; // empty json
}
