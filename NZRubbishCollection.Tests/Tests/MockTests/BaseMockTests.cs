using HtmlAgilityPack;
using Moq;
using Moq.Protected;
using NZRubbishCollection.Shared.Collections;
using NZRubbishCollection.Shared.Enums;
using NZRubbishCollection.Shared.Services.ScrapingService;
using NZRubbishCollection.Tests.Models;

namespace NZRubbishCollection.Tests.Tests.MockTests;

/// <summary>
/// These tests will mock any HTTP calls and use the sample HTML pages saved
/// </summary>
public abstract class BaseMockTests : UnitTests
{
    /// <summary>
    /// Get the mocked IHttpClientFactory to create the HttpClient
    /// </summary>
    /// <param name="parameters">Parameters to mock the HTTP calls</param>
    /// <returns>Mocked IHttpClientFactory</returns>
    protected Mock<IHttpClientFactory> GetMockHttpClientFactory(MockHttpParameter[] parameters)
    {
        var mockHttpClientFactory = new Mock<IHttpClientFactory>();
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        foreach (var mhp in parameters ?? new MockHttpParameter[] { })
        {
            var result = Task.FromResult(new HttpResponseMessage()
            {
                StatusCode = mhp.StatusCode,
                Content = new StringContent(mhp.ResponseContent)
            });


            var protectedMock = handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x => x.RequestUri.OriginalString.StartsWith(mhp.RequestUrl)), ItExpr.IsAny<CancellationToken>());
            protectedMock.Returns(() => result).Verifiable();
        }

        var httpClient = new HttpClient(handlerMock.Object);
        mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

        return mockHttpClientFactory;
    }

    /// <summary>
    /// Get the BaseCollection class for each different council
    /// </summary>
    /// <param name="httpParameters">An array of mocked parameters for HttpClient</param>
    /// <param name="htmlDocuments">An array of mocked HtmlDocuments</param>
    /// <returns>A concrete implementation of BaseCollection</returns>
    /// <summary />
    protected BaseCollection GetBaseCollection(MockHttpParameter[] httpParameters, MockHtmlDocument[] htmlDocuments)
    {
        var httpClient = GetMockHttpClientFactory(httpParameters).Object.CreateClient();

        var scrapingService = new Mock<IScrapingService>();
        foreach (var hd in htmlDocuments)
        {
            var document = new HtmlDocument();
            document.LoadHtml(hd.HtmlString);

            scrapingService.Setup(x => x.GetHtmlDocument(It.Is<string>(x => x == hd.RequestUrl))).Returns(document);
        }

        var collection = BaseCollection.GetCouncilCollection(((int)CouncilType).ToString(), httpClient, scrapingService.Object);
        if (collection == null)
            throw new Exception("Could not find the specified Council");

        return collection;
    }
}
