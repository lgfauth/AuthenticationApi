using MicroservicesLogger.Enums;
using MicroservicesLogger.Models;

namespace MicroservicesLogger.Interfaces
{
    public interface IApiLog<T1> where T1 : LogObject
    {
        Task<T1> CreateBaseLogAsync();
        Task<T1> GetBaseLogAsync();
        Task WriteLogAsync(LogTypes typeLog, T1 value);
        Task WriteLogAsync(T1 value);
    }
}
