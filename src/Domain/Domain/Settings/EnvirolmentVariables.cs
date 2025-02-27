using System.Diagnostics.CodeAnalysis;

namespace Domain.Settings
{
    [ExcludeFromCodeCoverage]
    public class EnvirolmentVariables
    {
        public string JWTSETTINGS_SECRETKEY { get; set; } = string.Empty;
        public string JWTSETTINGS_ISSUER { get; set; } = string.Empty;
        public string JWTSETTINGS_AUDIENCE { get; set; } = string.Empty;
        public int JWTSETTINGS_EXPIRATIONMINUTES { get; set; } = 60;
        public string MONGODBDATA_USER { get; set; } = string.Empty;
        public string MONGODBDATA_PASSWORD { get; set; } = string.Empty;
        public string MONGODBDATA_CLUSTER { get; set; } = string.Empty;
        public string MONGODBSETTINGS_CONNECTIONSTRING { get; set; } = string.Empty;
        public string MONGODBSETTINGS_DATABASENAME { get; set; } = "mongodb+srv://{0}:{1}@{2}.mongodb.net/";
        public string RABBITMQCONFIGURATION_HOSTNAME { get; set; } = string.Empty;
        public string RABBITMQCONFIGURATION_USERNAME { get; set; } = string.Empty;
        public string RABBITMQCONFIGURATION_PASSWORD { get; set; } = string.Empty;
        public string RABBITMQCONFIGURATION_QUEUENAME { get; set; } = string.Empty;
        public string RABBITMQCONFIGURATION_VIRTUALHOST { get; set; } = string.Empty;
    }
}
