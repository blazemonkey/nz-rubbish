using NZRubbishCollection.Shared.Enums;
using NZRubbishCollection.Tests.Models;

namespace NZRubbishCollection.Tests.Tests.MockTests;

/// <summary>
/// Mocked tests for Auckland
/// </summary>
[TestClass]
public class AucklandTests : BaseMockTests
{
    /// <summary>
    /// Council for Tests
    /// </summary>
    public override Council CouncilType => Council.Auckland;

    [TestMethod]
    public async Task HasAllTest()
    {
        var httpParameter = new MockHttpParameter() { RequestUrl = "https://www.aucklandcouncil.govt.nz/_vti_bin/ACWeb/ACservices.svc/GetMatchingPropertyAddresses", ResponseContent = "[{\"ACRateAccountKey\":\"12343873601\",\"Address\":\"10 Popokatea Drive, Takanini\",\"Suggestion\":\"10 Popokatea Drive, Takanini\"}]" };
        var htmlDocumentParameter = new MockHtmlDocument() { RequestUrl = $"https://www.aucklandcouncil.govt.nz/rubbish-recycling/rubbish-recycling-collections/Pages/collection-day-detail.aspx?an=12343873601", HtmlString = EmbeddedResourceHelper.GetResourceFromHtml($"{CouncilType}.{nameof(HasAllTest)}") };

        var collection = GetBaseCollection(new MockHttpParameter[] { httpParameter }, new MockHtmlDocument[] { htmlDocumentParameter });
        var result = await collection.GetCollection("10 Popokatea Drive");

        Assert.IsNotNull(result);
        Assert.IsNull(result.Error);
        Assert.AreEqual("https://www.aucklandcouncil.govt.nz/rubbish-recycling/rubbish-recycling-collections/Pages/collection-day-detail.aspx?an=12343873601", result.SourceUrl);
        Assert.AreEqual("10 Popokatea Drive, Takanini", result.StreetAddress);
        Assert.AreEqual(5, result.Details?.Length);
        Assert.AreEqual(2, result.Details?.Count(x => x.Type == CollectionType.Rubbish));
        Assert.AreEqual(2, result.Details?.Count(x => x.Type == CollectionType.FoodScraps));
        Assert.AreEqual(1, result.Details?.Count(x => x.Type == CollectionType.Recycling));
    }

    [TestMethod]
    public async Task HasRecyclingTest()
    {
        var httpParameter = new MockHttpParameter() { RequestUrl = "https://www.aucklandcouncil.govt.nz/_vti_bin/ACWeb/ACservices.svc/GetMatchingPropertyAddresses", ResponseContent = "[{\"ACRateAccountKey\":\"12345874265\",\"Address\":\"2 Surf View Crescent, Red Beach\",\"Suggestion\":\"2 Surf View Crescent, Red Beach\"}]" };
        var htmlDocumentParameter = new MockHtmlDocument() { RequestUrl = $"https://www.aucklandcouncil.govt.nz/rubbish-recycling/rubbish-recycling-collections/Pages/collection-day-detail.aspx?an=12345874265", HtmlString = EmbeddedResourceHelper.GetResourceFromHtml($"{CouncilType}.{nameof(HasRecyclingTest)}") };

        var collection = GetBaseCollection(new MockHttpParameter[] { httpParameter }, new MockHtmlDocument[] { htmlDocumentParameter });
        var result = await collection.GetCollection("2 Surf View Crescent");

        Assert.IsNotNull(result);
        Assert.IsNull(result.Error);
        Assert.AreEqual("https://www.aucklandcouncil.govt.nz/rubbish-recycling/rubbish-recycling-collections/Pages/collection-day-detail.aspx?an=12345874265", result.SourceUrl);
        Assert.AreEqual("2 Surf View Crescent, Red Beach", result.StreetAddress);
        Assert.AreEqual(1, result.Details?.Length);
        Assert.AreEqual(0, result.Details?.Count(x => x.Type == CollectionType.Rubbish));
        Assert.AreEqual(0, result.Details?.Count(x => x.Type == CollectionType.FoodScraps));
        Assert.AreEqual(1, result.Details?.Count(x => x.Type == CollectionType.Recycling));
    }

    [TestMethod]
    public async Task HasRubbishAndRecyclingTest()
    {
        var httpParameter = new MockHttpParameter() { RequestUrl = "https://www.aucklandcouncil.govt.nz/_vti_bin/ACWeb/ACservices.svc/GetMatchingPropertyAddresses", ResponseContent = "[{\"ACRateAccountKey\":\"12342423734\",\"Address\":\"2 West End Road, Herne Bay\",\"Suggestion\":\"2 West End Road, Herne Bay\"}]" };
        var htmlDocumentParameter = new MockHtmlDocument() { RequestUrl = $"https://www.aucklandcouncil.govt.nz/rubbish-recycling/rubbish-recycling-collections/Pages/collection-day-detail.aspx?an=12342423734", HtmlString = EmbeddedResourceHelper.GetResourceFromHtml($"{CouncilType}.{nameof(HasRubbishAndRecyclingTest)}") };

        var collection = GetBaseCollection(new MockHttpParameter[] { httpParameter }, new MockHtmlDocument[] { htmlDocumentParameter });
        var result = await collection.GetCollection("2 West End Road");

        Assert.IsNotNull(result);
        Assert.IsNull(result.Error);
        Assert.AreEqual("https://www.aucklandcouncil.govt.nz/rubbish-recycling/rubbish-recycling-collections/Pages/collection-day-detail.aspx?an=12342423734", result.SourceUrl);
        Assert.AreEqual("2 West End Road, Herne Bay", result.StreetAddress);
        Assert.AreEqual(3, result.Details?.Length);
        Assert.AreEqual(2, result.Details?.Count(x => x.Type == CollectionType.Rubbish));
        Assert.AreEqual(0, result.Details?.Count(x => x.Type == CollectionType.FoodScraps));
        Assert.AreEqual(1, result.Details?.Count(x => x.Type == CollectionType.Recycling));
    }

    [TestMethod]
    public async Task NoSearchAddressTest()
    {
        var httpParameter = new MockHttpParameter() { RequestUrl = "https://www.aucklandcouncil.govt.nz/_vti_bin/ACWeb/ACservices.svc/GetMatchingPropertyAddresses", ResponseContent = "[]" };
        var collection = GetBaseCollection(new MockHttpParameter[] { httpParameter }, new MockHtmlDocument[] { });
        var result = await collection.GetCollection("!@#$%^&*(");

        Assert.IsNotNull(result);
        Assert.AreEqual("Could not match a single street address. Please try another address or be more specific", result.Error);
    }


    [TestMethod]
    public async Task NoCollectionDetailsTest()
    {
        var httpParameter = new MockHttpParameter() { RequestUrl = "https://www.aucklandcouncil.govt.nz/_vti_bin/ACWeb/ACservices.svc/GetMatchingPropertyAddresses", ResponseContent = "[{\"ACRateAccountKey\":\"12346212126\",\"Address\":\"Karaka Bay, Great Barrier Island\",\"Suggestion\":\"Karaka Bay, Great Barrier Island\"}]" };
        var htmlDocumentParameter = new MockHtmlDocument() { RequestUrl = $"https://www.aucklandcouncil.govt.nz/rubbish-recycling/rubbish-recycling-collections/Pages/collection-day-detail.aspx?an=12346212126", HtmlString = EmbeddedResourceHelper.GetResourceFromHtml($"{CouncilType}.{nameof(NoCollectionDetailsTest)}") };

        var collection = GetBaseCollection(new MockHttpParameter[] { httpParameter }, new MockHtmlDocument[] { htmlDocumentParameter });
        var result = await collection.GetCollection("Karaka Bay, Great Barrier Island");

        Assert.IsNotNull(result);
        Assert.AreEqual("No collection details found for this street", result.Error);
    }
}
