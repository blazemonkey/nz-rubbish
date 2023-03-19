using NZRubbishCollection.Shared.Collections;
using NZRubbishCollection.Shared.Enums;
using NZRubbishCollection.Shared.Services.ScrapingService;

namespace NZRubbishCollection.Tests.Tests.IntegrationTests;

/// <summary>
/// These are tests that will make an actual URL call to the Pages
/// </summary>
public abstract class BaseIntegrationTests : UnitTests
{
    /// <summary>
    /// Gets or sets the HttpClient. This is not mocked because Integration tests will make actual HTTP calls
    /// </summary>
    protected static HttpClient HttpClient { get; set; } = new HttpClient();

    /// <summary>
    /// Get the BaseCollection class for each different council
    /// </summary>
    /// <returns>BaseCollection object</returns>
    /// <exception cref="Exception">If no collection can be found for the specified council</exception>
    protected BaseCollection GetBaseCollection()
    {
        var collection = BaseCollection.GetCouncilCollection(((int)CouncilType).ToString(), HttpClient, new ScrapingService());
        if (collection == null)
            throw new Exception("Could not find the specified Council");

        return collection;
    }
}
