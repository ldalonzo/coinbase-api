namespace LDZ.Coinbase.Api.Options
{
    /// <summary>
    /// Before being able to sign any requests, you must create an API key via the Coinbase Pro website. The API key will be scoped to a specific profile.
    /// <para>
    /// The <see cref="Key"/> and <see cref="Secret"/> will be randomly generated and provided by Coinbase Pro; the <see cref="Passphrase"/> will be provided
    /// by you to further secure your API access.
    /// </para>
    /// </summary>
    public class CoinbaseApiKeyOptions
    {
        public string? Key { get; set; }

        public string? Secret { get; set; }

        public string? Passphrase { get; set; }
    }
}
