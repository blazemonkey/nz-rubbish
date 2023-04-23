using NZRubbishCollection.Shared.Enums;

namespace NZRubbishCollection.Api;

/// <summary>
/// Command line interface
/// </summary>
public class CommandLine
{
    /// <summary>
    /// Parses the command line arguments
    /// </summary>
    /// <param name="args">the command line args</param>
    public static void Parse(string[] args)
    {
        Globals.Council = Environment.GetEnvironmentVariable("Council");
        Globals.StreetAddress = Environment.GetEnvironmentVariable("StreetAddress");
        if (int.TryParse(Environment.GetEnvironmentVariable("Types") ?? string.Empty, out int iCollectionTypes))
            Globals.CollectionTypes = (CollectionType)iCollectionTypes;
        
        if (args?.Any() != true)
            return;
        for (int i = 0; i < args.Length - 1; i++)
        {
            var arg = args[i].ToLowerInvariant();
            if (arg == "--council")
                Globals.Council = args[++i];
            else if (arg == "--street")
                Globals.StreetAddress = args[++i];
            else if (arg == "--types")
            {
                if(int.TryParse(args[++i], out int iTypes))
                    Globals.CollectionTypes = (CollectionType)iTypes;
            }
        }
    }
}