using System.Diagnostics.CodeAnalysis;

namespace Domain.Settings
{
    [ExcludeFromCodeCoverage]
    public class MongoDbData
    {
        public string user { get; set; } = string.Empty;
        public string passsword { get; set; } = string.Empty;
        public string cluster { get; set; } = string.Empty;
    }
}