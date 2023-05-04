using System.Security.Cryptography;
using System.Text;

namespace NZRubbishCollection.Shared.Helpers;

/// <summary>
/// Helper for generating hashes
/// </summary>
public class HashHelper
{
    /// <summary>
    /// Computes the MD5 hash value of a string and returns it as a hexadecimal string.
    /// </summary>
    /// <param name="input">The input string to hash.</param>
    /// <returns>A hexadecimal string representation of the MD5 hash of the input string.</returns>
    public static string Md5Hash(string input)
    {
        using var md5 = MD5.Create();

        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = md5.ComputeHash(inputBytes);

        // Convert the byte array to a hexadecimal string representation of the hash
        StringBuilder sb = new ();
        foreach (byte b in hashBytes)
            sb.Append(b.ToString("x2"));

        return sb.ToString();
    }
}