using System.Text.Json.Serialization;

namespace Domain.Models.Envelope
{
    public interface IResponse<T>
    {
        bool IsSuccess { get; }
        T Data { get; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        ResponseModel Error { get; }
    }
}
