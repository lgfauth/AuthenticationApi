using MicroservicesLogger.Enums;
using MicroservicesLogger.Utilities;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace MicroservicesLogger.Models
{
    [ExcludeFromCodeCoverage]
    public class LogObject : IDisposable
    {
        public string Id { get; set; }
        public object? Request { get; set; }
        public object? Response { get; set; }
        public DateTime ExecutedAt { get; set; }
        public long ElapsedMilliseconds { get; set; }
        public ConcurrentDictionary<string, object> Steps { get; set; }
        public List<TraceLog> TraceStep { get; set; }

        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public LogTypes Level { get; set; }

        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        internal ConcurrentDictionary<string, int> StepCount { get; set; }

        public LogObject()
        {
            Level = LogTypes.INFO;
            Id = Guid.NewGuid().ToString();
            TraceStep = new List<TraceLog>();
            StepCount = new ConcurrentDictionary<string, int>();
            Steps = new ConcurrentDictionary<string, object>();
            ExecutedAt = DateConverter.ToBrazilianDateTime(DateTime.UtcNow);
        }

        public Task AddStepAsync(string baseStep, SubLog? subLog = null, bool? addCountStep = null)
        {
            if (subLog is null)
                subLog = new SubLog();

            TraceStep.Add(new TraceLog { Id = TraceStep.Count + 1, Name = baseStep });

            subLog.TraceId = TraceStep.Count;

            if (StepCount!.GetValueOrDefault(baseStep) >= 1)
            {
                StepCount.TryGetValue(baseStep, out int total);

                if (total >= 2)
                {
                    total++;

                    var logName = $"{baseStep}_{total.ToString().PadLeft(3, '0')}";

                    Steps.TryAdd(logName, subLog);
                    StepCount.TryUpdate(baseStep, total, total - 1);
                }
                else
                {
                    var logName = $"{baseStep}_{total.ToString().PadLeft(3, '0')}";

                    total++;

                    var _logName = $"{baseStep}_{total.ToString().PadLeft(3, '0')}";

                    Steps.TryRemove(baseStep, out var obj);
                    Steps.TryAdd(logName, obj!);
                    Steps.TryAdd(_logName, subLog);
                    StepCount.TryUpdate(baseStep, total, total - 1);
                }
            }
            else if (addCountStep.HasValue && addCountStep.Value)
            {
                StepCount.TryGetValue(baseStep, out int total);

                total++;

                var logName = $"{baseStep}_{total.ToString().PadLeft(3, '0')}";

                Steps.TryAdd(baseStep, subLog);
                StepCount.TryAdd(baseStep, total);
            }
            else
            {
                Steps.TryAdd(baseStep, subLog);
                StepCount.TryAdd(baseStep, 1);
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (Request != null)
                Request = null;

            if (Response != null)
                Response = null;

            if (Steps != null)
                Steps.Clear();
        }
    }
}
