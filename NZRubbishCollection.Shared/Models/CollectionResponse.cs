using NZRubbishCollection.Shared.Enums;

namespace NZRubbishCollection.Shared.Models;

/// <summary>
/// Response with street rubbish collection details
/// </summary>
public class CollectionResponse
{
    /// <summary>
    /// Gets or sets the street address of the collection details
    /// </summary>
    public string? StreetAddress { get; set; }
    /// <summary>
    /// Gets or sets the Url of the page where the source of the collection details can be found
    /// </summary>
    public string? SourceUrl { get; set; }
    /// <summary>
    /// Gets or sets the details containing the dates of the collection
    /// </summary>
    public CollectionDetail[]? Details { get; set; }
    /// <summary>
    /// Getse or sets the time taken to retrieve the details
    /// </summary>
    public TimeSpan TimeTaken { get; set; }
    /// <summary>
    /// Gets or sets any error that occured during retrieval
    /// </summary>
    public string? Error { get; set; }
}

/// <summary>
/// Response with the different types of a collection can be
/// </summary>
public class CollectionDetail
{
    /// <summary>
    /// Gets or sets the type of the collection
    /// </summary>
    public CollectionType Type { get; set; }
    /// <summary>
    /// Gets or sets the date of the collection
    /// </summary>
    public DateTime Date { get; set; }
    /// <summary>
    /// Gets or sets the full description about the collection
    /// </summary>
    public string? Description { get; set; }
}