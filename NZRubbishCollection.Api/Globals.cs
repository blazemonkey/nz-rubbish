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
    
    
    private static HttpClient _HttpClient;

    /// <summary>
    /// Gets or sets the HttpClient. This is not mocked because Integration tests will make actual HTTP calls
    /// </summary>
    public static HttpClient HttpClient
    {
        get
        {
            if (_HttpClient == null)
            {
                var handler = new HttpClientHandler
                {
                    SslProtocols = System.Security.Authentication.SslProtocols.Tls | System.Security.Authentication.SslProtocols.Tls11 | System.Security.Authentication.SslProtocols.Tls12,
                    ServerCertificateCustomValidationCallback = (_, _, _, _)
                        => true
                };

                _HttpClient = new(handler);
            }

            return _HttpClient;
        }
        set => _HttpClient = value;
    } 
}