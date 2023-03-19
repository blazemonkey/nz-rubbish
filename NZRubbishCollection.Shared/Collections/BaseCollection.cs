using HtmlAgilityPack;
using NZRubbishCollection.Shared.Enums;
using NZRubbishCollection.Shared.Models;
using NZRubbishCollection.Shared.Services.ScrapingService;
using System.Diagnostics;
using System.Reflection;

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
    /// Gets or sets the service used for web scraping
    /// </summary>
    protected IScrapingService ScrapingService { get; set; }

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
    /// <param name="scrapingService">Service used for web scraping</param>
    public BaseCollection(HttpClient httpClient, IScrapingService scrapingService)
    {
        HttpClient = httpClient;
        ScrapingService = scrapingService;
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
        return ScrapingService.GetHtmlDocument(url);
    }

    /// <summary>
    /// Get the Council instance class based on either a council enum or the full council name
    /// </summary>
    /// <param name="council">Either the Enum of the Council as a string or the full name of the Council</param>
    /// <param name="httpClient">HttpClient that will be used to grab the data from the pages</param>
    /// <param name="scrapingService">Service used for web scraping</param>
    /// <returns>A concrete instance of BaseCollection</returns>
    public static BaseCollection? GetCouncilCollection(string council, HttpClient httpClient, IScrapingService scrapingService)
    {
        var types = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsSubclassOf(typeof(BaseCollection)) && x.IsAbstract == false).ToArray();
        foreach (var t in types)
        {
            var instance = Activator.CreateInstance(t, httpClient, scrapingService) as BaseCollection;
            if (instance == null)
                continue;

            // the Environmental Variable can pass either the Enum or full string of the Council
            var isCouncilEnum = int.TryParse(council.ToString(), out int councilEnum);
            if (isCouncilEnum)
            {
                if (instance.CouncilType == (Council)councilEnum)
                    return instance;
            }
            else
            {
                if (instance.CouncilName.ToLower() == council.ToLower())
                    return instance;
            }
        }

        return null;
    }
}
