using System.Text.Json.Serialization;

namespace Domain.Models.Envelope
{
    /// <summary>
    /// Interface for internal response model.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IResponse<T>
    {
        /// <summary>
        /// It is a flag to check if the response is success.
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        /// It is a data of response.
        /// </summary>
        T Data { get; }

        /// <summary>
        /// It is a error of response.
        /// </summary>

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        ResponseModel Error { get; }
    }
}
