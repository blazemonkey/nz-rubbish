using HtmlAgilityPack;
using NZRubbishCollection.Shared.Enums;
using NZRubbishCollection.Shared.Models;
using NZRubbishCollection.Shared.Services.ScrapingService;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;

namespace NZRubbishCollection.Shared.Collections;

/// <summary>
/// Collection for Auckland City Council
/// </summary>
public class AucklandCollection : BaseCollection
{
    /// <summary>
    /// Gets the name of the Council
    /// </summary>
    public override string CouncilName => "Auckland City Council";

    /// <summary>
    /// Gets the enum of the Council
    /// </summary>
    public override Council CouncilType => Council.Auckland;

    /// <summary>
    /// Gets the Base URL of the Council website
    /// </summary>
    protected override string BaseUrl => "https://new.aucklandcouncil.govt.nz/";

    /// <summary>
    /// Gets the URL that is used to search for a list of matching addresses based on an input
    /// </summary>
    private string GetIdUrl => $"{BaseUrl}nextapi/property?query=";

    /// <summary>
    /// Gets the URL that will get the collection details based on a StreetID that was returned from GetIdUrl
    /// </summary>
    private string CollectionUrl => $"{BaseUrl}en/rubbish-recycling/rubbish-recycling-collections/rubbish-recycling-collection-days/{_streetId}.html";

    /// <summary>
    /// Id that is returned from the GetIdUrl that is then passed to the CollectionUrl to get the collection details
    /// </summary>
    private string? _streetId;

    /// <summary>
    /// Constructor of the class
    /// </summary>
    /// <param name="httpClient">HttpClient that will be used to grab the data from the pages</param>
    /// <param name="scrapingService">Service used for web scraping</param>
    public AucklandCollection(HttpClient httpClient, IScrapingService scrapingService) : base(httpClient, scrapingService) { }

    /// <summary>
    /// Get the collection details for the street address and council region
    /// </summary>
    /// <param name="streetAddress">Street address to search</param>
    /// <returns>A CollectionResponse with the details</returns>
    protected override async Task<CollectionResponse> DoGetCollection(string streetAddress)
    {
        _streetId = await GetStreetId(streetAddress);
        if (string.IsNullOrEmpty(_streetId))
            return new CollectionResponse() { Error = "Could not match a single street address. Please try another address or be more specific" };

        var response = new CollectionResponse();

        var doc = GetDocument(CollectionUrl);
        var streetName = GetStreetName(doc);
        if (string.IsNullOrEmpty(streetName))
            return new CollectionResponse() { Error = "No collection details found for this street" };

        response.StreetAddress = streetName;
        response.SourceUrl = CollectionUrl;

        var dates = GetCollectionDates(doc);
        if (dates.Any() == false)
            return new CollectionResponse() { Error = "No collection details found for this street" };

        response.Details = dates;

        return response;
    }

    /// <summary>
    /// Get the StreetId that is used to get the collection details
    /// </summary>
    /// <param name="streetAddress">Street address to search for</param>
    /// <returns>The StreetId</returns>
    private async Task<string> GetStreetId(string streetAddress)
    {
        var message = await HttpClient.GetAsync($"{GetIdUrl}{streetAddress}");
        if (message.IsSuccessStatusCode == false)
            return string.Empty;

        var content = await message.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<GetIdResponse>(content, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        if (result?.Items.Length != 0 == false)
            return string.Empty;

        var streetId = result?.Items.FirstOrDefault()?.Id ?? string.Empty;
        return streetId;
    }

    /// <summary>
    /// Get the street name that is displayed on the page of the document
    /// </summary>
    /// <param name="doc">Document which shows the collection details for the street address</param>
    /// <returns>Full street name</returns>
    private static string GetStreetName(HtmlDocument doc)
    {
        var streetName = doc.DocumentNode.SelectNodes("//h2[@class='']//span[@class='heading']//span").FirstOrDefault()?.InnerText ?? string.Empty;
        var suburb = doc.DocumentNode.SelectNodes("//h2[@class='']//span[@class='subheading']").FirstOrDefault()?.InnerText ?? string.Empty;

        if (string.IsNullOrEmpty(streetName))
            return string.Empty;

        return string.IsNullOrEmpty(suburb) ? streetName : $"{streetName}, {suburb}";
    }

    private static CollectionDetail[] GetCollectionDates(HtmlDocument doc)
    {
        var dates = new List<CollectionDetail>();

        var nextCollectionNodes = doc.DocumentNode.SelectNodes("//span[text() = 'Household collection']/ancestor::div[@class='card-heading']//following-sibling::div//p[@class='mb-0 lead']");
        var descriptionNodes = doc.DocumentNode.SelectNodes("//span[text() = 'Household collection']/ancestor::div[@class='card-body']//following-sibling::div[@class='card-footer']//span[@class='acpl-icon-with-attribute left']");

        if (nextCollectionNodes == null || descriptionNodes == null)
            return dates.ToArray();

        foreach (var n in nextCollectionNodes)
        {
            var splitText = n.InnerText?.Trim().Split([":"], StringSplitOptions.None) ?? [];
            if (splitText.Length == 0)
                continue;
            
            var typeText = splitText[0].Replace(":", "").Trim().ToLower();
            CollectionType type = typeText.ToLower() switch
            {
                var t when t.Contains("rubbish") => CollectionType.Rubbish,
                var t when t.Contains("recycling") => CollectionType.Recycling,
                var t when t.Contains("food") || t.Contains("scrap") => CollectionType.FoodScraps,
                _ => 0
            };
            
            if (type == 0)
                continue;

            var dateText = splitText[1].Trim();
            if (string.IsNullOrEmpty(dateText))
                continue;
            
            var date = ParseCollectionDate(dateText, DateTime.Now.Year);

            var description = "";
            var descriptionNode = descriptionNodes.FirstOrDefault(x => x?.InnerText?.ToLower()?.Trim() == typeText);
            if (descriptionNode != null)
            {
                description = descriptionNode.NextSibling.NextSibling.NextSibling.InnerText;
                // the description should start with Collection Day:
                // if it doesn't, try next sibling one more time
                if (description.StartsWith("Collection day") == false)
                    description = descriptionNode.NextSibling.NextSibling.NextSibling.NextSibling.InnerText;

                if (description.StartsWith("Collection day") == false)
                    description = string.Empty;
            }

            var detail = new CollectionDetail()
            {
                Type = type,
                Description = description,
                Date = date,
            };

            dates.Add(detail);
        }

        return dates.ToArray();
    }

    /// <summary>
    /// Parse the string date text returned from the collection page as an actual DateTime object
    /// </summary>
    /// <param name="date">Date of the collection as text, e.g. Tuesday 7 March</param>
    /// <param name="year">The year to pass, first passes in the current year, if that fails use the next year incase the next collection is next year</param>
    /// <returns>DateTime of the collection</returns>
    private static DateTime ParseCollectionDate(string date, int year)
    {
        while (true)
        {
            var fullDate = $"{date} {year}";
            var success = DateTime.TryParseExact(fullDate, "dddd, d MMMM yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parseDate);
            if (success) return parseDate;
            year += 1;
        }
    }

    /// <summary>
    /// Root response for getting StreetId
    /// </summary>
    private record GetIdResponse(StreetItem[] Items);

    /// <summary>
    /// Item matched from getting StreetId
    /// </summary>
    /// <param name="Id">Id of the property</param>
    /// <param name="Address">Address of the property</param>
    private record StreetItem(string? Id, string? Address);
}
