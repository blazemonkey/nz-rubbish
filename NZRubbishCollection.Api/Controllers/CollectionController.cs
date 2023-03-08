using Microsoft.AspNetCore.Mvc;
using NZRubbishCollection.Shared.Collections;
using NZRubbishCollection.Shared.Enums;
using NZRubbishCollection.Shared.Models;

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
    /// Gets or sets the Configurations used
    /// </summary>
    public IConfiguration Configuration { get; init; }

    public CollectionController(HttpClient httpClient, IConfiguration configuration)
    {
        HttpClient = httpClient;
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

        var instance = GetCouncilCollection(council, HttpClient);
        if (instance == null)
            return new CollectionResponse() { Error = "Incorrect Council name or enum used." };

        var response = await instance.GetCollection(streetAddress);
        return response;
    }

    /// <summary>
    /// Get the Council instance class based on either a council enum or the full council name
    /// </summary>
    /// <param name="council">Either the Enum of the Council as a string or the full name of the Council</param>
    /// <param name="httpClient">HttpClient that will be used to grab the data from the pages</param>
    /// <returns>A concrete instance of BaseCollection</returns>
    private static BaseCollection? GetCouncilCollection(string council, HttpClient httpClient)
    {
        var types = typeof(BaseCollection).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(BaseCollection)) && x.IsAbstract == false).ToArray();
        foreach (var t in types)
        {
            var instance = Activator.CreateInstance(t, httpClient) as BaseCollection;
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