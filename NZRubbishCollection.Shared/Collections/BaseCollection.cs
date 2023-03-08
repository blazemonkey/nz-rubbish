using HtmlAgilityPack;
using NZRubbishCollection.Shared.Enums;
using NZRubbishCollection.Shared.Models;
using System.Diagnostics;

namespace NZRubbishCollection.Shared.Collections;

/// <summary>
/// Base collection class containing abstract methods and properties the inherited collection classes will use
/// </summary>
public abstract class BaseCollection
{
    /// <summary>
    /// Gets or sets the HttpClient that will be used to grab the data from the pages
    /// </summary>
    protected HttpClient HttpClient { get; set; }

    /// <summary>
    /// Gets the name of the Council
    /// </summary>
    public abstract string CouncilName { get; }

    /// <summary>
    /// Gets the enum of the Council
    /// </summary>
    public abstract Council CouncilType { get; }

    /// <summary>
    /// Gets the Base URL of the Council website
    /// </summary>
    protected abstract string BaseUrl { get; }

    /// <summary>
    /// Constructor of the class
    /// </summary>
    /// <param name="httpClient">HttpClient that will be used to grab the data from the pages</param>
    public BaseCollection(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }

    /// <summary>
    /// Get the collection details for the street address and council region. Wraps in a try-catch for timing and error handling purposes
    /// </summary>
    /// <param name="streetAddress">Street address to search</param>
    /// <returns>A CollectionResponse with the details</returns>
    public async Task<CollectionResponse> GetCollection(string streetAddress)
    {
        var response = new CollectionResponse();
        var sw = new Stopwatch();
        sw.Start();
        try
        {
            response = await DoGetCollection(streetAddress);
        }
        catch (Exception)
        {
            response = new CollectionResponse() { Error = "An unexpected error occured while retrieving" };
        }
        finally
        {
            sw.Stop();
            response.TimeTaken = sw.Elapsed;            
        }

        return response;
    }

    /// <summary>
    /// Get the collection details for the street address and council region
    /// </summary>
    /// <param name="streetAddress">Street address to search</param>
    /// <returns>A CollectionResponse with the details</returns>
    protected abstract Task<CollectionResponse> DoGetCollection(string streetAddress);

    /// <summary>
    /// Get the HtmlDocument
    /// </summary>
    /// <param name="url">Url to get</param>
    /// <returns>An HtmlDocument</returns>
    protected HtmlDocument GetDocument(string url)
    {
        var web = new HtmlWeb();
        var doc = web.Load(url);

        return doc;
    }
}
