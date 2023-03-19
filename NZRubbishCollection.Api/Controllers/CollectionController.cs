using Microsoft.AspNetCore.Mvc;
using NZRubbishCollection.Shared.Collections;
using NZRubbishCollection.Shared.Models;
using NZRubbishCollection.Shared.Services.ScrapingService;

namespace NZRubbishCollection.Api.Controllers;

/// <summary>
/// Controller for getting Collection dates
/// </summary>
[ApiController]
[Route("[controller]")]
public class CollectionController : ControllerBase
{
    /// <summary>
    /// Gets or sets the HttpClient that will be used to grab the data from the pages
    /// </summary>
    public HttpClient HttpClient { get; init; }

    /// <summary>
    /// Gets or sets the service used for web scraping
    /// </summary>
    protected IScrapingService ScrapingService { get; set; }

    /// <summary>
    /// Gets or sets the Configurations used
    /// </summary>
    public IConfiguration Configuration { get; init; }

    /// <summary>
    /// Constructor of the Controller
    /// </summary>
    /// <param name="httpClient">HttpClient that will be used to grab the data from the pages</param>
    /// <param name="scrapingService">Service used for web scraping</param>
    /// <param name="configuration">Used for getting configurations set</param>
    public CollectionController(HttpClient httpClient, IScrapingService scrapingService, IConfiguration configuration)
    {
        HttpClient = httpClient;
        ScrapingService = scrapingService;
        Configuration = configuration;
    }

    /// <summary>
    /// Default endpoint. Checks if any default addresses are set in configuration, if available then use, if not return an error
    /// </summary>
    /// <returns>CollectionResponse with collection details</returns>
    [HttpGet]
    public async Task<CollectionResponse> Get()
    {
        var council = Configuration["Council"];
        var streetAddress = Configuration["StreetAddress"];
        if (string.IsNullOrEmpty(council) || string.IsNullOrEmpty(streetAddress))
            return new CollectionResponse() { Error = "Both 'Council' and 'StreetAddress' must be set in Environmental Variables to use this endpoint." };

        var instance = BaseCollection.GetCouncilCollection(council, HttpClient, ScrapingService);
        if (instance == null)
            return new CollectionResponse() { Error = "Incorrect Council name or enum used." };

        var response = await instance.GetCollection(streetAddress);
        return response;
    }
}