using System.Diagnostics.CodeAnalysis;

namespace MicroservicesLogger.Models
{
    [ExcludeFromCodeCoverage]
    public class TraceLog
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}