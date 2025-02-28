using MicroservicesLogger.Enums;
using MicroservicesLogger.Interfaces;
using MicroservicesLogger.Models;

namespace MicroservicesLogger
{
    public class ApiLog<Tlog> : IApiLog<Tlog> where Tlog : LogObject
    {
        private readonly LoggerManager _loggerManager;
        private Tlog? _baseLog;
        public ApiLog()
        {
            if (_loggerManager is null)
                _loggerManager = new LoggerManager();
        }

        public Task<Tlog> CreateBaseLogAsync()
        {
            _baseLog = (Tlog)Activator.CreateInstance(typeof(Tlog))!;

            return Task.FromResult(_baseLog);
        }

        public Task<Tlog> GetBaseLogAsync() => Task.FromResult(_baseLog)!;

        public Task WriteLogAsync(LogTypes typeLog, Tlog value)
        {
            long elapsedMilliseconds = 0;

            foreach (var step in value.Steps.Values)
            {
                if (step is not null && step is SubLog)
                    ((SubLog)step).StopCronometer();

                elapsedMilliseconds += ((SubLog)step!).ElapsedMilliseconds;
            }

            value.ElapsedMilliseconds = elapsedMilliseconds;

            _loggerManager.WriteLog(typeLog, value);

            return Task.CompletedTask;
        }

        public async Task WriteLogAsync(Tlog value) => await WriteLogAsync(value.Level, value);
    }
}
