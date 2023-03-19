using System.Reflection;

namespace NZRubbishCollection.Tests;

/// <summary>
/// Helper class to grab the contents of embedded resources
/// </summary>
public class EmbeddedResourceHelper
{
    private static readonly Assembly _assembly;

    /// <summary>
    /// Static constructor to set the assembly variable so we don't have to get it over and over again
    /// </summary>
    static EmbeddedResourceHelper()
    {
        _assembly = IntrospectionExtensions.GetTypeInfo(typeof(EmbeddedResourceHelper)).Assembly;
    }

    /// <summary>
    /// Get the HTML string content of a embedded resource
    /// </summary>
    /// <param name="name">Name of the embedded resource</param>
    /// <returns>HTML string content</returns>
    public static string? GetResourceFromHtml(string name)
    {
        using var stream = _assembly.GetManifestResourceStream($"NZRubbishCollection.Tests.Sample.{name}.html");
        if (stream == null)
            return default;

        var reader = new StreamReader(stream);
        var html = reader.ReadToEnd();
        return html;
    }
}
