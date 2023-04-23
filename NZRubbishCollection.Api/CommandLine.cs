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
        if (args?.Any() != true)
            return;
        for (int i = 0; i < args.Length - 2; i++)
        {
            var arg = args[i].ToLowerInvariant();
            if (arg == "--council")
            {
                Globals.Council = args[i + 1];
                i++;
            }
            else if (arg == "--street")
            {
                Globals.StreetAddress = args[i + 1];
                i++;
            }
        }
    }
}