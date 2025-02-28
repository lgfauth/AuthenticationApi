using MicroservicesLogger.Enums;
using MicroservicesLogger.Models;

namespace MicroservicesLogger.Interfaces
{
    public interface IWorkerLog<T1> where T1 : LogObject
    {
        Task<T1> CreateBaseLogAsync();
        Task<T1> GetBaseLogAsync(string name);
        Task WriteLogAsync(LogTypes typeLog, T1 value);
        Task WriteLogAsync(string name);
        Task WriteLogAndClearAsync(string name);
    }
}
