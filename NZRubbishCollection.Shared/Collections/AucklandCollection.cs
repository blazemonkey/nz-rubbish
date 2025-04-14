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
    protected override string BaseUrl => "https://www.aucklandcouncil.govt.nz/";

    /// <summary>
    /// Gets the URL that is used to search for a list of matching addresses based on an input
    /// </summary>
    private string GetIdUrl { get => $"{BaseUrl}_vti_bin/ACWeb/ACservices.svc/GetMatchingPropertyAddresses"; }

    /// <summary>
    /// Gets the URL that will get the collection details based on a StreetID that was returned from GetIdUrl
    /// </summary>
    private string CollectionUrl { get => $"{BaseUrl}rubbish-recycling/rubbish-recycling-collections/Pages/collection-day-detail.aspx?an={_streetId}"; }

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
        var message = await HttpClient.PostAsJsonAsync(GetIdUrl, new { SearchText = streetAddress, ResultCount = "1", RateKeyRequired = "false" }, new JsonSerializerOptions() { PropertyNamingPolicy = null }); // must set naming policy to null or it'll turn to camel case which the endpoint doesn't like
        if (message.IsSuccessStatusCode == false)
            return string.Empty;

        var content = await message.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<GetIdResponse[]>(content, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? new GetIdResponse[] { };
        if (result.Any() == false)
            return string.Empty;

        var streetId = result.FirstOrDefault()?.ACRateAccountKey ?? string.Empty;
        return streetId;
    }

    /// <summary>
    /// Get the street name that is displayed on the page of the document
    /// </summary>
    /// <param name="doc">Document which shows the collection details for the street address</param>
    /// <returns>Full street name</returns>
    private static string GetStreetName(HtmlDocument doc)
    {
        var streetName = doc.DocumentNode.SelectNodes("//h2").Where(x => x.HasClass("m-b-2")).FirstOrDefault()?.InnerText ?? string.Empty;
        return streetName;
    }

    private static CollectionDetail[] GetCollectionDates(HtmlDocument doc)
    {
        var dates = new List<CollectionDetail>();

        var nextCollectionNodes = doc.DocumentNode.SelectNodes("//div[@class='card-header'][h4[@class='card-title h2' and text()='Household collection']]//h5[@class='collectionDayDate']");
        var descriptionNodes = doc.DocumentNode.SelectNodes("//div[@class='card-header'][h4[@class='card-title h2' and text()='Household collection']]/following-sibling::div/div/h6");

        if (nextCollectionNodes == null || descriptionNodes == null)
            return dates.ToArray();

        foreach (var n in nextCollectionNodes)
        {
            var typeText = n.InnerText?.Trim()?.Split(["\r\n", "\r", "\n"], StringSplitOptions.None)[0]?.Replace(":", "")?.ToLower() ?? string.Empty;
            CollectionType type = typeText switch
            {
                "rubbish" => CollectionType.Rubbish,
                "recycling" => CollectionType.Recycling,
                "food scraps" => CollectionType.FoodScraps,
                _ => 0
            };

            if (type == 0)
                continue;

            var dateText = n.SelectSingleNode("strong")?.InnerText?.Trim() ?? string.Empty;
            var date = ParseCollectionDate(dateText, DateTime.Now.Year);

            var description = "";
            var descriptionNode = descriptionNodes.FirstOrDefault(x => x?.InnerText?.ToLower()?.Trim() == typeText);
            if (descriptionNode != null)
            {
                description = descriptionNode.NextSibling.NextSibling.InnerText;
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
        var fullDate = $"{date} {year}";
        var success = DateTime.TryParseExact(fullDate, "dddd d MMMM yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parseDate);
        if (success == false)
            return ParseCollectionDate(date, year + 1);

        return parseDate;
    }

    /// <summary>
    /// Item matched from getting StreetId
    /// </summary>
    class GetIdResponse
    {
        /// <summary>
        /// Gets or sets the Id that will be used to pass to CollectionUrl
        /// </summary>
        public string? ACRateAccountKey { get; set; }

        /// <summary>
        /// Gets or sets the Address that has been matched
        /// </summary>
        public string? Address { get; set; }
    }
}
