namespace Domain.Settings
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = "LFauthDevHubLTDA";
        public string Audience { get; set; } = "MyAuthApiClients";
        public int ExpirationMinutes { get; set; } = 60;
    }
}
