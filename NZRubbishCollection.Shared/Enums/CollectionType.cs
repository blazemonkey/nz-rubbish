namespace NZRubbishCollection.Shared.Enums;

/// <summary>
/// Type of the Collection
/// </summary>
[Flags]
public enum CollectionType
{
    /// <summary>
    /// Rubbish Collection
    /// </summary>
    Rubbish = 1,
    /// <summary>
    /// Recycling Collection
    /// </summary>
    Recycling = 2,
    /// <summary>
    /// Food Scraps Collection
    /// </summary>
    FoodScraps = 4
}
