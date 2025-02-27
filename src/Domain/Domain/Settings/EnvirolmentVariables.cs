using System.Diagnostics.CodeAnalysis;

namespace Domain.Settings
{
    [ExcludeFromCodeCoverage]
    public class EnvirolmentVariables
    {
        public string JWTSETTINGS__SECRETKEY { get; set; } = string.Empty;
        public string JWTSETTINGS__ISSUER { get; set; } = string.Empty;
        public string JWTSETTINGS__AUDIENCE { get; set; } = string.Empty;
        public int JWTSETTINGS__EXPIRATIONMINUTES { get; set; } = 60;
        public string MONGODBDATA__USER { get; set; } = string.Empty;
        public string MONGODBDATA__PASSWORD { get; set; } = string.Empty;
        public string MONGODBDATA__CLUSTER { get; set; } = string.Empty;
        public string MONGODBSETTINGS__CONNECTIONSTRING { get; set; } = string.Empty;
        public string MONGODBSETTINGS__DATABASENAME { get; set; } = "mongodb+srv://{0}:{1}@{2}.mongodb.net/";
        public string RABBITMQCONFIGURATION__HOSTNAME { get; set; } = string.Empty;
        public string RABBITMQCONFIGURATION__USERNAME { get; set; } = string.Empty;
        public string RABBITMQCONFIGURATION__PASSWORD { get; set; } = string.Empty;
        public string RABBITMQCONFIGURATION__QUEUENAME { get; set; } = string.Empty;
        public string RABBITMQCONFIGURATION__VIRTUALHOST { get; set; } = string.Empty;
    }
}
