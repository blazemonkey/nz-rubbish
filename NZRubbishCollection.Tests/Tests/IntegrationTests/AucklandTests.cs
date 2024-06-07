using NZRubbishCollection.Shared.Collections;
using NZRubbishCollection.Shared.Enums;

namespace NZRubbishCollection.Tests.Tests.IntegrationTests;

/// <summary>
/// Integration Tests for Auckland
/// </summary>
[TestClass]
public class AucklandTests : BaseIntegrationTests
{
    /// <summary>
    /// Council for Tests
    /// </summary>
    public override Council CouncilType => Council.Auckland;

    [TestMethod]
    public async Task HasAllTest()
    {
        var collection = GetBaseCollection();
        var result = await collection.GetCollection("10 Popokatea Drive");

        Assert.IsNotNull(result);
        Assert.IsNull(result.Error);
        Assert.AreEqual("https://www.aucklandcouncil.govt.nz/rubbish-recycling/rubbish-recycling-collections/Pages/collection-day-detail.aspx?an=12343873601", result.SourceUrl);
        Assert.AreEqual("10 Popokatea Drive, Takanini", result.StreetAddress?.Trim());
        Assert.AreEqual(3, result.Details?.Length);
        Assert.AreEqual(1, result.Details?.Count(x => x.Type == CollectionType.Rubbish));
        Assert.AreEqual(1, result.Details?.Count(x => x.Type == CollectionType.FoodScraps));
        Assert.AreEqual(1, result.Details?.Count(x => x.Type == CollectionType.Recycling));
    }

    [TestMethod]
    public async Task HasRecyclingTest()
    {
        var collection = GetBaseCollection();
        var result = await collection.GetCollection("2 Surf View Crescent");

        Assert.IsNotNull(result);
        Assert.IsNull(result.Error);
        Assert.AreEqual("https://www.aucklandcouncil.govt.nz/rubbish-recycling/rubbish-recycling-collections/Pages/collection-day-detail.aspx?an=12345874265", result.SourceUrl);
        Assert.AreEqual("2 Surf View Crescent, Red Beach", result.StreetAddress?.Trim());
        Assert.AreEqual(2, result.Details?.Length);
        Assert.AreEqual(0, result.Details?.Count(x => x.Type == CollectionType.Rubbish));
        Assert.AreEqual(1, result.Details?.Count(x => x.Type == CollectionType.FoodScraps));
        Assert.AreEqual(1, result.Details?.Count(x => x.Type == CollectionType.Recycling));
    }

    [TestMethod]
    public async Task HasRubbishAndRecyclingTest()
    {
        var collection = GetBaseCollection();
        var result = await collection.GetCollection("Clevedon-Takanini Road, Clevedon");

        Assert.IsNotNull(result);
        Assert.IsNull(result.Error);
        Assert.AreEqual("https://www.aucklandcouncil.govt.nz/rubbish-recycling/rubbish-recycling-collections/Pages/collection-day-detail.aspx?an=12345049582", result.SourceUrl);
        Assert.AreEqual("Clevedon-Takanini Road, Clevedon", result.StreetAddress?.Trim());
        Assert.AreEqual(2, result.Details?.Length);
        Assert.AreEqual(1, result.Details?.Count(x => x.Type == CollectionType.Rubbish));
        Assert.AreEqual(0, result.Details?.Count(x => x.Type == CollectionType.FoodScraps));
        Assert.AreEqual(1, result.Details?.Count(x => x.Type == CollectionType.Recycling));
    }

    [TestMethod]
    public async Task NoSearchAddressTest()
    {
        var collection = GetBaseCollection();
        var result = await collection.GetCollection("!@#$%^&*(");

        Assert.IsNotNull(result);
        Assert.AreEqual("Could not match a single street address. Please try another address or be more specific", result.Error);
    }

    [TestMethod]
    public async Task NoCollectionDetailsTest()
    {
        var collection = GetBaseCollection();
        var result = await collection.GetCollection("Karaka Bay");

        Assert.IsNotNull(result);
        Assert.AreEqual("No collection details found for this street", result.Error);
    }
}
