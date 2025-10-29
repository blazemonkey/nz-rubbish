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
        Assert.AreEqual("https://new.aucklandcouncil.govt.nz/en/rubbish-recycling/rubbish-recycling-collections/rubbish-recycling-collection-days/12343873601.html", result.SourceUrl);
        Assert.AreEqual("10 Popokatea Drive, Takanini, Auckland 2112", result.StreetAddress?.Trim());
        Assert.AreEqual(3, result.Details?.Length);
        Assert.AreEqual(1, result.Details?.Count(x => x.Type == CollectionType.Rubbish));
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
        Assert.AreEqual("https://new.aucklandcouncil.govt.nz/en/rubbish-recycling/rubbish-recycling-collections/rubbish-recycling-collection-days/12345049582.html", result.SourceUrl);
        Assert.AreEqual("Clevedon-Takanini Road, Clevedon, Auckland 2582", result.StreetAddress?.Trim());
        Assert.AreEqual(2, result.Details?.Length);
        Assert.AreEqual(1, result.Details?.Count(x => x.Type == CollectionType.Rubbish));
        Assert.AreEqual(0, result.Details?.Count(x => x.Type == CollectionType.FoodScraps));
        Assert.AreEqual(1, result.Details?.Count(x => x.Type == CollectionType.Recycling));
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
