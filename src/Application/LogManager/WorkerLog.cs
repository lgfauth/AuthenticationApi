using MicroservicesLogger.Enums;
using MicroservicesLogger.Interfaces;
using MicroservicesLogger.Models;
using System.Collections.Concurrent;

namespace MicroservicesLogger
{
    public class WorkerLog<Tlog> : IWorkerLog<Tlog> where Tlog : LogObject
    {
        private readonly LoggerManager _logManager;
        private readonly ConcurrentDictionary<string, Tlog> _logs;

        public WorkerLog()
        {
            if (_logManager is null)
                _logManager = new LoggerManager();

            _logs = new ConcurrentDictionary<string, Tlog>();
        }

        public Task<Tlog> CreateBaseLogAsync() =>
            Task.FromResult((Tlog)Activator.CreateInstance(typeof(Tlog))!)!;

        public Task<Tlog> GetBaseLogAsync(string name)
        {
            _logs.TryGetValue(name, out var log);
            return Task.FromResult(log)!;
        }

        public async Task WriteLogAndClearAsync(string name)
        {
            await WriteLogAsync(name);
            _logs.TryRemove(name, out _);
        }

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

            _logManager.WriteLog(value.Level, value);

            return Task.CompletedTask;
        }

        public async Task WriteLogAsync(string name)
        {
            var obj = await GetBaseLogAsync(name);
            await WriteLogAsync(obj.Level, obj);
        }
    }
}