using MicroservicesLogger.Enums;
using MicroservicesLogger.Utilities;
using Newtonsoft.Json;
using NLog;
using System.Diagnostics.CodeAnalysis;

namespace MicroservicesLogger
{
    [ExcludeFromCodeCoverage]
    public sealed class LoggerManager : IDisposable
    {
        private readonly Logger message;

        public LoggerManager()
        {
            message = LogManager.GetCurrentClassLogger();
        }

        public void WriteLog(LogTypes type, object obj)
        {
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };

            var jsonObject = Strings.Clean(JsonConvert.SerializeObject(obj, jsonSerializerSettings));

            var originalColor = Console.ForegroundColor;
            switch (type)
            {
                case LogTypes.INFO:
                    Console.ForegroundColor = ConsoleColor.Green;
                    message.Info(jsonObject);
                    break;
                case LogTypes.WARN:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    message.Warn(jsonObject);
                    break;
                case LogTypes.ERROR:
                    Console.ForegroundColor = ConsoleColor.Red;
                    message.Error(jsonObject);
                    break;
                case LogTypes.FATAL:
                default:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    message.Fatal(jsonObject);
                    break;
            }
            
            Console.ForegroundColor = originalColor;
        }

        public void Dispose() => LogManager.Shutdown();
    }
}
