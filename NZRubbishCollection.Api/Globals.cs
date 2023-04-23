using NZRubbishCollection.Shared.Enums;

namespace NZRubbishCollection.Api;

/// <summary>
/// Global settings 
/// </summary>
public class Globals
{
    /// <summary>
    /// Gets or sets the council
    /// </summary>
    public static string Council { get; set; }
    
    /// <summary>
    /// Gets or sets the street address
    /// </summary>
    public static string StreetAddress { get; set; }

    /// <summary>
    /// Gets or sets the collection types to include in data
    /// </summary>
    public static CollectionType CollectionTypes { get; set; } =
        CollectionType.Recycling | CollectionType.Rubbish | CollectionType.FoodScraps;
}