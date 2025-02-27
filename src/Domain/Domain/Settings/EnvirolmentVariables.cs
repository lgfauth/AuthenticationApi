using System.Diagnostics.CodeAnalysis;

namespace Domain.Settings
{
    [ExcludeFromCodeCoverage]
    public class EnvirolmentVariables
    {
        public string JwtSettings__SecretKey { get; set; } = string.Empty;
        public string JwtSettings__Issuer { get; set; } = string.Empty;
        public string JwtSettings__Audience { get; set; } = string.Empty;
        public int JwtSettings__ExpirationMinutes { get; set; } = 60;
        public string MongoDbData__user { get; set; } = string.Empty;
        public string MongoDbData__passsword { get; set; } = string.Empty;
        public string MongoDbData__cluster { get; set; } = string.Empty;
        public string MongoDbSettings__ConnectionString { get; set; } = string.Empty;
        public string MongoDbSettings__DatabaseName { get; set; } = "mongodb+srv://{0}:{1}@{2}.mongodb.net/";
        public string RabbitMqConfiguration__HostName { get; set; } = string.Empty;
        public string RabbitMqConfiguration__UserName { get; set; } = string.Empty;
        public string RabbitMqConfiguration__Password { get; set; } = string.Empty;
        public string RabbitMqConfiguration__QueueName { get; set; } = string.Empty;
        public string RabbitMqConfiguration__VirtualHost { get; set; } = string.Empty;
    }
}
