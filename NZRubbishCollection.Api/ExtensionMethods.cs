namespace NZRubbishCollection.Api;

/// <summary>
/// Common extension methods
/// </summary>
public static class ExtensionMethods
{
    /// <summary>
    /// Returns the string, or null if the string is empty
    /// </summary>
    /// <param name="str">the string</param>
    /// <returns>the string or null if empty</returns>
    public static string? EmptyAsNull(this string str)
        => string.IsNullOrEmpty(str) ? null : str;
}