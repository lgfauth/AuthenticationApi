using MicroservicesLogger.Models;

namespace Application.LogModels
{
    public class ApiLogModel : LogObject
    {
        public string? Endpoint { get; set; }
    }
}
