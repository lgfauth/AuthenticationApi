using System.Diagnostics.CodeAnalysis;

namespace Domain.Settings
{
    [ExcludeFromCodeCoverage]
    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = "LFauthDevHubLTDA";
        public string Audience { get; set; } = "MyAuthApiClients";
        public int ExpirationMinutes { get; set; } = 60;
    }
}
